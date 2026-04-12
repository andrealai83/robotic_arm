import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
    AiInterpretResponse,
    AiStatus,
    EndstopState,
    GripPressureState,
    Movimento,
    RobotConfiguration,
    RobotService,
    PositionSet,
    SerialLogEntry
} from '../../services/robot.service';

interface SpeechRecognitionAlternativeLike {
    transcript: string;
}

interface SpeechRecognitionResultLike {
    isFinal: boolean;
    length: number;
    item(index: number): SpeechRecognitionAlternativeLike;
    [index: number]: SpeechRecognitionAlternativeLike;
}

interface SpeechRecognitionEventLike extends Event {
    resultIndex: number;
    results: ArrayLike<SpeechRecognitionResultLike>;
}

interface SpeechRecognitionLike extends EventTarget {
    continuous: boolean;
    interimResults: boolean;
    lang: string;
    onstart: ((this: SpeechRecognitionLike, ev: Event) => unknown) | null;
    onend: ((this: SpeechRecognitionLike, ev: Event) => unknown) | null;
    onerror: ((this: SpeechRecognitionLike, ev: Event & { error?: string }) => unknown) | null;
    onresult: ((this: SpeechRecognitionLike, ev: SpeechRecognitionEventLike) => unknown) | null;
    start(): void;
    stop(): void;
}

type SpeechRecognitionCtor = new () => SpeechRecognitionLike;

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
    isExecutingGrip: boolean = false;

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
    naturalCommand = '';
    aiStatus: AiStatus | null = null;
    aiResult: AiInterpretResponse | null = null;
    aiError = '';
    isCheckingAi = false;
    isInterpretingAi = false;
    isExecutingAi = false;
    isListeningVoice = false;
    voiceWakeWord = 'robot';
    voiceSupported = false;
    voiceTranscriptDraft = '';
    voiceLiveTranscript = '';
    voiceWakeDetected = false;
    lastWakeTrigger = '';
    voiceStatusMessage = '';
    voiceStatusTone: 'info' | 'success' | 'error' = 'info';
    endstopTelemetryEnabled = false;
    endstopState: EndstopState = {
        e1: null,
        e2: null,
        e3: null,
        e4: null,
        updatedAt: null
    };
    gripPressure: GripPressureState = {
        p1: null,
        p2: null,
        max: null,
        updatedAt: null,
        source: null
    };
    private logsTimer: ReturnType<typeof setInterval> | null = null;
    private gripPressureTimer: ReturnType<typeof setInterval> | null = null;
    private lastViewerMoveLogKey: string | null = null;
    private speechRecognition: SpeechRecognitionLike | null = null;
    voiceAutoRestart = false;
    private wakeOverlayTimer: ReturnType<typeof setTimeout> | null = null;
    private voiceStatusTimer: ReturnType<typeof setTimeout> | null = null;
    private audioContext: AudioContext | null = null;

    constructor(
        public robotService: RobotService,
        private readonly cdr: ChangeDetectorRef
    ) { }

    async ngOnInit() {
        this.refreshPorts();
        this.refreshSequences();
        this.initVoiceRecognition();
        await this.loadConfig();
        await this.refreshAiStatus();
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
        if (this.gripPressureTimer) {
            clearInterval(this.gripPressureTimer);
            this.gripPressureTimer = null;
        }
        if (this.wakeOverlayTimer) {
            clearTimeout(this.wakeOverlayTimer);
            this.wakeOverlayTimer = null;
        }
        if (this.voiceStatusTimer) {
            clearTimeout(this.voiceStatusTimer);
            this.voiceStatusTimer = null;
        }
        this.audioContext?.close().catch(() => undefined);
        this.stopVoiceRecognition();
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
        await this.refreshAiStatus();
        await this.refreshLogs();
        await this.refreshGripPressure();
        this.startGripPressurePolling();
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
            await this.refreshGripPressure();
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

    async setGripper(open: boolean) {
        if (this.isExecutingGrip) {
            return;
        }

        this.gripOpen = open;
        this.isExecutingGrip = true;
        this.cdr.detectChanges();

        try {
            await this.robotService.sendSerial(this.currentGripCommand);
            await this.trackGripFeedbackUntilSettled();
            await this.refreshLogs();
        } catch (err) {
            console.error(err);
        } finally {
            this.isExecutingGrip = false;
            this.cdr.detectChanges();
        }
    }

    async loadConfig() {
        this.config = await this.robotService.getConfig();
    }

    async saveConfig() {
        await this.robotService.saveConfig(this.config);
        await this.refreshLogs();
    }

    async refreshAiStatus() {
        this.isCheckingAi = true;
        try {
            this.aiStatus = await this.robotService.getAiStatus();
            this.aiError = '';
        } catch (err) {
            this.aiStatus = null;
            this.aiError = err instanceof Error ? err.message : 'AI status unavailable';
        } finally {
            this.isCheckingAi = false;
            this.cdr.detectChanges();
        }
    }

    async interpretAiCommand(autoExecuteSafe: boolean = false) {
        const input = this.naturalCommand.trim();
        if (!input || this.isInterpretingAi) {
            return;
        }

        this.isInterpretingAi = true;
        try {
            this.aiResult = await this.robotService.interpretAiWithCurrent(input, {
                m1: this.m1,
                m2: this.m2,
                m3: this.m3,
                m5: this.m5,
                m6: this.m6
            });
            this.aiError = '';
            if (this.aiResult?.parsed.intent === 'unknown') {
                this.setVoiceStatus('Comando non riconosciuto.', 'error');
                this.playVoiceCue('error');
            } else if (autoExecuteSafe && this.shouldAutoExecuteAi(this.aiResult)) {
                await this.executeAiPreview(true);
            } else if (autoExecuteSafe) {
                this.setVoiceStatus('Comando riconosciuto. In attesa di conferma.', 'info');
            }
        } catch (err) {
            this.aiResult = null;
            this.aiError = err instanceof Error ? err.message : 'AI interpretation failed';
            if (autoExecuteSafe) {
                this.setVoiceStatus('Errore durante l’analisi del comando.', 'error');
                this.playVoiceCue('error');
            }
        } finally {
            this.isInterpretingAi = false;
            this.cdr.detectChanges();
        }
    }

    async executeAiPreview(fromVoice: boolean = false) {
        const preview = this.aiResult?.preview;
        if (!preview || this.isExecutingAi) {
            return;
        }

        this.isExecutingAi = true;
        try {
            const payload = preview.payload;

            switch (preview.endpoint) {
                case '/api/move/cartesian':
                    await this.robotService.moveCartesian({
                        x: Number(payload['x']),
                        y: Number(payload['y']),
                        z: Number(payload['z'])
                    });
                    break;
                case '/api/move/raw':
                    this.applyJointStateFromPayload(payload);
                    await this.robotService.moveRaw({
                        m1: Number(payload['m1'] ?? 0),
                        m2: Number(payload['m2'] ?? 0),
                        m3: Number(payload['m3'] ?? 0),
                        m5: Number(payload['m5'] ?? 0),
                        m6: Number(payload['m6'] ?? 0),
                        grip: String(payload['grip'] ?? 'GRIP:0'),
                        c: String(payload['c'] ?? 'C:0')
                    });
                    break;
                default:
                    if (preview.endpoint.startsWith('/api/home/')) {
                        await this.robotService.home(preview.endpoint.split('/').pop() ?? 'all');
                        this.applyHomeToUi(preview.endpoint.split('/').pop() ?? 'all');
                    } else if (preview.endpoint.startsWith('/api/sequence/run-sync/')) {
                        const id = Number(preview.endpoint.split('/').pop());
                        await this.robotService.runSequence(id);
                    } else {
                        throw new Error(`Unsupported AI preview endpoint: ${preview.endpoint}`);
                    }
            }

            await this.refreshLogs();
            await this.refreshGripPressure(false);
            if (fromVoice) {
                this.setVoiceStatus('Comando eseguito.', 'success');
                this.playVoiceCue('execute');
            }
        } catch (err) {
            this.aiError = err instanceof Error ? err.message : 'AI execution failed';
            if (fromVoice) {
                this.setVoiceStatus('Errore durante l’esecuzione del comando.', 'error');
                this.playVoiceCue('error');
            }
        } finally {
            this.isExecutingAi = false;
            this.cdr.detectChanges();
        }
    }

    toggleVoiceRecognition() {
        if (!this.voiceSupported) {
            this.aiError = 'Riconoscimento vocale non supportato da questo browser.';
            return;
        }

        if (this.voiceAutoRestart || this.isListeningVoice) {
            this.stopVoiceRecognition();
            return;
        }

        this.aiError = '';
        this.voiceTranscriptDraft = '';
        this.voiceLiveTranscript = '';
        this.lastWakeTrigger = '';
        this.voiceStatusMessage = '';
        this.voiceAutoRestart = true;
        this.speechRecognition?.start();
    }

    async refreshLogs() {
        this.logs = await this.robotService.getLogs();
        if (this.isLoopRunning) {
            this.syncGripPressureFromLogs();
            this.cdr.detectChanges();
            return;
        }
        this.syncViewerFromLogs();
        this.syncGripPressureFromLogs();
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
        if (cmd.toUpperCase() === 'PRESS?') {
            await this.refreshGripPressure(false);
        }
    }

    async refreshGripPressure(triggerDeviceRefresh: boolean = true) {
        try {
            this.gripPressure = triggerDeviceRefresh
                ? await this.robotService.refreshGripPressure()
                : await this.robotService.getGripPressure();
            this.cdr.detectChanges();
        } catch (err) {
            console.error(err);
            await this.refreshLogs();
        }
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

    private syncGripPressureFromLogs() {
        const latestGripLog = [...this.logs].reverse().find((row) =>
            row.dir === 'RX' && (row.message.startsWith('GRIP ') || row.message.startsWith('PRESS '))
        );
        if (!latestGripLog) {
            return;
        }

        const match = latestGripLog.message.match(/P1:(\d+)%\s+P2:(\d+)%\s+MAX:(\d+)/);
        if (!match) {
            return;
        }

        this.gripPressure = {
            p1: Number(match[1]),
            p2: Number(match[2]),
            max: Number(match[3]),
            updatedAt: latestGripLog.ts,
            source: latestGripLog.message.startsWith('GRIP ') ? 'GRIP' : 'PRESS'
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

    private startGripPressurePolling() {
        if (this.gripPressureTimer) {
            clearInterval(this.gripPressureTimer);
        }

        this.gripPressureTimer = setInterval(() => {
            if (this.robotService.connectionStatus() !== 'connected' || this.isExecutingGrip) {
                return;
            }
            void this.refreshGripPressure();
        }, 1200);
    }

    private async trackGripFeedbackUntilSettled() {
        const maxAttempts = 10;
        let previousMax: number | null = null;
        let stableCount = 0;

        for (let attempt = 0; attempt < maxAttempts; attempt++) {
            await this.delay(220);
            const sample = await this.robotService.refreshGripPressure();
            this.gripPressure = sample;
            this.cdr.detectChanges();

            const currentMax = sample.max;
            if (currentMax !== null && previousMax !== null && Math.abs(currentMax - previousMax) <= 1) {
                stableCount += 1;
            } else {
                stableCount = 0;
            }

            previousMax = currentMax;

            if (stableCount >= 2) {
                break;
            }
        }
    }

    private applyJointStateFromPayload(payload: Record<string, unknown>) {
        this.m1 = Number(payload['m1'] ?? this.m1);
        this.m2 = Number(payload['m2'] ?? this.m2);
        this.m3 = Number(payload['m3'] ?? this.m3);
        this.m5 = Number(payload['m5'] ?? this.m5);
        this.m6 = Number(payload['m6'] ?? this.m6);
        this.hasPendingManual = false;
        this.pushViewerAngles();
    }

    private applyHomeToUi(axis: string) {
        if (axis === 'all') {
            this.m1 = 0;
            this.m2 = 0;
            this.m3 = 0;
            this.m5 = 0;
            this.m6 = 0;
        } else if (axis === '1') {
            this.m1 = 0;
        } else if (axis === '2') {
            this.m2 = 0;
        } else if (axis === '3') {
            this.m3 = 0;
        } else if (axis === '5') {
            this.m5 = 0;
        } else if (axis === '6') {
            this.m6 = 0;
        }
        this.hasPendingManual = false;
        this.pushViewerAngles();
    }

    get currentGripCommand(): string {
        return this.gripOpen ? 'GRIP:120' : 'GRIP:0';
    }

    get gripPressurePercent(): number {
        return this.gripPressure.max ?? 0;
    }

    get endstopCards() {
        return [
            { label: 'E1', value: this.endstopState.e1 },
            { label: 'E2', value: this.endstopState.e2 },
            { label: 'E3', value: this.endstopState.e3 },
            { label: 'E4', value: this.endstopState.e4 }
        ];
    }

    get aiConfidencePercent(): number | null {
        const confidence = this.aiResult?.parsed.confidence;
        return typeof confidence === 'number' ? Math.round(confidence * 100) : null;
    }

    private initVoiceRecognition() {
        const recognitionCtor = this.getSpeechRecognitionCtor();
        if (!recognitionCtor) {
            this.voiceSupported = false;
            return;
        }

        const recognition = new recognitionCtor();
        recognition.lang = 'it-IT';
        recognition.continuous = true;
        recognition.interimResults = true;

        recognition.onstart = () => {
            this.isListeningVoice = true;
            this.voiceTranscriptDraft = '';
            this.voiceLiveTranscript = '';
            this.cdr.detectChanges();
        };

        recognition.onresult = (event) => {
            let transcript = '';
            for (let i = event.resultIndex; i < event.results.length; i++) {
                const chunk = event.results[i]?.[0]?.transcript ?? event.results[i]?.item(0)?.transcript ?? '';
                transcript += chunk;
            }

            const cleanedTranscript = transcript.trim();
            this.voiceLiveTranscript = cleanedTranscript;
            this.voiceTranscriptDraft = cleanedTranscript;
            if (cleanedTranscript) {
                this.naturalCommand = cleanedTranscript;
            }
            const latestResult = event.results[event.results.length - 1];
            if (latestResult?.isFinal) {
                void this.handleFinalVoiceTranscript(this.voiceTranscriptDraft);
            }
            this.cdr.detectChanges();
        };

        recognition.onerror = (event) => {
            this.isListeningVoice = false;
            if (event.error && event.error !== 'no-speech' && event.error !== 'aborted') {
                this.aiError = `Errore riconoscimento vocale: ${event.error}`;
            }
            this.cdr.detectChanges();
        };

        recognition.onend = () => {
            this.isListeningVoice = false;
            if (this.voiceAutoRestart) {
                setTimeout(() => {
                    if (this.voiceAutoRestart) {
                        this.speechRecognition?.start();
                    }
                }, 250);
            }
            this.cdr.detectChanges();
        };

        this.speechRecognition = recognition;
        this.voiceSupported = true;
    }

    private stopVoiceRecognition() {
        this.voiceAutoRestart = false;
        if (this.speechRecognition && this.isListeningVoice) {
            this.speechRecognition.stop();
        }
        this.isListeningVoice = false;
        this.voiceLiveTranscript = '';
    }

    private async handleFinalVoiceTranscript(transcript: string) {
        const cleaned = transcript.trim();
        if (!cleaned) {
            return;
        }

        const normalized = this.normalizeVoiceText(cleaned);
        const wakeWord = this.normalizeVoiceText(this.voiceWakeWord);
        const tokens = normalized.split(' ').filter(Boolean);
        const wakeIndex = tokens.findIndex((token) => token === wakeWord);

        // Accept "robot ..." and also short lead-ins like "hey robot ..."
        if (wakeIndex === -1 || wakeIndex > 1) {
            return;
        }

        const command = tokens.slice(wakeIndex + 1).join(' ').trim();

        if (!command) {
            this.aiError = `Wake word rilevata. Pronuncia: "${this.voiceWakeWord} home asse 2".`;
            this.cdr.detectChanges();
            return;
        }

        this.naturalCommand = command;
        this.voiceTranscriptDraft = command;
        this.lastWakeTrigger = cleaned;
        this.showWakeOverlay();
        this.setVoiceStatus(`Wake word rilevata. Comando: ${command}`, 'info');
        this.playVoiceCue('wake');
        await this.interpretAiCommand(true);
    }

    private showWakeOverlay() {
        this.voiceWakeDetected = true;
        if (this.wakeOverlayTimer) {
            clearTimeout(this.wakeOverlayTimer);
        }
        this.wakeOverlayTimer = setTimeout(() => {
            this.voiceWakeDetected = false;
            this.cdr.detectChanges();
        }, 1800);
        this.cdr.detectChanges();
    }

    private setVoiceStatus(message: string, tone: 'info' | 'success' | 'error') {
        this.voiceStatusMessage = message;
        this.voiceStatusTone = tone;
        if (this.voiceStatusTimer) {
            clearTimeout(this.voiceStatusTimer);
        }
        this.voiceStatusTimer = setTimeout(() => {
            this.voiceStatusMessage = '';
            this.cdr.detectChanges();
        }, 3200);
        this.cdr.detectChanges();
    }

    private playVoiceCue(kind: 'wake' | 'execute' | 'error') {
        try {
            const AudioCtx = window.AudioContext
                || (window as Window & { webkitAudioContext?: typeof AudioContext }).webkitAudioContext;
            if (!AudioCtx) {
                return;
            }
            if (!this.audioContext) {
                this.audioContext = new AudioCtx();
            }

            const now = this.audioContext.currentTime;
            const master = this.audioContext.createGain();
            master.gain.value = 0.05;
            master.connect(this.audioContext.destination);

            const notes = kind === 'wake'
                ? [{ freq: 740, duration: 0.08 }, { freq: 920, duration: 0.09 }]
                : kind === 'execute'
                    ? [{ freq: 620, duration: 0.06 }, { freq: 820, duration: 0.06 }, { freq: 1040, duration: 0.08 }]
                    : [{ freq: 340, duration: 0.12 }, { freq: 250, duration: 0.16 }];

            let cursor = now;
            for (const note of notes) {
                const osc = this.audioContext.createOscillator();
                const gain = this.audioContext.createGain();
                osc.type = kind === 'error' ? 'sawtooth' : 'sine';
                osc.frequency.setValueAtTime(note.freq, cursor);
                gain.gain.setValueAtTime(0.0001, cursor);
                gain.gain.exponentialRampToValueAtTime(1, cursor + 0.01);
                gain.gain.exponentialRampToValueAtTime(0.0001, cursor + note.duration);
                osc.connect(gain);
                gain.connect(master);
                osc.start(cursor);
                osc.stop(cursor + note.duration);
                cursor += note.duration + 0.03;
            }
        } catch {
            // Best effort only.
        }
    }

    private shouldAutoExecuteAi(result: AiInterpretResponse | null): boolean {
        const intent = result?.parsed.intent;
        if (!intent || !result?.preview) {
            return false;
        }

        return intent === 'home'
            || intent === 'set_grip'
            || intent === 'move_joint'
            || intent === 'move_joint_delta'
            || intent === 'run_sequence';
    }

    private normalizeVoiceText(value: string): string {
        return value
            .toLowerCase()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .replace(/[^\p{L}\p{N}\s]/gu, ' ')
            .replace(/\s+/g, ' ')
            .trim();
    }

    private getSpeechRecognitionCtor(): SpeechRecognitionCtor | null {
        const target = window as Window & {
            SpeechRecognition?: SpeechRecognitionCtor;
            webkitSpeechRecognition?: SpeechRecognitionCtor;
        };

        return target.SpeechRecognition ?? target.webkitSpeechRecognition ?? null;
    }
}
