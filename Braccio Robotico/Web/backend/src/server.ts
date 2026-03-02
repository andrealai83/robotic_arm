import express from 'express';
import cors from 'cors';
import { SerialManager } from './serial-manager';
import { Storage } from './storage';
import { KinematicsHelper, CartesianPosition, Movimento } from './kinematics';
import { TrajectoryInterpolator } from './trajectory';
import { RobotConfigStore, RobotConfiguration } from './robot-config';

const app = express();
const port = 3000;

app.use(cors());
app.use(express.json());

const serial = new SerialManager(); // Default COM5, can be changed via API
const storage = new Storage();
const robotConfigStore = new RobotConfigStore();
let manualExecBusy = false;
let sequenceExecBusy = false;

type SerialLogEntry = { ts: string; dir: 'RX' | 'TX' | 'SYS'; message: string };
const logBuffer: SerialLogEntry[] = [];
const LOG_MAX = 500;

const pushLog = (dir: 'RX' | 'TX' | 'SYS', message: string) => {
    logBuffer.push({ ts: new Date().toISOString(), dir, message });
    if (logBuffer.length > LOG_MAX) {
        logBuffer.splice(0, logBuffer.length - LOG_MAX);
    }
};

serial.onDataReceived = (data: string) => pushLog('RX', data);

// Serial endpoints
app.post('/api/connect', async (req, res) => {
    const { path, baudRate } = req.body;
    if (path) serial.setPath(path);
    try {
        await serial.open();
        pushLog('SYS', `Connected to ${path || 'default'}`);
        const config = robotConfigStore.get();
        await initializeDevice(config);
        res.json({ status: 'connected', path: path || 'default', initialized: true, config });
    } catch (err: any) {
        res.status(500).json({ error: err.message });
    }
});

app.get('/api/ports', async (req, res) => {
    try {
        const ports = await serial.listPorts();
        res.json(ports);
    } catch (err: any) {
        res.status(500).json({ error: err.message });
    }
});

// Movement endpoints
app.post('/api/move/raw', (req, res) => {
    const mov = req.body as Movimento;
    const cmd = buildRunCommand(mov);
    sendSerial(cmd);
    res.json({ status: 'sent', command: cmd });
});

app.post('/api/move/manual-exec', async (req, res) => {
    if (manualExecBusy) {
        return res.status(409).json({ error: 'Manual movement already in progress' });
    }

    const m1 = Number(req.body?.m1);
    const m2 = Number(req.body?.m2);
    const m3 = Number(req.body?.m3);
    const timeoutRaw = Number(req.body?.timeoutMs);
    const timeoutMs = Number.isFinite(timeoutRaw) ? Math.max(500, Math.min(60000, timeoutRaw)) : 20000;

    if (![m1, m2, m3].every(Number.isFinite)) {
        return res.status(400).json({ error: 'Invalid manual angles' });
    }

    // Mirror desktop manual command style: M1/M3/M2 + EXEC (no M4).
    const command = `M1:${Math.round(m1)};M3:${Math.round(m3)};M2:${Math.round(m2)};EXEC`;

    manualExecBusy = true;
    try {
        const ready = await sendAndWaitReady(command, timeoutMs);
        if (!ready) {
            pushLog('SYS', `Timeout waiting 'ready' on manual exec: ${command}`);
            return res.status(504).json({ error: "Timeout waiting 'ready'", command, timeoutMs });
        }
        res.json({ status: 'completed', command });
    } finally {
        manualExecBusy = false;
    }
});

app.get('/api/config', (req, res) => {
    res.json(robotConfigStore.get());
});

app.put('/api/config', async (req, res) => {
    try {
        const config = robotConfigStore.set(req.body as Partial<RobotConfiguration>);
        if (serial.isOpen()) {
            await applyConfiguration(config);
        }
        res.json({ status: 'saved', applied: serial.isOpen(), config });
    } catch (err: any) {
        res.status(500).json({ error: err.message });
    }
});

app.get('/api/logs', (req, res) => {
    res.json(logBuffer);
});

app.post('/api/logs/clear', (req, res) => {
    logBuffer.length = 0;
    res.json({ status: 'ok' });
});

app.post('/api/serial/send', (req, res) => {
    const command = String(req.body?.command ?? '').trim();
    if (!command) {
        return res.status(400).json({ error: 'Empty command' });
    }
    sendSerial(command.endsWith('\n') ? command : `${command}\n`);
    res.json({ status: 'sent' });
});

app.post('/api/move/cartesian', (req, res) => {
    const { x, y, z, m1, m2, m3 } = req.body;
    // m1..m3 are current angles to help IK choose nearest solution

    const target = new CartesianPosition(x, y, z);
    const current = (m1 !== undefined) ? { m1, m2, m3, c: 'C:0' } as Movimento : null;

    const mov = KinematicsHelper.InverseKinematics(target, current);

    if (mov) {
        const cmd = buildRunCommand(mov);
        sendSerial(cmd);
        res.json({ status: 'calculated', movement: mov, command: cmd });
    } else {
        res.status(400).json({ error: 'Target unreachable' });
    }
});

app.post('/api/home/:axis', (req, res) => {
    const axis = req.params.axis; // 'all', '1', '2', '3', '4'
    let cmd = '';

    if (axis === 'all') cmd = 'HOME\n';
    else if (['1', '2', '3', '4'].includes(axis)) cmd = `HOME_${axis}\n`;
    else return res.status(400).json({ error: 'Invalid axis' });

    sendSerial(cmd);
    res.json({ status: 'homing', axis, command: cmd.trim() });
});

// Storage endpoints
app.get('/api/sequences', (req, res) => {
    res.json(storage.getAll());
});

app.post('/api/sequences', (req, res) => {
    storage.save(req.body);
    res.json({ status: 'saved' });
});

app.delete('/api/sequences/:id', (req, res) => {
    const id = parseInt(req.params.id);
    storage.delete(id);
    res.json({ status: 'deleted' });
});

app.post('/api/sequence/run/:id', (req, res) => {
    const id = parseInt(req.params.id);
    const set = storage.getAll().find(s => s.id === id);
    if (!set) {
        return res.status(404).json({ error: 'Sequence not found' });
    }

    // Create interpolated trajectory
    const trajectory = TrajectoryInterpolator.InterpolateMovements(set.movements, 5);

    // Execute sequence (simple implementation: send all with delay?)
    // Real implementation needs feedback or simple delay.
    // For now, let's just send them with a small delay between each.

    // NOTE: This relies on the Arduino buffer or simple timing.
    // A better approach would be to send one, wait for 'ready', send next.
    // But since this is a simple http request, we might just return the full list 
    // and let the frontend drive the execution? 
    // OR we execute here. Let's execute here for "smart" controller.

    runSequence(trajectory, { waitReady: false, stepDelayMs: 200, stepTimeoutMs: 0 });

    res.json({ status: 'started', steps: trajectory.length });
});

app.post('/api/sequence/run-sync/:id', async (req, res) => {
    if (sequenceExecBusy) {
        return res.status(409).json({ error: 'Sequence execution already in progress' });
    }

    const id = parseInt(req.params.id);
    const set = storage.getAll().find(s => s.id === id);
    if (!set) {
        return res.status(404).json({ error: 'Sequence not found' });
    }

    const trajectory = TrajectoryInterpolator.InterpolateMovements(set.movements, 5);
    sequenceExecBusy = true;
    try {
        const result = await runSequence(trajectory, { waitReady: true, stepDelayMs: 0, stepTimeoutMs: 20000 });
        if (!result.ok) {
            return res.status(504).json({ error: "Timeout waiting 'ready' during sequence", failedAt: result.failedAt });
        }
        res.json({ status: 'completed', steps: trajectory.length });
    } finally {
        sequenceExecBusy = false;
    }
});

async function runSequence(
    moves: Movimento[],
    options: { waitReady: boolean; stepDelayMs: number; stepTimeoutMs: number }
): Promise<{ ok: true } | { ok: false; failedAt: number }> {
    for (let i = 0; i < moves.length; i++) {
        const mov = moves[i];
        const cmd = buildRunCommand(mov);
        if (options.waitReady) {
            sendSerial(cmd);
            const ready = await waitForLine((msg) => msg === 'ready', options.stepTimeoutMs);
            if (!ready) {
                return { ok: false, failedAt: i };
            }
        } else {
            sendSerial(cmd);
            if (options.stepDelayMs > 0) {
                await new Promise(r => setTimeout(r, options.stepDelayMs));
            }
        }
    }
    return { ok: true };
}

function buildRunCommand(mov: Movimento): string {
    return `M1:${mov.m1.toFixed(2)}\nM2:${mov.m2.toFixed(2)}\nM3:${mov.m3.toFixed(2)}\n${mov.c}\nRUN\n`;
}

function sendSerial(data: string): void {
    serial.write(data);
    pushLog('TX', data.replace(/\r/g, '\\r').replace(/\n/g, '\\n'));
}

function wait(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function waitForLine(predicate: (line: string) => boolean, timeoutMs: number): Promise<string | null> {
    return new Promise<string | null>((resolve) => {
        const timeout = setTimeout(() => {
            serial.removeDataListener(onLine);
            resolve(null);
        }, timeoutMs);

        const onLine = (line: string) => {
            if (!predicate(line)) return;
            clearTimeout(timeout);
            serial.removeDataListener(onLine);
            resolve(line);
        };

        serial.addDataListener(onLine);
    });
}

async function sendAndWaitOk(command: string, timeoutMs: number): Promise<boolean> {
    sendSerial(`${command}\n`);
    const line = await waitForLine((msg) => msg === 'ok', timeoutMs);
    return line !== null;
}

async function sendAndWaitReady(command: string, timeoutMs: number): Promise<boolean> {
    sendSerial(`${command}\n`);
    const line = await waitForLine((msg) => msg === 'ready', timeoutMs);
    return line !== null;
}

async function applyConfiguration(config: RobotConfiguration): Promise<void> {
    const commands = robotConfigStore.buildConfigurationCommands(config);
    for (const cfg of commands) {
        let ok = false;
        for (let attempt = 1; attempt <= 3; attempt++) {
            ok = await sendAndWaitOk(cfg, 4000);
            if (ok) break;
            pushLog('SYS', `Retry ${attempt}/3: ${cfg}`);
            await wait(250);
        }
        if (!ok) {
            throw new Error(`Timeout ack su: ${cfg}`);
        }
    }
}

async function initializeDevice(config: RobotConfiguration): Promise<void> {
    await wait(1500);
    serial.clearBuffers();
    await waitForLine((line) => line === 'Sistema pronto.', 8000);
    await applyConfiguration(config);
}

app.listen(port, () => {
    console.log(`Backend listening at http://localhost:${port}`);
});
