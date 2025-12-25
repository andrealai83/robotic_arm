using System;
using System.Collections.Generic;
using System.Linq;

namespace Braccio_Robotico.Helper
{
    /// <summary>
    /// Ottimizza le traiettorie del braccio robotico per minimizzare lo sforzo sui giunti.
    /// </summary>
    public static class TrajectoryOptimizer
    {
        /// <summary>
        /// Analizza una sequenza di movimenti e suggerisce un ordine ottimale per minimizzare
        /// il movimento totale dei giunti.
        /// Questo è utile quando hai più posizioni salvate e vuoi eseguirle nel modo più efficiente.
        /// </summary>
        /// <param name="movements">Lista di movimenti da ottimizzare</param>
        /// <param name="startPosition">Posizione di partenza (opzionale, default: primo movimento)</param>
        /// <returns>Lista ottimizzata di movimenti</returns>
        public static List<Movimento> OptimizeSequence(List<Movimento> movements, Movimento startPosition = null)
        {
            if (movements == null || movements.Count <= 1)
                return movements?.ToList() ?? new List<Movimento>();

            var optimized = new List<Movimento>();
            var remaining = new List<Movimento>(movements);
            
            // Inizia dalla posizione corrente o dal primo movimento
            Movimento current = startPosition ?? remaining[0];
            
            if (startPosition == null)
            {
                optimized.Add(current);
                remaining.RemoveAt(0);
            }

            // Greedy algorithm: scegli sempre il prossimo movimento più vicino
            while (remaining.Count > 0)
            {
                int nearestIndex = FindNearestMovement(current, remaining);
                Movimento nearest = remaining[nearestIndex];
                
                optimized.Add(nearest);
                remaining.RemoveAt(nearestIndex);
                current = nearest;
            }

            return optimized;
        }

        /// <summary>
        /// Trova il movimento più vicino a quello corrente in termini di sforzo totale dei giunti.
        /// </summary>
        private static int FindNearestMovement(Movimento current, List<Movimento> candidates)
        {
            int nearestIndex = 0;
            double minEffort = double.MaxValue;

            for (int i = 0; i < candidates.Count; i++)
            {
                double effort = CalculateMovementEffort(current, candidates[i]);
                if (effort < minEffort)
                {
                    minEffort = effort;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        /// <summary>
        /// Calcola lo "sforzo" necessario per muoversi da una posizione all'altra.
        /// Lo sforzo è una combinazione della distanza angolare totale e dei pesi dei giunti.
        /// I giunti più vicini alla base hanno peso maggiore perché devono muovere più massa.
        /// </summary>
        /// <param name="from">Posizione di partenza</param>
        /// <param name="to">Posizione di arrivo</param>
        /// <returns>Valore di sforzo (più basso = meno sforzo)</returns>
        public static double CalculateMovementEffort(Movimento from, Movimento to)
        {
            // Pesi per ogni giunto (i giunti alla base hanno peso maggiore)
            const double weight_M1 = 2.5; // Base - muove tutto il braccio
            const double weight_M2 = 2.0; // Primo giunto - muove 3 segmenti
            const double weight_M3 = 1.5; // Secondo giunto - muove 2 segmenti
            const double weight_M4 = 1.0; // Terzo giunto - muove solo l'end-effector

            // Calcola le differenze angolari
            double delta_M1 = Math.Abs(to.M1 - from.M1);
            double delta_M2 = Math.Abs(to.M2 - from.M2);
            double delta_M3 = Math.Abs(to.M3 - from.M3);
            double delta_M4 = Math.Abs(to.M4 - from.M4);

            // Gestisci l'angolo della base che può "avvolgersi" (es. 350° a 10° è solo 20°, non 340°)
            delta_M1 = Math.Min(delta_M1, 360 - delta_M1);

            // Calcola lo sforzo pesato
            double effort = 
                delta_M1 * weight_M1 +
                delta_M2 * weight_M2 +
                delta_M3 * weight_M3 +
                delta_M4 * weight_M4;

            return effort;
        }

        /// <summary>
        /// Verifica se una traiettoria diretta tra due posizioni potrebbe causare problemi
        /// (es. movimenti troppo ampi, rischio di collisione, ecc.)
        /// </summary>
        /// <param name="from">Posizione di partenza</param>
        /// <param name="to">Posizione di arrivo</param>
        /// <returns>True se la traiettoria è considerata sicura</returns>
        public static bool IsSafeTrajectory(Movimento from, Movimento to)
        {
            // Soglia per movimenti "grandi" che potrebbero richiedere attenzione
            const double largeMovementThreshold = 90.0; // gradi

            double maxDelta = Math.Max(
                Math.Max(Math.Abs(to.M1 - from.M1), Math.Abs(to.M2 - from.M2)),
                Math.Max(Math.Abs(to.M3 - from.M3), Math.Abs(to.M4 - from.M4))
            );

            return maxDelta < largeMovementThreshold;
        }

        /// <summary>
        /// Calcola statistiche sulla sequenza di movimenti.
        /// Utile per il debug e per mostrare info all'utente.
        /// </summary>
        public static string GetSequenceStatistics(List<Movimento> movements)
        {
            if (movements == null || movements.Count == 0)
                return "Nessun movimento";

            double totalEffort = 0;
            for (int i = 0; i < movements.Count - 1; i++)
            {
                totalEffort += CalculateMovementEffort(movements[i], movements[i + 1]);
            }

            double avgEffort = totalEffort / Math.Max(1, movements.Count - 1);

            return $"Movimenti: {movements.Count}, Sforzo totale: {totalEffort:F1}, Sforzo medio: {avgEffort:F1}";
        }
    }
}
