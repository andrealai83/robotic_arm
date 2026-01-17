using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Braccio_Robotico.Helper
{
    // Classe che rappresenta un movimento
    public class Movimento
    {
        public double M1 { get; set; }
        public double M2 { get; set; }
        public double M4 { get; set; }
        public double M3 { get; set; }
        public string GRIP { get; set; }
        public override string ToString() => $"M2: {M2} | M1: {M1} | M4: {M4} | M3: {M3} | {GRIP}";
    }
}
