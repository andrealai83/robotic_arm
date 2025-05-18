using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Braccio_Robotico.Helper
{
    // Classe per la gestione dei movimenti
    public class MovimentoManager
    {
        public List<Movimento> Lista { get; private set; } = new List<Movimento>();

        public void Add(Movimento mov)
        {
            Lista.Add(mov);
        }

        public void Clear()
        {
            Lista.Clear();
        }

        public IEnumerable<string> GetFormattedList()
        {
            return Lista.Select(m => m.ToString());
        }

        public bool HasMovements => Lista.Count > 0;

        public List<Movimento> GetMovimenti() => Lista.ToList();
    }
}
