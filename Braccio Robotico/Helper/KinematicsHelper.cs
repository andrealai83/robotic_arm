using System;

namespace Braccio_Robotico.Helper
{
    /// <summary>
    /// Helper class for robot arm kinematics calculations.
    /// Handles forward and inverse kinematics for a 4-DOF robotic arm.
    /// </summary>
    public static class KinematicsHelper
    {
        // Lunghezze dei segmenti del braccio (in cm)
        private const double L1 = 20.0; // Lunghezza segmento 1 (dalla base al primo gomito)
        private const double L2 = 20.0; // Lunghezza segmento 2 (dal primo al secondo gomito)
        private const double L3 = 20.0; // Lunghezza segmento 3 (dal secondo gomito all'end-effector)

        /// <summary>
        /// Rappresenta una posizione cartesiana nello spazio 3D
        /// </summary>
        public class CartesianPosition
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }

            public CartesianPosition(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public override string ToString() => $"(X:{X:F2}, Y:{Y:F2}, Z:{Z:F2})";
        }

        /// <summary>
        /// Calcola la posizione cartesiana dell'end-effector date le angolazioni dei giunti.
        /// Forward Kinematics per un braccio a 4 DOF.
        /// </summary>
        /// <param name="m1">Angolo base (gradi) - rotazione sul piano XY</param>
        /// <param name="m2">Angolo primo giunto (gradi) - rotazione verticale</param>
        /// <param name="m3">Angolo secondo giunto (gradi) - rotazione verticale</param>
        /// <param name="m4">Angolo terzo giunto (gradi) - rotazione verticale</param>
        /// <returns>Posizione cartesiana dell'end-effector</returns>
        public static CartesianPosition ForwardKinematics(double m1, double m2, double m3, double m4)
        {
            // Converti gradi in radianti
            double theta1 = DegToRad(m1); // Base rotation
            double theta2 = DegToRad(m2); // First joint
            double theta3 = DegToRad(m3); // Second joint
            double theta4 = DegToRad(m4); // Third joint

            // Calcola la posizione nel piano verticale (ignorando per ora la rotazione della base)
            // Assumiamo che gli angoli siano misurati rispetto all'orizzontale
            double r = L1 * Math.Cos(theta2) + 
                      L2 * Math.Cos(theta2 + theta3) + 
                      L3 * Math.Cos(theta2 + theta3 + theta4);
            
            double z = L1 * Math.Sin(theta2) + 
                      L2 * Math.Sin(theta2 + theta3) + 
                      L3 * Math.Sin(theta2 + theta3 + theta4);

            // Proietta sul piano XY usando la rotazione della base
            double x = r * Math.Cos(theta1);
            double y = r * Math.Sin(theta1);

            return new CartesianPosition(x, y, z);
        }

        /// <summary>
        /// Calcola gli angoli dei giunti necessari per raggiungere una posizione cartesiana target.
        /// Inverse Kinematics per un braccio a 4 DOF.
        /// NOTA: Questo è un approccio semplificato che mantiene l'orientamento dell'end-effector costante.
        /// </summary>
        /// <param name="target">Posizione cartesiana target</param>
        /// <param name="currentAngles">Angoli correnti (per disambiguare soluzioni multiple)</param>
        /// <returns>Movimento con gli angoli calcolati, o null se irraggiungibile</returns>
        public static Movimento InverseKinematics(CartesianPosition target, Movimento currentAngles = null)
        {
            // 1. Calcola l'angolo della base (M1) - rotazione sul piano XY
            double m1 = RadToDeg(Math.Atan2(target.Y, target.X));

            // 2. Calcola la distanza radiale sul piano XY
            double r = Math.Sqrt(target.X * target.X + target.Y * target.Y);

            // 3. Calcola la distanza totale dall'origine della base al target
            double d = Math.Sqrt(r * r + target.Z * target.Z);

            // 4. Verifica se il target è raggiungibile
            double maxReach = L1 + L2 + L3;
            double minReach = Math.Abs(L1 - L2 - L3);

            if (d > maxReach || d < minReach)
            {
                // Target irraggiungibile
                return null;
            }

            // 5. Per semplicità, usiamo una configurazione "elbow-up" con l'ultimo giunto (M4) 
            //    che mantiene l'end-effector orizzontale
            // Questo è un approccio semplificato - una soluzione completa richiederebbe
            // la specifica dell'orientamento desiderato

            // Angolo dal piano orizzontale al target
            double alpha = Math.Atan2(target.Z, r);

            // Risolvi il problema 2D usando la legge dei coseni
            // per i primi due segmenti
            //double L_combined = L1 + L2; // Semplificazione: trattiamo i primi due segmenti
            
            // Calcola M2 usando la geometria
            double cosM3 = (d * d - L1 * L1 - L2 * L2) / (2 * L1 * L2);
            cosM3 = Math.Max(-1, Math.Min(1, cosM3)); // Clamp per evitare errori numerici
            
            double m3_rad = Math.Acos(cosM3);
            
            double beta = Math.Atan2(L2 * Math.Sin(m3_rad), L1 + L2 * Math.Cos(m3_rad));
            double m2_rad = alpha - beta;

            double m2 = RadToDeg(m2_rad);
            double m3 = RadToDeg(m3_rad);

            // M4 viene calcolato per mantenere l'end-effector in una certa orientazione
            // Per ora, assumiamo che vogliamo mantenere l'end-effector orizzontale
            double m4 = -(m2 + m3); // Compensa per mantenere orizzontale

            // Se abbiamo gli angoli correnti, usiamo la soluzione più vicina
            // (per evitare salti improvvisi da una configurazione all'altra)
            if (currentAngles != null)
            {
                // Possiamo provare l'altra soluzione (elbow-down) e scegliere la più vicina
                // Per ora usiamo solo la soluzione "elbow-up"
            }

            return new Movimento
            {
                M1 = m1,
                M2 = m2,
                M3 = m3,
                M4 = m4,
                GRIP = currentAngles?.GRIP ?? "GRIP:0"
            };
        }

        /// <summary>
        /// Genera una traiettoria lineare nello spazio cartesiano tra due posizioni.
        /// </summary>
        /// <param name="from">Movimento di partenza</param>
        /// <param name="to">Movimento di arrivo</param>
        /// <param name="numSteps">Numero di passi intermedi (default: 5)</param>
        /// <returns>Lista di movimenti che formano una traiettoria lineare</returns>
        public static System.Collections.Generic.List<Movimento> GenerateCartesianTrajectory(
            Movimento from, Movimento to, int numSteps = 5)
        {
            var trajectory = new System.Collections.Generic.List<Movimento>();

            // Calcola posizioni cartesiane di partenza e arrivo
            var startPos = ForwardKinematics(from.M1, from.M2, from.M3, from.M4);
            var endPos = ForwardKinematics(to.M1, to.M2, to.M3, to.M4);

            // Aggiungi il punto di partenza
            trajectory.Add(from);

            // Genera punti intermedi lungo una linea retta nello spazio cartesiano
            for (int i = 1; i < numSteps; i++)
            {
                double t = (double)i / numSteps;

                var intermediatePos = new CartesianPosition(
                    Lerp(startPos.X, endPos.X, t),
                    Lerp(startPos.Y, endPos.Y, t),
                    Lerp(startPos.Z, endPos.Z, t)
                );

                // Usa cinematica inversa per trovare gli angoli
                var intermediateMov = InverseKinematics(intermediatePos, trajectory[trajectory.Count - 1]);
                
                if (intermediateMov != null)
                {
                    intermediateMov.GRIP = from.GRIP; // Mantieni lo stato della calamita
                    trajectory.Add(intermediateMov);
                }
                else
                {
                    // Se un punto intermedio è irraggiungibile, usa interpolazione lineare negli angoli
                    trajectory.Add(new Movimento
                    {
                        M1 = Lerp(from.M1, to.M1, t),
                        M2 = Lerp(from.M2, to.M2, t),
                        M3 = Lerp(from.M3, to.M3, t),
                        M4 = Lerp(from.M4, to.M4, t),
                        GRIP = from.GRIP
                    });
                }
            }

            // Aggiungi il punto finale
            trajectory.Add(to);

            return trajectory;
        }

        // Utility methods
        private static double DegToRad(double degrees) => degrees * Math.PI / 180.0;
        private static double RadToDeg(double radians) => radians * 180.0 / Math.PI;
        private static double Lerp(double a, double b, double t) => a + (b - a) * t;
    }
}
