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
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int A { get; set; }
        public string C { get; set; }
        public override string ToString() => $"Y: {Y} | X: {X} | Z: {Z} | A: {A} | {C}";
    }
}
