import express from 'express';
import cors from 'cors';
import { SerialManager } from './serial-manager';
import { Storage } from './storage';
import { KinematicsHelper, CartesianPosition, Movimento } from './kinematics';
import { RobotConfigStore, RobotConfiguration } from './robot-config';
import { AiProvider, OllamaAiProvider, AiCommand, interpretFastPath } from './ai';

type SerialLike = Pick<
    SerialManager,
    'setPath' | 'open' | 'listPorts' | 'write' | 'isOpen' | 'addDataListener' | 'removeDataListener' | 'clearBuffers'
> & {
    onDataReceived: ((data: string) => void) | null;
};

type StorageLike = Pick<Storage, 'getAll' | 'save' | 'delete'>;
type RobotConfigStoreLike = Pick<RobotConfigStore, 'get' | 'set' | 'buildConfigurationCommands'>;
type SerialLogEntry = { ts: string; dir: 'RX' | 'TX' | 'SYS'; message: string };
type GripPressureState = {
    p1: number | null;
    p2: number | null;
    max: number | null;
    updatedAt: string | null;
    source: 'GRIP' | 'PRESS' | null;
};

interface AppDependencies {
    serial?: SerialLike;
    storage?: StorageLike;
    robotConfigStore?: RobotConfigStoreLike;
    aiProvider?: AiProvider;
    port?: number;
    initializeDelayMs?: number;
}

export function createApp(deps: AppDependencies = {}) {
    const app = express();
    const port = deps.port ?? 3002;
    const serial = deps.serial ?? new SerialManager();
    const storage = deps.storage ?? new Storage();
    const robotConfigStore = deps.robotConfigStore ?? new RobotConfigStore();
    const aiProvider = deps.aiProvider ?? new OllamaAiProvider();
    const initializeDelayMs = deps.initializeDelayMs ?? 1500;
    let manualExecBusy = false;
    let sequenceExecBusy = false;

    app.use(cors());
    app.use(express.json());

    const logBuffer: SerialLogEntry[] = [];
    const LOG_MAX = 500;
    const gripPressure: GripPressureState = {
        p1: null,
        p2: null,
        max: null,
        updatedAt: null,
        source: null
    };

    const pushLog = (dir: 'RX' | 'TX' | 'SYS', message: string) => {
        logBuffer.push({ ts: new Date().toISOString(), dir, message });
        if (logBuffer.length > LOG_MAX) {
            logBuffer.splice(0, logBuffer.length - LOG_MAX);
        }
    };

    serial.onDataReceived = (data: string) => {
        pushLog('RX', data);
        updateGripPressureFromLine(data);
    };

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
    const m5 = Number(req.body?.m5 ?? 0);
    const m6 = Number(req.body?.m6 ?? 0);
    const grip = normalizeGrip(req.body?.grip);
    const timeoutRaw = Number(req.body?.timeoutMs);
    const timeoutMs = Number.isFinite(timeoutRaw) ? Math.max(500, Math.min(60000, timeoutRaw)) : 20000;

    if (![m1, m2, m3, m5, m6].every(Number.isFinite)) {
        return res.status(400).json({ error: 'Invalid manual angles' });
    }

    const parts = [
        `M1:${Math.round(m1)}`,
        `M3:${Math.round(m3)}`,
        `M2:${Math.round(m2)}`,
        `M5:${Math.round(m5)}`,
        `M6:${Math.round(m6)}`,
        grip,
        'EXEC'
    ];
    const command = parts.join(';');

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

    app.get('/api/ai/status', async (req, res) => {
    try {
        const status = await aiProvider.health();
        res.json(status);
    } catch (err: any) {
        res.status(503).json({ ok: false, error: err.message });
    }
    });

    app.post('/api/ai/interpret', async (req, res) => {
    const input = String(req.body?.input ?? '').trim();
    if (!input) {
        return res.status(400).json({ error: 'Empty input' });
    }

    try {
        const context = {
            input,
            sequences: storage.getAll(),
            currentJoints: sanitizeCurrentJoints(req.body?.currentJoints)
        };
        const parsed = interpretFastPath(context) ?? await aiProvider.interpret(context);
        const preview = buildAiPreview(parsed, context.currentJoints);
        res.json({
            status: 'ok',
            input,
            parsed,
            preview
        });
    } catch (err: any) {
        res.status(502).json({ error: err.message });
    }
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

    app.get('/api/grip-pressure', (req, res) => {
    res.json(gripPressure);
    });

    app.post('/api/grip-pressure/refresh', async (req, res) => {
    if (!serial.isOpen()) {
        return res.status(409).json({ error: 'Serial port not connected' });
    }

    const line = await requestPressureSample();
    if (!line) {
        return res.status(504).json({ error: 'Timeout waiting pressure sample' });
    }

    res.json(gripPressure);
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
    const axis = req.params.axis; // 'all', '1', '2', '3', '4', '5', '6'
    let cmd = '';

    if (axis === 'all') cmd = 'HOME\n';
    else if (['1', '2', '3', '4', '5', '6'].includes(axis)) cmd = `HOME_${axis}\n`;
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

    const trajectory = [...set.movements];

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

    const trajectory = [...set.movements];
    sequenceExecBusy = true;
    try {
        const result = await runSequence(trajectory, { waitReady: true, stepDelayMs: 30, stepTimeoutMs: 20000 });
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
        }

        if (options.stepDelayMs > 0 && i < moves.length - 1) {
            await wait(options.stepDelayMs);
        }
    }
    return { ok: true };
    }

    function buildRunCommand(mov: Movimento): string {
    const c = (mov.c && mov.c.trim()) ? mov.c.trim() : 'C:0';
    const grip = normalizeGrip(mov.grip);
    const m5 = typeof mov.m5 === 'number' && Number.isFinite(mov.m5) ? mov.m5 : 0;
    const m6 = typeof mov.m6 === 'number' && Number.isFinite(mov.m6) ? mov.m6 : 0;
    // Single-line batch avoids parser interleaving between loop cycles on Arduino.
    return `M1:${mov.m1.toFixed(2)};M2:${mov.m2.toFixed(2)};M3:${mov.m3.toFixed(2)};M5:${m5.toFixed(2)};M6:${m6.toFixed(2)};${grip};${c};RUN\n`;
    }

    function buildAiPreview(
        command: AiCommand,
        currentJoints: { m1: number; m2: number; m3: number; m5: number; m6: number } | null
    ): { endpoint: string; payload: Record<string, unknown> } | null {
    switch (command.intent) {
        case 'home':
            return {
                endpoint: `/api/home/${command.axis}`,
                payload: {}
            };
        case 'move_cartesian':
            return {
                endpoint: '/api/move/cartesian',
                payload: {
                    x: command.x,
                    y: command.y,
                    z: command.z
                }
            };
        case 'move_joint':
            return {
                endpoint: '/api/move/raw',
                payload: {
                    m1: command.m1 ?? currentJoints?.m1 ?? 0,
                    m2: command.m2 ?? currentJoints?.m2 ?? 0,
                    m3: command.m3 ?? currentJoints?.m3 ?? 0,
                    m5: command.m5 ?? currentJoints?.m5 ?? 0,
                    m6: command.m6 ?? currentJoints?.m6 ?? 0,
                    grip: command.grip ?? 'GRIP:0',
                    c: command.c ?? 'C:0'
                }
            };
        case 'move_joint_delta':
            if (!currentJoints) {
                return null;
            }
            return {
                endpoint: '/api/move/raw',
                payload: {
                    m1: currentJoints.m1 + (command.m1 ?? 0),
                    m2: currentJoints.m2 + (command.m2 ?? 0),
                    m3: currentJoints.m3 + (command.m3 ?? 0),
                    m5: currentJoints.m5 + (command.m5 ?? 0),
                    m6: currentJoints.m6 + (command.m6 ?? 0),
                    grip: 'GRIP:0',
                    c: 'C:0'
                }
            };
        case 'run_sequence': {
            const sequence = findSequence(command);
            if (!sequence) {
                return null;
            }
            return {
                endpoint: `/api/sequence/run-sync/${sequence.id}`,
                payload: {}
            };
        }
        case 'set_grip':
            return {
                endpoint: '/api/move/raw',
                payload: {
                    m1: 0,
                    m2: 0,
                    m3: 0,
                    m5: 0,
                    m6: 0,
                    grip: command.grip,
                    c: 'C:0'
                }
            };
        case 'unknown':
            return null;
    }
    }

    function sanitizeCurrentJoints(value: unknown): { m1: number; m2: number; m3: number; m5: number; m6: number } | null {
    const record = value && typeof value === 'object' && !Array.isArray(value)
        ? value as Record<string, unknown>
        : null;
    if (!record) {
        return null;
    }

    const m1 = Number(record.m1);
    const m2 = Number(record.m2);
    const m3 = Number(record.m3);
    const m5 = Number(record.m5);
    const m6 = Number(record.m6);
    if (![m1, m2, m3, m5, m6].every(Number.isFinite)) {
        return null;
    }

    return { m1, m2, m3, m5, m6 };
    }

    function findSequence(command: Extract<AiCommand, { intent: 'run_sequence' }>) {
    const all = storage.getAll();
    if (typeof command.sequenceId === 'number') {
        const byId = all.find((item) => item.id === command.sequenceId);
        if (byId) {
            return byId;
        }
    }

    const requestedName = String(command.sequenceName ?? '').trim().toLowerCase();
    if (!requestedName) {
        return null;
    }

    return all.find((item) => item.name.trim().toLowerCase() === requestedName) ?? null;
    }

    function normalizeGrip(value: unknown): string {
    const grip = String(value ?? '').trim().toUpperCase();
    if (/^GRIP:\d+$/.test(grip)) {
        return grip;
    }
    return 'GRIP:0';
    }

    function sendSerial(data: string): void {
    serial.write(data);
    pushLog('TX', data.replace(/\r/g, '\\r').replace(/\n/g, '\\n'));
    }

    function updateGripPressureFromLine(line: string): void {
    const match = line.match(/(?:^GRIP\b.*\s|^PRESS\s+)P1:(\d+)%\s+P2:(\d+)%\s+MAX:(\d+)/i);
    if (!match) {
        return;
    }

    gripPressure.p1 = Number(match[1]);
    gripPressure.p2 = Number(match[2]);
    gripPressure.max = Number(match[3]);
    gripPressure.updatedAt = new Date().toISOString();
    gripPressure.source = line.startsWith('GRIP ') ? 'GRIP' : 'PRESS';
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
    return sendAndWaitLine(`${command}\n`, (msg) => msg === 'ok', timeoutMs);
    }

    async function sendAndWaitReady(command: string, timeoutMs: number): Promise<boolean> {
    return sendAndWaitLine(`${command}\n`, (msg) => msg === 'ready', timeoutMs);
    }

    async function sendAndWaitLine(data: string, predicate: (line: string) => boolean, timeoutMs: number): Promise<boolean> {
    return new Promise<boolean>((resolve) => {
        const timeout = setTimeout(() => {
            serial.removeDataListener(onLine);
            resolve(false);
        }, timeoutMs);

        const onLine = (line: string) => {
            if (!predicate(line)) return;
            clearTimeout(timeout);
            serial.removeDataListener(onLine);
            resolve(true);
        };

        // Attach listener before sending to avoid missing very fast ACKs.
        serial.addDataListener(onLine);
        sendSerial(data);
    });
    }

    async function requestPressureSample(): Promise<string | null> {
    const waitPromise = waitForLine((line) => /^PRESS\s+P1:\d+%\s+P2:\d+%\s+MAX:\d+$/i.test(line), 1200);
    sendSerial('PRESS?\n');
    const line = await waitPromise;
    if (line) {
        updateGripPressureFromLine(line);
    }
    return line;
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
    await wait(initializeDelayMs);
    serial.clearBuffers();
    await waitForLine((line) => line === 'Sistema pronto.', 8000);
    await applyConfiguration(config);
    }

    return { app, port };
}

if (require.main === module) {
    const { app, port } = createApp();
    app.listen(port, () => {
        console.log(`Backend listening at http://localhost:${port}`);
    });
}
