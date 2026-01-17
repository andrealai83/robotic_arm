import { Injectable, signal } from '@angular/core';

export interface Movimento {
    m1: number;
    m2: number;
    m3: number;
    m4: number;
    c: string;
}

export interface PositionSet {
    id: number;
    name: string;
    movements: Movimento[];
}

@Injectable({
    providedIn: 'root'
})
export class RobotService {
    private apiUrl = '/api';

    // State signals
    public connectionStatus = signal<'connected' | 'disconnected'>('disconnected');
    public currentPath = signal<string>('');

    constructor() { }

    async connect(path: string = 'COM5', baudRate: number = 115200): Promise<void> {
        try {
            const res = await fetch(`${this.apiUrl}/connect`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ path, baudRate })
            });
            if (res.ok) {
                this.connectionStatus.set('connected');
                this.currentPath.set(path);
            } else {
                throw new Error((await res.json()).error);
            }
        } catch (err) {
            console.error(err);
            this.connectionStatus.set('disconnected');
            throw err;
        }
    }

    async getPorts(): Promise<string[]> {
        const res = await fetch(`${this.apiUrl}/ports`);
        return res.json();
    }

    async moveRaw(mov: Movimento): Promise<void> {
        await fetch(`${this.apiUrl}/move/raw`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(mov)
        });
    }

    async moveCartesian(target: { x: number, y: number, z: number }, current?: Movimento): Promise<void> {
        await fetch(`${this.apiUrl}/move/cartesian`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ ...target, ...current })
        });
    }

    async getSequences(): Promise<PositionSet[]> {
        const res = await fetch(`${this.apiUrl}/sequences`);
        return res.json();
    }

    async saveSequence(set: PositionSet): Promise<void> {
        await fetch(`${this.apiUrl}/sequences`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(set)
        });
    }

    async runSequence(id: number): Promise<void> {
        await fetch(`${this.apiUrl}/sequence/run/${id}`, { method: 'POST' });
    }

    async home(axis: string): Promise<void> {
        await fetch(`${this.apiUrl}/home/${axis}`, { method: 'POST' });
    }
}
