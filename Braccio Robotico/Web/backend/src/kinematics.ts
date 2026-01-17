export class CartesianPosition {
    constructor(public x: number, public y: number, public z: number) { }
    toString() { return `(X:${this.x.toFixed(2)}, Y:${this.y.toFixed(2)}, Z:${this.z.toFixed(2)})`; }
}

export interface Movimento {
    m1: number;
    m2: number;
    m3: number;
    m4: number;
    c: string;
}

export class KinematicsHelper {
    // Lunghezze dei segmenti del braccio (in cm)
    private static readonly L1 = 20.0;
    private static readonly L2 = 20.0;
    private static readonly L3 = 20.0;

    public static ForwardKinematics(m1: number, m2: number, m3: number, m4: number): CartesianPosition {
        const theta1 = this.DegToRad(m1);
        const theta2 = this.DegToRad(m2);
        const theta3 = this.DegToRad(m3);
        const theta4 = this.DegToRad(m4);

        const r = this.L1 * Math.cos(theta2) +
            this.L2 * Math.cos(theta2 + theta3) +
            this.L3 * Math.cos(theta2 + theta3 + theta4);

        const z = this.L1 * Math.sin(theta2) +
            this.L2 * Math.sin(theta2 + theta3) +
            this.L3 * Math.sin(theta2 + theta3 + theta4);

        const x = r * Math.cos(theta1);
        const y = r * Math.sin(theta1);

        return new CartesianPosition(x, y, z);
    }

    public static InverseKinematics(target: CartesianPosition, currentAngles: Movimento | null = null): Movimento | null {
        // 1. Calcola l'angolo della base (M1)
        const m1 = this.RadToDeg(Math.atan2(target.y, target.x));

        // 2. Distanza radiale sul piano XY
        const r = Math.sqrt(target.x * target.x + target.y * target.y);

        // 3. Distanza totale
        const d = Math.sqrt(r * r + target.z * target.z);

        // 4. Verifica raggiungibilità
        const maxReach = this.L1 + this.L2 + this.L3;
        const minReach = Math.abs(this.L1 - this.L2 - this.L3);

        if (d > maxReach || d < minReach) {
            return null;
        }

        // 5. Configurazione Elbow-Up (semplificata)
        const alpha = Math.atan2(target.z, r);

        // Geometria per i primi due segmenti (semplificazione L1+L2 vs L3?)
        // Wait, the C# code had a simplified approach treating L1+L2 as one segment? 
        // No, it was solving for M2 and M3.
        // Let's re-read C# logic carefully.
        // C# code: double cosM3 = (d * d - L1 * L1 - L2 * L2) / (2 * L1 * L2);
        // This seems to assume L3 is part of the end effector or something?
        // Actually the C# code says: "Semplificazione: trattiamo i primi due segmenti".
        // It seems it calculates M3 angle based on L1 and L2 to reach distance d?
        // But d includes L3?
        // Let's copy the C# logic EXACTLY to preserve behavior, even if physics seems simplified.

        let cosM3 = (d * d - this.L1 * this.L1 - this.L2 * this.L2) / (2 * this.L1 * this.L2);
        cosM3 = Math.max(-1, Math.min(1, cosM3));

        const m3_rad = Math.acos(cosM3);

        const beta = Math.atan2(this.L2 * Math.sin(m3_rad), this.L1 + this.L2 * Math.cos(m3_rad));
        const m2_rad = alpha - beta;

        const m2 = this.RadToDeg(m2_rad);
        const m3 = this.RadToDeg(m3_rad);
        const m4 = -(m2 + m3);

        return {
            m1: m1,
            m2: m2,
            m3: m3,
            m4: m4,
            c: currentAngles ? currentAngles.c : "C:0"
        };
    }

    private static DegToRad(degrees: number): number { return degrees * Math.PI / 180.0; }
    private static RadToDeg(radians: number): number { return radians * 180.0 / Math.PI; }

    // Lerp helper if needed, though mostly used in Trajectory
    public static Lerp(a: number, b: number, t: number): number { return a + (b - a) * t; }
}
