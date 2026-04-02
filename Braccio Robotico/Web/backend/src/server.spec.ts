import request from 'supertest';
import { describe, expect, it, beforeEach } from 'vitest';
import { createApp } from './server';
import { Movimento } from './kinematics';
import { RobotConfiguration } from './robot-config';

function last<T>(items: T[]): T | undefined {
    return items[items.length - 1];
}

class MockSerial {
    public onDataReceived: ((data: string) => void) | null = null;
    public writes: string[] = [];
    public currentPath = 'COM5';
    public openCalled = false;
    public openState = false;
    public ports = ['COM3', 'COM5'];
    public autoReady = true;
    public autoConfigAck = true;
    private listeners = new Set<(data: string) => void>();

    async open(): Promise<void> {
        this.openCalled = true;
        this.openState = true;
    }

    async listPorts(): Promise<string[]> {
        return this.ports;
    }

    write(data: string): void {
        this.writes.push(data);

        if (this.autoConfigAck && data.startsWith('CFG:')) {
            setTimeout(() => this.emit('ok'), 0);
        }

        if (this.autoReady && (data.includes('EXEC') || data.includes(';RUN\n'))) {
            setTimeout(() => this.emit('ready'), 0);
        }

        if (data === 'PRESS?\n') {
            setTimeout(() => this.emit('PRESS P1:33% P2:47% MAX:47'), 0);
        }
    }

    isOpen(): boolean {
        return this.openState;
    }

    addDataListener(listener: (data: string) => void): void {
        this.listeners.add(listener);
    }

    removeDataListener(listener: (data: string) => void): void {
        this.listeners.delete(listener);
    }

    clearBuffers(): void {
        setTimeout(() => this.emit('Sistema pronto.'), 0);
    }

    setPath(path: string): void {
        this.currentPath = path;
    }

    emit(data: string): void {
        this.onDataReceived?.(data);
        for (const listener of this.listeners) {
            listener(data);
        }
    }
}

class MockStorage {
    constructor(private sets: Array<{ id: number; name: string; movements: Movimento[] }> = []) { }

    getAll() {
        return this.sets;
    }

    save(set: { id: number; name: string; movements: Movimento[] }) {
        const index = this.sets.findIndex((item) => item.id === set.id);
        if (index >= 0) {
            this.sets[index] = set;
            return;
        }
        this.sets.push(set);
    }

    delete(id: number) {
        this.sets = this.sets.filter((item) => item.id !== id);
    }
}

class MockRobotConfigStore {
    public config: RobotConfiguration = {
        passiPerGiro: 200,
        microstep: 4,
        maxSpeed: 5000,
        maxAccel: 2000
    };

    get(): RobotConfiguration {
        return this.config;
    }

    set(next: Partial<RobotConfiguration>): RobotConfiguration {
        this.config = { ...this.config, ...next };
        return this.config;
    }

    buildConfigurationCommands(config: RobotConfiguration): string[] {
        return [
            `CFG:passiPerGiro:${config.passiPerGiro}`,
            `CFG:microstep:${config.microstep}`,
            `CFG:maxSpeed:${config.maxSpeed}`,
            `CFG:maxAccel:${config.maxAccel}`
        ];
    }
}

function buildTestContext(overrides?: {
    serial?: MockSerial;
    storage?: MockStorage;
    robotConfigStore?: MockRobotConfigStore;
}) {
    const serial = overrides?.serial ?? new MockSerial();
    const storage = overrides?.storage ?? new MockStorage();
    const robotConfigStore = overrides?.robotConfigStore ?? new MockRobotConfigStore();
    const { app } = createApp({
        serial,
        storage,
        robotConfigStore,
        initializeDelayMs: 0
    });

    return { app, serial, storage, robotConfigStore };
}

describe('API endpoints', () => {
    let ctx: ReturnType<typeof buildTestContext>;

    beforeEach(() => {
        ctx = buildTestContext();
    });

    it('returns available serial ports', async () => {
        const res = await request(ctx.app).get('/api/ports');

        expect(res.status).toBe(200);
        expect(res.body).toEqual(['COM3', 'COM5']);
    });

    it('connects and applies robot configuration', async () => {
        const res = await request(ctx.app)
            .post('/api/connect')
            .send({ path: 'COM7', baudRate: 115200 });

        expect(res.status).toBe(200);
        expect(res.body.status).toBe('connected');
        expect(res.body.path).toBe('COM7');
        expect(ctx.serial.currentPath).toBe('COM7');
        expect(ctx.serial.openCalled).toBe(true);
        expect(ctx.serial.writes).toEqual([
            'CFG:passiPerGiro:200\n',
            'CFG:microstep:4\n',
            'CFG:maxSpeed:5000\n',
            'CFG:maxAccel:2000\n'
        ]);
    });

    it('builds and sends raw move commands', async () => {
        const res = await request(ctx.app)
            .post('/api/move/raw')
            .send({ m1: 10, m2: 20, m3: 30, c: 'C:1', grip: 'grip:12', m5: 5, m6: 6 });

        expect(res.status).toBe(200);
        expect(res.body.command).toBe('M1:10.00;M2:20.00;M3:30.00;M5:5.00;M6:6.00;GRIP:12;C:1;RUN\n');
        expect(last(ctx.serial.writes)).toBe('M1:10.00;M2:20.00;M3:30.00;M5:5.00;M6:6.00;GRIP:12;C:1;RUN\n');
    });

    it('rejects invalid manual execution payloads', async () => {
        const res = await request(ctx.app)
            .post('/api/move/manual-exec')
            .send({ m1: 'bad', m2: 20, m3: 30 });

        expect(res.status).toBe(400);
        expect(res.body).toEqual({ error: 'Invalid manual angles' });
    });

    it('executes manual movement and waits for ready', async () => {
        const res = await request(ctx.app)
            .post('/api/move/manual-exec')
            .send({ m1: 11.6, m2: 20.2, m3: 30.8, m5: 4, m6: 9, grip: 'grip:7', timeoutMs: 1000 });

        expect(res.status).toBe(200);
        expect(res.body).toEqual({
            status: 'completed',
            command: 'M1:12;M3:31;M2:20;M5:4;M6:9;GRIP:7;EXEC'
        });
        expect(last(ctx.serial.writes)).toBe('M1:12;M3:31;M2:20;M5:4;M6:9;GRIP:7;EXEC\n');
    });

    it('reads, stores and clears logs', async () => {
        ctx.serial.emit('hello');
        await request(ctx.app).post('/api/serial/send').send({ command: 'PING' });

        const beforeClear = await request(ctx.app).get('/api/logs');
        const clear = await request(ctx.app).post('/api/logs/clear');
        const afterClear = await request(ctx.app).get('/api/logs');

        expect(beforeClear.status).toBe(200);
        expect(beforeClear.body).toHaveLength(2);
        expect(beforeClear.body[0].message).toBe('hello');
        expect(beforeClear.body[1].message).toBe('PING\\n');
        expect(clear.body).toEqual({ status: 'ok' });
        expect(afterClear.body).toEqual([]);
    });

    it('tracks grip pressure from serial feedback and refresh endpoint', async () => {
        ctx.serial.openState = true;
        ctx.serial.emit('GRIP A:120 P1:21% P2:18% MAX:21');

        const current = await request(ctx.app).get('/api/grip-pressure');
        const refreshed = await request(ctx.app).post('/api/grip-pressure/refresh');

        expect(current.status).toBe(200);
        expect(current.body.p1).toBe(21);
        expect(current.body.p2).toBe(18);
        expect(current.body.max).toBe(21);
        expect(current.body.source).toBe('GRIP');

        expect(refreshed.status).toBe(200);
        expect(refreshed.body.p1).toBe(33);
        expect(refreshed.body.p2).toBe(47);
        expect(refreshed.body.max).toBe(47);
        expect(refreshed.body.source).toBe('PRESS');
        expect(last(ctx.serial.writes)).toBe('PRESS?\n');
    });

    it('validates home axis and supports wrist endpoints', async () => {
        const ok = await request(ctx.app).post('/api/home/6');
        const bad = await request(ctx.app).post('/api/home/9');

        expect(ok.status).toBe(200);
        expect(ok.body).toEqual({ status: 'homing', axis: '6', command: 'HOME_6' });
        expect(last(ctx.serial.writes)).toBe('HOME_6\n');
        expect(bad.status).toBe(400);
        expect(bad.body).toEqual({ error: 'Invalid axis' });
    });

    it('reads and updates configuration', async () => {
        const initial = await request(ctx.app).get('/api/config');

        ctx.serial.openState = true;
        const updated = await request(ctx.app)
            .put('/api/config')
            .send({ microstep: 8, maxSpeed: 6200 });

        expect(initial.body).toEqual({
            passiPerGiro: 200,
            microstep: 4,
            maxSpeed: 5000,
            maxAccel: 2000
        });
        expect(updated.status).toBe(200);
        expect(updated.body).toEqual({
            status: 'saved',
            applied: true,
            config: {
                passiPerGiro: 200,
                microstep: 8,
                maxSpeed: 6200,
                maxAccel: 2000
            }
        });
    });

    it('lists, saves and deletes sequences', async () => {
        const listBefore = await request(ctx.app).get('/api/sequences');
        const save = await request(ctx.app)
            .post('/api/sequences')
            .send({ id: 1, name: 'demo', movements: [{ m1: 1, m2: 2, m3: 3, c: 'C:0' }] });
        const listAfterSave = await request(ctx.app).get('/api/sequences');
        const del = await request(ctx.app).delete('/api/sequences/1');
        const listAfterDelete = await request(ctx.app).get('/api/sequences');

        expect(listBefore.body).toEqual([]);
        expect(save.body).toEqual({ status: 'saved' });
        expect(listAfterSave.body).toHaveLength(1);
        expect(del.body).toEqual({ status: 'deleted' });
        expect(listAfterDelete.body).toEqual([]);
    });

    it('runs a stored sequence synchronously', async () => {
        ctx = buildTestContext({
            storage: new MockStorage([
                {
                    id: 7,
                    name: 'pick',
                    movements: [
                        { m1: 1, m2: 2, m3: 3, m5: 4, m6: 5, grip: 'GRIP:6', c: 'C:0' },
                        { m1: 10, m2: 20, m3: 30, c: 'C:1' }
                    ]
                }
            ])
        });

        const res = await request(ctx.app).post('/api/sequence/run-sync/7');

        expect(res.status).toBe(200);
        expect(res.body).toEqual({ status: 'completed', steps: 2 });
        expect(ctx.serial.writes).toEqual([
            'M1:1.00;M2:2.00;M3:3.00;M5:4.00;M6:5.00;GRIP:6;C:0;RUN\n',
            'M1:10.00;M2:20.00;M3:30.00;M5:0.00;M6:0.00;GRIP:0;C:1;RUN\n'
        ]);
    });

    it('returns 404 when the requested sequence does not exist', async () => {
        const res = await request(ctx.app).post('/api/sequence/run-sync/999');

        expect(res.status).toBe(404);
        expect(res.body).toEqual({ error: 'Sequence not found' });
    });
});
