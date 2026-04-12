import { SerialPort } from 'serialport';
import { ReadlineParser } from '@serialport/parser-readline';

export class SerialManager {
    private port: SerialPort | null = null;
    private parser: ReadlineParser | null = null;
    private readonly listeners = new Set<(data: string) => void>();

    // Event callback
    public onDataReceived: ((data: string) => void) | null = null;

    constructor(private path: string = 'COM5', private baudRate: number = 115200) { }

    public async open(): Promise<void> {
        return new Promise((resolve, reject) => {
            if (this.port && this.port.isOpen) {
                resolve();
                return;
            }

            this.port = new SerialPort({ path: this.path, baudRate: this.baudRate, autoOpen: false });

            this.parser = this.port.pipe(new ReadlineParser({ delimiter: '\r\n' }));

            this.parser.on('data', (data: string) => {
                const trimmed = data.trim();
                // console.log('Serial RX:', trimmed); 
                if (this.onDataReceived) {
                    this.onDataReceived(trimmed);
                }
                for (const listener of this.listeners) {
                    listener(trimmed);
                }
            });

            this.port.open((err) => {
                if (err) {
                    console.error('Error opening port:', err.message);
                    reject(err);
                } else {
                    console.log(`Serial port ${this.path} opened at ${this.baudRate}`);
                    resolve();
                }
            });
        });
    }

    public close(): void {
        if (this.port && this.port.isOpen) {
            this.port.close();
        }
    }

    public write(data: string): void {
        if (this.port && this.port.isOpen) {
            this.port.write(data, (err) => {
                if (err) console.error('Error writing to serial:', err.message);
            });
        } else {
            console.warn('Serial port not open, cannot write:', data);
        }
    }

    public isOpen(): boolean {
        return !!this.port?.isOpen;
    }

    public addDataListener(listener: (data: string) => void): void {
        this.listeners.add(listener);
    }

    public removeDataListener(listener: (data: string) => void): void {
        this.listeners.delete(listener);
    }

    public clearBuffers(): void {
        if (!this.port || !this.port.isOpen) {
            return;
        }
        try {
            this.port.flush();
            this.port.drain();
        } catch {
            // best effort
        }
    }

    public async listPorts(): Promise<string[]> {
        const ports = await SerialPort.list();
        return ports.map(p => p.path);
    }

    // Update port path dynamically
    public setPath(path: string) {
        if (this.port && this.port.isOpen) {
            this.close();
        }
        this.path = path;
    }
}
