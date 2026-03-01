import { Movimento, KinematicsHelper } from './kinematics';

export class TrajectoryInterpolator {
    private static resolveM4(m: Movimento): number {
        return Number.isFinite(m.m4) ? (m.m4 as number) : -m.m2;
    }

    public static InterpolateMovements(movements: Movimento[], stepsPerTransition: number = 10): Movimento[] {
        if (!movements || movements.length === 0) return [];
        if (movements.length === 1) return [...movements];

        const interpolated: Movimento[] = [];

        for (let i = 0; i < movements.length; i++) {
            interpolated.push(movements[i]);

            if (i < movements.length - 1) {
                const from = movements[i];
                const to = movements[i + 1];

                for (let step = 1; step <= stepsPerTransition; step++) {
                    const t = step / (stepsPerTransition + 1);
                    interpolated.push(this.InterpolateTwo(from, to, t));
                }
            }
        }

        return interpolated;
    }

    public static InterpolateTwo(from: Movimento, to: Movimento, t: number): Movimento {
        t = Math.max(0, Math.min(1, t));

        return {
            m1: KinematicsHelper.Lerp(from.m1, to.m1, t),
            m2: KinematicsHelper.Lerp(from.m2, to.m2, t),
            m3: KinematicsHelper.Lerp(from.m3, to.m3, t),
            m4: KinematicsHelper.Lerp(this.resolveM4(from), this.resolveM4(to), t),
            c: t < 1.0 ? from.c : to.c
        };
    }

    public static CalculateSteps(from: Movimento, to: Movimento, degreesPerStep: number = 5.0): number {
        const maxDelta = Math.max(
            Math.max(Math.abs(to.m1 - from.m1), Math.abs(to.m2 - from.m2)),
            Math.max(Math.abs(to.m3 - from.m3), Math.abs(this.resolveM4(to) - this.resolveM4(from)))
        );

        const steps = Math.ceil(maxDelta / degreesPerStep);
        return Math.max(1, steps);
    }
}
