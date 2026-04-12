import { PositionSet } from './storage';

export type AiCommand =
    | {
        intent: 'home';
        axis: 'all' | '1' | '2' | '3' | '4' | '5' | '6';
        confidence?: number;
        explanation?: string;
    }
    | {
        intent: 'move_cartesian';
        x: number;
        y: number;
        z: number;
        confidence?: number;
        explanation?: string;
    }
    | {
        intent: 'move_joint';
        m1?: number;
        m2?: number;
        m3?: number;
        m5?: number;
        m6?: number;
        grip?: string;
        c?: string;
        confidence?: number;
        explanation?: string;
    }
    | {
        intent: 'move_joint_delta';
        m1?: number;
        m2?: number;
        m3?: number;
        m5?: number;
        m6?: number;
        confidence?: number;
        explanation?: string;
    }
    | {
        intent: 'run_sequence';
        sequenceId?: number;
        sequenceName?: string;
        confidence?: number;
        explanation?: string;
    }
    | {
        intent: 'set_grip';
        grip: string;
        confidence?: number;
        explanation?: string;
    }
    | {
        intent: 'unknown';
        confidence?: number;
        explanation?: string;
        missing?: string[];
    };

export interface AiInterpretContext {
    input: string;
    sequences: PositionSet[];
    currentJoints?: {
        m1: number;
        m2: number;
        m3: number;
        m5: number;
        m6: number;
    } | null;
}

export interface AiProvider {
    health(): Promise<{ ok: boolean; baseUrl: string; model: string }>;
    interpret(context: AiInterpretContext): Promise<AiCommand>;
}

export function interpretFastPath(context: AiInterpretContext): AiCommand | null {
    const original = context.input.trim();
    if (!original) {
        return null;
    }

    const normalized = normalizeText(original);

    const homeAll = normalized.match(/\b(?:home|homing)\b(?:\s+(?:tutto|tutti|all))?$/);
    if (homeAll) {
        return {
            intent: 'home',
            axis: 'all',
            confidence: 0.99,
            explanation: 'Recognized direct homing command.'
        };
    }

    const homeAxis = normalized.match(/\b(?:home|homing)\b(?:\s+(?:asse|axis|giunto|joint|motore|motor))?\s*([1-6])\b/);
    if (homeAxis) {
        return {
            intent: 'home',
            axis: homeAxis[1] as '1' | '2' | '3' | '4' | '5' | '6',
            confidence: 0.99,
            explanation: 'Recognized direct homing command for a specific axis.'
        };
    }

    if (/\b(?:apri|open)\b.*\b(?:pinza|gripper|grip)\b|\b(?:pinza|gripper|grip)\b.*\b(?:apri|open)\b/.test(normalized)) {
        return {
            intent: 'set_grip',
            grip: 'GRIP:0',
            confidence: 0.99,
            explanation: 'Recognized direct open gripper command.'
        };
    }

    if (/\b(?:chiudi|close|serra)\b.*\b(?:pinza|gripper|grip)\b|\b(?:pinza|gripper|grip)\b.*\b(?:chiudi|close|serra)\b/.test(normalized)) {
        return {
            intent: 'set_grip',
            grip: 'GRIP:120',
            confidence: 0.99,
            explanation: 'Recognized direct close gripper command.'
        };
    }

    const moveCartesian = parseCartesianCommand(normalized);
    if (moveCartesian) {
        return {
            intent: 'move_cartesian',
            ...moveCartesian,
            confidence: 0.98,
            explanation: 'Recognized explicit cartesian coordinates.'
        };
    }

    const relativeJointMove = parseRelativeJointMove(normalized);
    if (relativeJointMove) {
        return {
            intent: 'move_joint_delta',
            ...relativeJointMove,
            confidence: 0.98,
            explanation: 'Recognized relative joint movement.'
        };
    }

    const absoluteJointMove = parseAbsoluteJointMove(normalized);
    if (absoluteJointMove) {
        return {
            intent: 'move_joint',
            ...absoluteJointMove,
            confidence: 0.98,
            explanation: 'Recognized absolute joint target.'
        };
    }

    const sequence = findSequenceReference(normalized, context.sequences);
    if (sequence) {
        return {
            intent: 'run_sequence',
            sequenceId: sequence.id,
            sequenceName: sequence.name,
            confidence: 0.98,
            explanation: 'Recognized direct sequence execution command.'
        };
    }

    return null;
}

export class OllamaAiProvider implements AiProvider {
    constructor(
        private readonly baseUrl: string = process.env.OLLAMA_BASE_URL?.trim() || 'http://127.0.0.1:11434',
        private readonly model: string = process.env.OLLAMA_MODEL?.trim() || 'qwen2.5:7b'
    ) { }

    async health(): Promise<{ ok: boolean; baseUrl: string; model: string }> {
        const response = await fetch(`${this.baseUrl}/api/tags`);
        if (!response.ok) {
            throw new Error(`Ollama health check failed with status ${response.status}`);
        }

        return { ok: true, baseUrl: this.baseUrl, model: this.model };
    }

    async interpret(context: AiInterpretContext): Promise<AiCommand> {
        const sequenceSummary = context.sequences.map((set) => ({
            id: set.id,
            name: set.name
        }));

        const response = await fetch(`${this.baseUrl}/api/chat`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                model: this.model,
                stream: false,
                format: 'json',
                messages: [
                    {
                        role: 'system',
                        content: [
                            'Sei un interprete di comandi per un braccio robotico.',
                            'Rispondi sempre e solo con un singolo oggetto JSON valido.', 
                            'Non inventare valori mancanti.',
                            'Se il comando non e chiaro usa intent="unknown".',
                            'Intent supportati: home, move_cartesian, move_joint, move_joint_delta, run_sequence, set_grip, unknown.',
                            'Per home usa axis: all oppure 1..6.',
                            'Per move_cartesian servono x, y, z numerici.',
                            'Per move_joint usa uno o piu assi assoluti tra m1, m2, m3, m5, m6.',
                            'Per move_joint_delta usa uno o piu delta tra m1, m2, m3, m5, m6.',
                            'Per set_grip usa grip nel formato GRIP:<numero intero>.',
                            'Per run_sequence usa sequenceId se identificabile, altrimenti sequenceName.',
                            'Aggiungi confidence tra 0 e 1 ed explanation breve.'
                        ].join(' ')
                    },
                    {
                        role: 'user',
                        content: JSON.stringify({
                            input: context.input,
                            knownSequences: sequenceSummary
                        })
                    }
                ]
            })
        });

        if (!response.ok) {
            throw new Error(`Ollama chat failed with status ${response.status}`);
        }

        const payload = await response.json() as {
            message?: { content?: string };
        };

        const content = payload.message?.content?.trim();
        if (!content) {
            throw new Error('Ollama returned an empty response');
        }

        let parsed: unknown;
        try {
            parsed = JSON.parse(content);
        } catch {
            throw new Error('Ollama returned invalid JSON');
        }

        return normalizeAiCommand(parsed);
    }
}

export function normalizeAiCommand(input: unknown): AiCommand {
    const data = asRecord(input);
    const intent = asString(data.intent);

    switch (intent) {
        case 'home': {
            const axis = asString(data.axis);
            if (axis === 'all' || ['1', '2', '3', '4', '5', '6'].includes(axis)) {
                return {
                    intent,
                    axis: axis as 'all' | '1' | '2' | '3' | '4' | '5' | '6',
                    confidence: asNumberOrUndefined(data.confidence),
                    explanation: asOptionalString(data.explanation)
                };
            }
            break;
        }
        case 'move_cartesian': {
            const x = asFiniteNumber(data.x);
            const y = asFiniteNumber(data.y);
            const z = asFiniteNumber(data.z);
            if (x !== null && y !== null && z !== null) {
                return {
                    intent,
                    x,
                    y,
                    z,
                    confidence: asNumberOrUndefined(data.confidence),
                    explanation: asOptionalString(data.explanation)
                };
            }
            break;
        }
        case 'move_joint': {
            const m1 = asFiniteNumberOrUndefined(data.m1);
            const m2 = asFiniteNumberOrUndefined(data.m2);
            const m3 = asFiniteNumberOrUndefined(data.m3);
            const m5 = asFiniteNumberOrUndefined(data.m5);
            const m6 = asFiniteNumberOrUndefined(data.m6);
            if ([m1, m2, m3, m5, m6].some((value) => value !== undefined)) {
                return {
                    intent,
                    m1,
                    m2,
                    m3,
                    m5,
                    m6,
                    grip: normalizeGripOrUndefined(data.grip),
                    c: normalizeMagnetOrUndefined(data.c),
                    confidence: asNumberOrUndefined(data.confidence),
                    explanation: asOptionalString(data.explanation)
                };
            }
            break;
        }
        case 'move_joint_delta': {
            const m1 = asFiniteNumberOrUndefined(data.m1);
            const m2 = asFiniteNumberOrUndefined(data.m2);
            const m3 = asFiniteNumberOrUndefined(data.m3);
            const m5 = asFiniteNumberOrUndefined(data.m5);
            const m6 = asFiniteNumberOrUndefined(data.m6);
            if ([m1, m2, m3, m5, m6].some((value) => value !== undefined)) {
                return {
                    intent,
                    m1,
                    m2,
                    m3,
                    m5,
                    m6,
                    confidence: asNumberOrUndefined(data.confidence),
                    explanation: asOptionalString(data.explanation)
                };
            }
            break;
        }
        case 'run_sequence': {
            const sequenceId = asFiniteNumberOrUndefined(data.sequenceId);
            const sequenceName = asOptionalString(data.sequenceName);
            if (sequenceId !== undefined || sequenceName) {
                return {
                    intent,
                    sequenceId,
                    sequenceName,
                    confidence: asNumberOrUndefined(data.confidence),
                    explanation: asOptionalString(data.explanation)
                };
            }
            break;
        }
        case 'set_grip': {
            const grip = normalizeGripOrUndefined(data.grip);
            if (grip) {
                return {
                    intent,
                    grip,
                    confidence: asNumberOrUndefined(data.confidence),
                    explanation: asOptionalString(data.explanation)
                };
            }
            break;
        }
    }

    return {
        intent: 'unknown',
        confidence: asNumberOrUndefined(data.confidence),
        explanation: asOptionalString(data.explanation) ?? 'Unable to map request to a safe robot command.',
        missing: asStringArray(data.missing)
    };
}

function normalizeText(value: string): string {
    return value
        .toLowerCase()
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '')
        .replace(/\s+/g, ' ')
        .trim();
}

function parseCartesianCommand(input: string): { x: number; y: number; z: number } | null {
    const x = extractCoordinate(input, 'x');
    const y = extractCoordinate(input, 'y');
    const z = extractCoordinate(input, 'z');

    if (x === null || y === null || z === null) {
        return null;
    }

    return { x, y, z };
}

function parseRelativeJointMove(input: string): Partial<Record<'m1' | 'm2' | 'm3' | 'm5' | 'm6', number>> | null {
    const match = input.match(/\b(?:muovi|sposta|ruota)\s+(?:il\s+)?(?:motore\s+|asse\s+)?(m?[1-6])\s+(?:di|del(?:la)?)\s*(-?\d+(?:[\.,]\d+)?)\s*gradi?\b/);
    if (!match) {
        return null;
    }

    const axis = normalizeJointAxis(match[1]);
    if (!axis) {
        return null;
    }

    return { [axis]: Number(match[2].replace(',', '.')) };
}

function parseAbsoluteJointMove(input: string): Partial<Record<'m1' | 'm2' | 'm3' | 'm5' | 'm6', number>> | null {
    const match = input.match(/\b(?:imposta|porta|metti|ruota|muovi)\s+(?:il\s+)?(?:motore\s+|asse\s+)?(m?[1-6])\s+(?:a|su)\s*(-?\d+(?:[\.,]\d+)?)\s*gradi?\b/);
    if (!match) {
        return null;
    }

    const axis = normalizeJointAxis(match[1]);
    if (!axis) {
        return null;
    }

    return { [axis]: Number(match[2].replace(',', '.')) };
}

function extractCoordinate(input: string, axis: 'x' | 'y' | 'z'): number | null {
    const match = input.match(new RegExp(`\\b${axis}\\s*[:=]?\\s*(-?\\d+(?:[\\.,]\\d+)?)\\b`));
    if (!match) {
        return null;
    }

    return Number(match[1].replace(',', '.'));
}

function findSequenceReference(input: string, sequences: PositionSet[]): PositionSet | null {
    if (!/\b(?:sequenza|sequence|esegui|run|avvia|play)\b/.test(input)) {
        return null;
    }

    const byExactName = sequences.find((item) => input.includes(normalizeText(item.name)));
    if (byExactName) {
        return byExactName;
    }

    const idMatch = input.match(/\b(?:sequenza|sequence)\s*(\d+)\b/);
    if (!idMatch) {
        return null;
    }

    const id = Number(idMatch[1]);
    return sequences.find((item) => item.id === id) ?? null;
}

function normalizeJointAxis(value: string): 'm1' | 'm2' | 'm3' | 'm5' | 'm6' | null {
    const cleaned = value.startsWith('m') ? value : `m${value}`;
    if (cleaned === 'm1' || cleaned === 'm2' || cleaned === 'm3' || cleaned === 'm5' || cleaned === 'm6') {
        return cleaned;
    }
    return null;
}

function asRecord(value: unknown): Record<string, unknown> {
    return value && typeof value === 'object' && !Array.isArray(value)
        ? value as Record<string, unknown>
        : {};
}

function asString(value: unknown): string {
    return typeof value === 'string' ? value.trim() : '';
}

function asOptionalString(value: unknown): string | undefined {
    const text = asString(value);
    return text || undefined;
}

function asFiniteNumber(value: unknown): number | null {
    if (typeof value === 'number' && Number.isFinite(value)) {
        return value;
    }
    if (typeof value === 'string' && value.trim()) {
        const parsed = Number(value);
        if (Number.isFinite(parsed)) {
            return parsed;
        }
    }
    return null;
}

function asFiniteNumberOrUndefined(value: unknown): number | undefined {
    const parsed = asFiniteNumber(value);
    return parsed === null ? undefined : parsed;
}

function asNumberOrUndefined(value: unknown): number | undefined {
    const parsed = asFiniteNumber(value);
    if (parsed === null) {
        return undefined;
    }
    return Math.max(0, Math.min(1, parsed));
}

function asStringArray(value: unknown): string[] | undefined {
    if (!Array.isArray(value)) {
        return undefined;
    }

    const items = value
        .map((item) => asString(item))
        .filter(Boolean);

    return items.length ? items : undefined;
}

function normalizeGripOrUndefined(value: unknown): string | undefined {
    const text = asString(value).toUpperCase();
    if (/^GRIP:\d+$/.test(text)) {
        return text;
    }
    return undefined;
}

function normalizeMagnetOrUndefined(value: unknown): string | undefined {
    const text = asString(value).toUpperCase();
    if (text === 'C:0' || text === 'C:1') {
        return text;
    }
    return undefined;
}
