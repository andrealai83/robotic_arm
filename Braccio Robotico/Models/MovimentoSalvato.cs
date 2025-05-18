using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Braccio_Robotico.Models
{
    public class MovimentoSalvato
    {
        public int Id { get; set; }
        public int PosizioneId { get; set; }
        public int Ordine { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int A { get; set; }
        public string C { get; set; }
    }
}
