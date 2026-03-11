import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EndstopState, Movimento, RobotConfiguration, RobotService, PositionSet, SerialLogEntry } from '../../services/robot.service';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
    @ViewChild('viewerFrame') viewerFrame?: ElementRef<HTMLIFrameElement>;

    // Connection
    ports: string[] = [];
    selectedPort: string = 'COM5';

    // Manual Control
    m1: number = 0;
    m2: number = 0;
    m3: number = 0;
    m5: number = 0;
    m6: number = 0;
    gripOpen: boolean = false;
    magnet: boolean = false;
    hasPendingManual: boolean = false;
    isExecutingManual: boolean = false;

    // Sequences
    sequences: PositionSet[] = [];
    draftSequenceName: string = 'Nuovo Set';
    draftMovements: Movimento[] = [];
    loopSequenceId: number | null = null;
    isLoopRunning: boolean = false;
    config: RobotConfiguration = {
        passiPerGiro: 200,
        microstep: 4,
        maxSpeed: 5000,
        maxAccel: 2000
    };
    logs: SerialLogEntry[] = [];
    consoleCommand = '';
    endstopTelemetryEnabled = false;
    endstopState: EndstopState = {
        e1: null,
        e2: null,
        e3: null,
        e4: null,
        updatedAt: null
    };
    private logsTimer: ReturnType<typeof setInterval> | null = null;
    private lastViewerMoveLogKey: string | null = null;

    constructor(
        public robotService: RobotService,
        private readonly cdr: ChangeDetectorRef
    ) { }

    async ngOnInit() {
        this.refreshPorts();
        this.refreshSequences();
        await this.loadConfig();
        await this.refreshLogs();
        this.startLogsPolling();
    }

    ngAfterViewInit(): void {
        this.pushViewerAngles();
    }

    ngOnDestroy(): void {
        if (this.logsTimer) {
            clearInterval(this.logsTimer);
            this.logsTimer = null;
        }
    }

    async refreshPorts() {
        try {
            this.ports = await this.robotService.getPorts();
            if (this.ports.length > 0) this.selectedPort = this.ports[0];
        } catch (e) { console.error(e); }
    }

    async connect() {
        await this.robotService.connect(this.selectedPort);
        await this.loadConfig();
        await this.refreshLogs();
    }

    async playManual() {
        if (this.isExecutingManual) return;
        this.isExecutingManual = true;
        try {
            await this.robotService.moveManualExec(
                this.m1,
                this.m2,
                this.m3,
                this.m5,
                this.m6,
                this.currentGripCommand,
                20000
            );
            this.hasPendingManual = false;
            this.pushViewerAngles();
            await this.refreshLogs();
        } catch (err) {
            console.error(err);
            await this.refreshLogs();
        } finally {
            this.isExecutingManual = false;
        }
    }

    simulateMove() {
        this.previewViewerAngles();
    }

    async refreshSequences() {
        this.sequences = await this.robotService.getSequences();
    }

    async runSequence(id: number) {
        if (this.isLoopRunning) return;
        await this.robotService.runSequence(id);
        await this.refreshLogs();
    }

    addCurrentPositionToDraft() {
        this.draftMovements.push({
            m1: Math.round(this.m1),
            m2: Math.round(this.m2),
            m3: Math.round(this.m3),
            m5: Math.round(this.m5),
            m6: Math.round(this.m6),
            grip: this.currentGripCommand,
            c: this.magnet ? 'C:1' : 'C:0'
        });
    }

    removeDraftMovement(index: number) {
        this.draftMovements.splice(index, 1);
    }

    clearDraftMovements() {
        this.draftMovements = [];
    }

    async saveDraftSequence() {
        const name = this.draftSequenceName.trim();
        if (!name || this.draftMovements.length === 0) {
            return;
        }
        await this.robotService.saveSequence({
            id: 0,
            name,
            movements: [...this.draftMovements]
        });
        this.clearDraftMovements();
        await this.refreshSequences();
    }

    async toggleSequenceLoop(id: number) {
        if (this.isLoopRunning && this.loopSequenceId === id) {
            this.isLoopRunning = false;
            this.loopSequenceId = null;
            return;
        }

        if (this.isLoopRunning) {
            return;
        }

        this.isLoopRunning = true;
        this.loopSequenceId = id;
        try {
            while (this.isLoopRunning && this.loopSequenceId === id) {
                await this.robotService.runSequence(id);
                await this.refreshLogs();
                await this.delay(150);
            }
        } catch (err) {
            console.error(err);
        } finally {
            this.isLoopRunning = false;
            this.loopSequenceId = null;
        }
    }

    onSliderChange() {
        this.hasPendingManual = true;
    }

    async emergencyStop() {
        this.isLoopRunning = false;
        this.loopSequenceId = null;
        await this.robotService.sendSerial('EMERGENCY_STOP');
        await this.refreshLogs();
    }

    async home(axis: string) {
        await this.robotService.home(axis);
        // Reset sliders if successful (optimistic UI)
        if (axis === 'all') {
            this.m1 = 0; this.m2 = 0; this.m3 = 0; this.m5 = 0; this.m6 = 0;
        } else if (axis === '1') this.m1 = 0;
        else if (axis === '2') this.m2 = 0;
        else if (axis === '3') this.m3 = 0;
        else if (axis === '5') this.m5 = 0;
        else if (axis === '6') this.m6 = 0;
        this.hasPendingManual = false;
        this.pushViewerAngles();
    }

    setGripper(open: boolean) {
        this.gripOpen = open;
        this.hasPendingManual = true;
    }

    async loadConfig() {
        this.config = await this.robotService.getConfig();
    }

    async saveConfig() {
        await this.robotService.saveConfig(this.config);
        await this.refreshLogs();
    }

    async refreshLogs() {
        this.logs = await this.robotService.getLogs();
        if (this.isLoopRunning) {
            this.cdr.detectChanges();
            return;
        }
        this.syncViewerFromLogs();
        this.cdr.detectChanges();
    }

    async clearLogs() {
        await this.robotService.clearLogs();
        this.logs = [];
        this.endstopState = {
            e1: null,
            e2: null,
            e3: null,
            e4: null,
            updatedAt: null
        };
        this.cdr.detectChanges();
    }

    async sendConsoleCommand() {
        const cmd = this.consoleCommand.trim();
        if (!cmd) {
            return;
        }
        await this.robotService.sendSerial(cmd);
        this.consoleCommand = '';
        await this.refreshLogs();
    }

    async toggleEndstopTelemetry() {
        this.endstopTelemetryEnabled = !this.endstopTelemetryEnabled;
        this.startLogsPolling();
        await this.robotService.setEndstopTelemetry(this.endstopTelemetryEnabled);
        await this.refreshLogs();
    }

    onViewerLoaded() {
        this.pushViewerAngles();
    }

    private previewViewerAngles(durationMs: number = 900) {
        const win = this.viewerFrame?.nativeElement?.contentWindow;
        if (!win) {
            return;
        }
        win.postMessage({
            type: 'previewAngles',
            m1: this.m1,
            m2: this.m2,
            m3: this.m3,
            m4: -this.m2,
            durationMs
        }, window.location.origin);
    }

    private pushViewerAngles(m1: number = this.m1, m2: number = this.m2, m3: number = this.m3) {
        const win = this.viewerFrame?.nativeElement?.contentWindow;
        if (!win) {
            return;
        }
        win.postMessage({
            type: 'setAngles',
            m1,
            m2,
            m3,
            m4: -m2
        }, window.location.origin);
    }

    private syncViewerFromLogs() {
        this.syncEndstopStateFromLogs();

        const latestMoveLog = [...this.logs].reverse().find((row) =>
            row.dir === 'TX' &&
            row.message.includes('RUN\\n') &&
            row.message.includes('M1:') &&
            row.message.includes('M2:') &&
            row.message.includes('M3:')
        );
        if (!latestMoveLog) {
            return;
        }

        const logKey = `${latestMoveLog.ts}|${latestMoveLog.message}`;
        if (this.lastViewerMoveLogKey === logKey) {
            return;
        }

        const match = latestMoveLog.message.match(
            /M1:([-+]?\d*\.?\d+)(?:\\n|;)\s*M2:([-+]?\d*\.?\d+)(?:\\n|;)\s*M3:([-+]?\d*\.?\d+)(?:\\n|;)/
        );
        if (!match) {
            return;
        }

        const m1 = Number(match[1]);
        const m2 = Number(match[2]);
        const m3 = Number(match[3]);
        if (![m1, m2, m3].every(Number.isFinite)) {
            return;
        }

        this.lastViewerMoveLogKey = logKey;
        this.pushViewerAngles(m1, m2, m3);
    }

    private syncEndstopStateFromLogs() {
        const latestEndstopLog = [...this.logs].reverse().find((row) =>
            row.dir === 'RX' && row.message.startsWith('ENDSTOP ')
        );
        if (!latestEndstopLog) {
            return;
        }

        const match = latestEndstopLog.message.match(/E1:(\d)\s+E2:(\d)\s+E3:(\d)\s+E4:(\d)/);
        if (!match) {
            return;
        }

        this.endstopState = {
            e1: match[1] === '1',
            e2: match[2] === '1',
            e3: match[3] === '1',
            e4: match[4] === '1',
            updatedAt: latestEndstopLog.ts
        };
    }

    private delay(ms: number) {
        return new Promise<void>((resolve) => setTimeout(resolve, ms));
    }

    private startLogsPolling() {
        if (this.logsTimer) {
            clearInterval(this.logsTimer);
        }

        const intervalMs = this.endstopTelemetryEnabled ? 150 : 700;
        this.logsTimer = setInterval(() => { void this.refreshLogs(); }, intervalMs);
    }

    get currentGripCommand(): string {
        return this.gripOpen ? 'GRIP:120' : 'GRIP:0';
    }

    get endstopCards() {
        return [
            { label: 'E1', value: this.endstopState.e1 },
            { label: 'E2', value: this.endstopState.e2 },
            { label: 'E3', value: this.endstopState.e3 },
            { label: 'E4', value: this.endstopState.e4 }
        ];
    }
}
