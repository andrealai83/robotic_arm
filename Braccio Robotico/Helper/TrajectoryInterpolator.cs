using System;
using System.Collections.Generic;
using System.Linq;

namespace Braccio_Robotico.Helper
{
    /// <summary>
    /// Classe helper per l'interpolazione di traiettorie nel movimento del braccio robotico.
    /// Genera punti intermedi tra posizioni consecutive per creare movimenti fluidi.
    /// </summary>
    public static class TrajectoryInterpolator
    {
        /// <summary>
        /// Interpola una lista di movimenti generando punti intermedi tra ogni coppia consecutiva.
        /// </summary>
        /// <param name="movements">Lista di movimenti da interpolare</param>
        /// <param name="stepsPerTransition">Numero di passi intermedi tra ogni coppia di movimenti (default: 10)</param>
        /// <returns>Lista espansa con movimenti interpolati</returns>
        public static List<Movimento> InterpolateMovements(List<Movimento> movements, int stepsPerTransition = 10)
        {
            if (movements == null || movements.Count == 0)
                return new List<Movimento>();

            if (movements.Count == 1)
                return movements.ToList();

            var interpolated = new List<Movimento>();

            for (int i = 0; i < movements.Count; i++)
            {
                // Aggiungi il movimento corrente
                interpolated.Add(movements[i]);

                // Se non è l'ultimo movimento, genera i punti intermedi verso il prossimo
                if (i < movements.Count - 1)
                {
                    var from = movements[i];
                    var to = movements[i + 1];

                    // Genera punti intermedi (escludendo il punto finale che sarà aggiunto alla prossima iterazione)
                    for (int step = 1; step <= stepsPerTransition; step++)
                    {
                        double t = (double)step / (stepsPerTransition + 1);
                        interpolated.Add(InterpolateTwo(from, to, t));
                    }
                }
            }

            return interpolated;
        }

        /// <summary>
        /// Interpola linearmente tra due movimenti usando un parametro t.
        /// </summary>
        /// <param name="from">Movimento di partenza</param>
        /// <param name="to">Movimento di arrivo</param>
        /// <param name="t">Parametro di interpolazione (0 = from, 1 = to)</param>
        /// <returns>Movimento interpolato</returns>
        public static Movimento InterpolateTwo(Movimento from, Movimento to, double t)
        {
            // Clamp t tra 0 e 1
            t = Math.Max(0, Math.Min(1, t));

            return new Movimento
            {
                M1 = Lerp(from.M1, to.M1, t),
                M2 = Lerp(from.M2, to.M2, t),
                M3 = Lerp(from.M3, to.M3, t),
                M4 = Lerp(from.M4, to.M4, t),
                // La calamita cambia stato solo alla fine della transizione
                GRIP =  to.GRIP
            };
        }

        /// <summary>
        /// Interpolazione lineare tra due valori.
        /// </summary>
        private static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        /// <summary>
        /// Calcola dinamicamente il numero di passi di interpolazione in base al movimento massimo.
        /// Utile per adattare la fluidità al movimento richiesto.
        /// </summary>
        /// <param name="from">Movimento di partenza</param>
        /// <param name="to">Movimento di arrivo</param>
        /// <param name="degreesPerStep">Numero di gradi per passo (default: 5)</param>
        /// <returns>Numero di passi consigliati</returns>
        public static int CalculateSteps(Movimento from, Movimento to, double degreesPerStep = 5.0)
        {
            double maxDelta = Math.Max(
                Math.Max(Math.Abs(to.M1 - from.M1), Math.Abs(to.M2 - from.M2)),
                Math.Max(Math.Abs(to.M3 - from.M3), Math.Abs(to.M4 - from.M4))
            );

            int steps = (int)Math.Ceiling(maxDelta / degreesPerStep);
            return Math.Max(1, steps); // Almeno 1 passo
        }
    }
}
