import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Movimento, RobotConfiguration, RobotService, PositionSet, SerialLogEntry } from '../../services/robot.service';

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
    private logsTimer: ReturnType<typeof setInterval> | null = null;

    constructor(public robotService: RobotService) { }

    async ngOnInit() {
        this.refreshPorts();
        this.refreshSequences();
        await this.loadConfig();
        await this.refreshLogs();
        this.logsTimer = setInterval(() => { void this.refreshLogs(); }, 700);
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
            await this.robotService.moveManualExec(this.m1, this.m2, this.m3, 20000);
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
            this.m1 = 0; this.m2 = 0; this.m3 = 0;
        } else if (axis === '1') this.m1 = 0;
        else if (axis === '2') this.m2 = 0;
        else if (axis === '3') this.m3 = 0;
        this.hasPendingManual = false;
        this.pushViewerAngles();
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
    }

    async clearLogs() {
        await this.robotService.clearLogs();
        this.logs = [];
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

    private pushViewerAngles() {
        const win = this.viewerFrame?.nativeElement?.contentWindow;
        if (!win) {
            return;
        }
        win.postMessage({
            type: 'setAngles',
            m1: this.m1,
            m2: this.m2,
            m3: this.m3,
            m4: -this.m2
        }, window.location.origin);
    }

    private delay(ms: number) {
        return new Promise<void>((resolve) => setTimeout(resolve, ms));
    }
}
