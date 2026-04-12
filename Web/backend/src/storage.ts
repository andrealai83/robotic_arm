import * as fs from 'fs';
import * as path from 'path';
import { Movimento } from './kinematics';

export interface PositionSet {
    id: number;
    name: string;
    movements: Movimento[];
}

export class Storage {
    private readonly filePath = path.join(__dirname, '../data/positions.json');

    constructor() {
        this.ensureFileExists();
    }

    private ensureFileExists() {
        const dir = path.dirname(this.filePath);
        if (!fs.existsSync(dir)) {
            fs.mkdirSync(dir, { recursive: true });
        }
        if (!fs.existsSync(this.filePath)) {
            fs.writeFileSync(this.filePath, JSON.stringify([], null, 2));
        }
    }

    public getAll(): PositionSet[] {
        try {
            const data = fs.readFileSync(this.filePath, 'utf-8');
            return JSON.parse(data) as PositionSet[];
        } catch (err) {
            console.error('Error reading storage:', err);
            return [];
        }
    }

    public save(set: PositionSet): void {
        const all = this.getAll();
        const index = all.findIndex(p => p.id === set.id);

        if (index >= 0) {
            all[index] = set;
        } else {
            // New ID if not provided or 0
            if (!set.id) {
                const maxId = all.reduce((max, curr) => Math.max(max, curr.id), 0);
                set.id = maxId + 1;
            }
            all.push(set);
        }

        fs.writeFileSync(this.filePath, JSON.stringify(all, null, 2));
    }

    public delete(id: number): void {
        let all = this.getAll();
        all = all.filter(p => p.id !== id);
        fs.writeFileSync(this.filePath, JSON.stringify(all, null, 2));
    }
}
