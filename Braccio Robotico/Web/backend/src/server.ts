import express from 'express';
import cors from 'cors';
import { SerialManager } from './serial-manager';
import { Storage } from './storage';
import { KinematicsHelper, CartesianPosition, Movimento } from './kinematics';
import { TrajectoryInterpolator } from './trajectory';

const app = express();
const port = 3000;

app.use(cors());
app.use(express.json());

const serial = new SerialManager(); // Default COM5, can be changed via API
const storage = new Storage();

// Serial endpoints
app.post('/api/connect', async (req, res) => {
    const { path, baudRate } = req.body;
    if (path) serial.setPath(path);
    try {
        await serial.open();
        res.json({ status: 'connected', path: path || 'default' });
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
    const cmd = `M1:${mov.m1}\nM2:${mov.m2}\nM4:${mov.m4}\nM3:${mov.m3}\n${mov.c}\nRUN\n`;
    serial.write(cmd);
    res.json({ status: 'sent', command: cmd });
});

app.post('/api/move/cartesian', (req, res) => {
    const { x, y, z, m1, m2, m3, m4 } = req.body;
    // m1..m4 are current angles to help IK choose nearest solution

    const target = new CartesianPosition(x, y, z);
    const current = (m1 !== undefined) ? { m1, m2, m3, m4, c: 'C:0' } as Movimento : null;

    const mov = KinematicsHelper.InverseKinematics(target, current);

    if (mov) {
        const cmd = `M1:${mov.m1.toFixed(2)}\nM2:${mov.m2.toFixed(2)}\nM4:${mov.m4.toFixed(2)}\nM3:${mov.m3.toFixed(2)}\n${mov.c}\nRUN\n`;
        serial.write(cmd);
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

    serial.write(cmd);
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

    runSequence(trajectory);

    res.json({ status: 'started', steps: trajectory.length });
});

async function runSequence(moves: Movimento[]) {
    for (const mov of moves) {
        const cmd = `M1:${mov.m1.toFixed(2)}\nM2:${mov.m2.toFixed(2)}\nM4:${mov.m4.toFixed(2)}\nM3:${mov.m3.toFixed(2)}\n${mov.c}\nRUN\n`;
        serial.write(cmd);
        // Wait estimate
        await new Promise(r => setTimeout(r, 200)); // Simple delay for now
    }
}

app.listen(port, () => {
    console.log(`Backend listening at http://localhost:${port}`);
});
