import * as fs from 'fs';
import * as path from 'path';

export interface RobotConfiguration {
    passiPerGiro: number;
    microstep: number;
    maxSpeed: number;
    maxAccel: number;
}

const DEFAULT_CONFIG: RobotConfiguration = {
    passiPerGiro: 200,
    microstep: 4,
    maxSpeed: 5000,
    maxAccel: 2000
};

export class RobotConfigStore {
    private readonly filePath = path.join(__dirname, '../data/robot-config.json');

    constructor() {
        this.ensureFileExists();
    }

    public get(): RobotConfiguration {
        try {
            const raw = fs.readFileSync(this.filePath, 'utf-8');
            const parsed = JSON.parse(raw) as Partial<RobotConfiguration>;
            return this.normalize(parsed);
        } catch {
            return { ...DEFAULT_CONFIG };
        }
    }

    public set(next: Partial<RobotConfiguration>): RobotConfiguration {
        const merged = this.normalize({ ...this.get(), ...next });
        fs.writeFileSync(this.filePath, JSON.stringify(merged, null, 2));
        return merged;
    }

    public buildConfigurationCommands(config: RobotConfiguration): string[] {
        return [
            `CFG:passiPerGiro:${config.passiPerGiro}`,
            `CFG:microstep:${config.microstep}`,
            `CFG:maxSpeed:${config.maxSpeed}`,
            `CFG:maxAccel:${config.maxAccel}`
        ];
    }

    private ensureFileExists(): void {
        const dir = path.dirname(this.filePath);
        if (!fs.existsSync(dir)) {
            fs.mkdirSync(dir, { recursive: true });
        }
        if (!fs.existsSync(this.filePath)) {
            fs.writeFileSync(this.filePath, JSON.stringify(DEFAULT_CONFIG, null, 2));
        }
    }

    private normalize(input: Partial<RobotConfiguration>): RobotConfiguration {
        const intOr = (value: unknown, fallback: number): number => {
            const n = Number(value);
            if (!Number.isFinite(n)) {
                return fallback;
            }
            return Math.max(1, Math.round(n));
        };

        return {
            passiPerGiro: intOr(input.passiPerGiro, DEFAULT_CONFIG.passiPerGiro),
            microstep: intOr(input.microstep, DEFAULT_CONFIG.microstep),
            maxSpeed: intOr(input.maxSpeed, DEFAULT_CONFIG.maxSpeed),
            maxAccel: intOr(input.maxAccel, DEFAULT_CONFIG.maxAccel)
        };
    }
}

