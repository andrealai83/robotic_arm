using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Braccio_Robotico
{
    public partial class FrmMain : Form
    {
        SerialPort serial = new SerialPort("COM5", 115200);

        Boolean MagnetState = false;

        System.Collections.Generic.List<movimento> ListaMovimenti = new System.Collections.Generic.List<movimento>();

        public FrmMain()
        {
            InitializeComponent();
            gpMagnetState.Text = "Magnet state (OFF)";
            serial.DataReceived += Serial_DataReceived;
            serial.Open();
            Init();
        }

        private void Init()
        {
            RobotConfig.SendConfiguration(serial);
            GoHome();
            

        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serial.ReadLine().Trim();
                LogMessage(data);
            }
            catch (Exception ex)
            {
                LogMessage($"Errore di comunicazione: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            if (listViewLog.InvokeRequired)
            {
                listViewLog.Invoke(new Action(() => LogMessage(message)));
                return;
            }

            var item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); 
            item.SubItems.Add(message); // Aggiungi il messaggio come SubItem
            listViewLog.Items.Add(item);

            // Scorri automaticamente verso l'ultimo elemento
            // Scorri automaticamente verso l'ultimo elemento
            if (listViewLog.Items.Count > 0)
            {
                var lastItem = listViewLog.Items[listViewLog.Items.Count - 1] as ListViewItem;
                lastItem?.EnsureVisible();
            }

        }
         
        private void InviaComando(char asse, decimal valore)
        {
            // Invio comando singolo per asse
            string comando = $"{asse}:{valore}\n";
            serial.Write(comando);
            Console.WriteLine($"Comando inviato: {comando}");
        }

        private void InviaComandiMultipli()
        {
            // Invio comandi per tutti gli assi
            string comando = $"X:{trackBar2.Value}\n" +
                             $"Y:{trackBarBase.Value}\n" +
                             $"Z:{trackBar1.Value}\n" +
                             $"A:{trackBar3.Value}\n";
            serial.Write(comando);
            Console.WriteLine($"Comandi multipli inviati:\n{comando}");
        }
 
        private void buttonAll_Click(object sender, EventArgs e)
        {
            InviaComandiMultipli();
        }
         
        private void button6_Click(object sender, EventArgs e)
        {
            System.Collections.Generic.List<movimento> lista = new System.Collections.Generic.List<movimento>();

            lista.Add(new movimento { Y = 0, X = 0, Z = 0 });
            lista.Add(new movimento { Y = 5, X = 10, Z = -5 });
            lista.Add(new movimento { Y = 10, X = 20, Z = -10 });
            lista.Add(new movimento { Y = 15, X = 30, Z = -15 });
            lista.Add(new movimento { Y = 20, X = 40, Z = -20 });
            lista.Add(new movimento { Y = 15, X = 30, Z = -15 });
            lista.Add(new movimento { Y = 10, X = 20, Z = -10 });
            lista.Add(new movimento { Y = 0, X = 0, Z = 0 }); 
            lista.Add(new movimento { Y = 5, X = 10, Z = -5 });
            lista.Add(new movimento { Y = 10, X = 20, Z = -10 });
            lista.Add(new movimento { Y = 15, X = 30, Z = -15 });
            lista.Add(new movimento { Y = 20, X = 40, Z = -20 });
            lista.Add(new movimento { Y = 15, X = 30, Z = -15 });
            lista.Add(new movimento { Y = 10, X = 20, Z = -10 });
            lista.Add(new movimento { Y = 0, X = 0, Z = 0 });

            foreach (movimento movi in lista)
            {
                // Invia il comando
                string comando = $"X:{movi.X}\nY:{movi.Y}\nZ:{movi.Z}\n";
                serial.Write(comando);
                Console.WriteLine($"Comando inviato: {comando}");

                // Aspetta la conferma "ok" dall'Arduino
                while (true)
                {
                    string risposta = serial.ReadLine().Trim();
                    if (risposta == "ok")
                    {
                        Console.WriteLine("Comando completato.");
                        break;
                    }
                }
            }
        }
       
        private void TurnMagnetState(int state) {
            MagnetState = state == 1 ? true : false;
            string comando = $"C:{state}";
            serial.Write(comando);
            Console.WriteLine($"Comando inviato: {comando}"); 
            gpMagnetState.Text = MagnetState ? "Magnet state (ON)" : "Magnet state (OFF)";
        }

        private void Form1_Load(object sender, EventArgs e)
        { 
          
        }

        private void magnetON_Click(object sender, EventArgs e)
        {
            TurnMagnetState(1);
        }

        private void magnetOFF_Click(object sender, EventArgs e)
        {
            TurnMagnetState(0);
        }

        private void kryptonListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSavePosition_Click(object sender, EventArgs e)
        {
            movimento mov = new movimento
            {
                Y = trackBarBase.Value,
                X = trackBar2.Value,
                Z = trackBar1.Value,
                A = trackBar3.Value
            };


            ListaMovimenti.Add(mov);

            // Aggiorna la lista visiva
            listBoxPositions.Items.Clear();
            foreach (var item in ListaMovimenti)
            {
                listBoxPositions.Items.Add($"Y: {item.Y} | X: {item.X} | Z: {item.Z} | A: {item.A}");
            }

            Console.WriteLine("Movimento aggiunto alla lista.");
        }

        private async void btnPlayPosition_Click(object sender, EventArgs e)
        {
            foreach (movimento movi in ListaMovimenti)
            {
                string comando = $"X:{movi.X}\n" +
                                 $"Y:{movi.Y}\n" +
                                 $"Z:{movi.Z}\n" +
                                 $"A:{movi.A}\n";

                try
                {
                    // Invia il comando
                    serial.Write(comando);
                    Console.WriteLine($"Comando inviato: {comando}");

                    // Attende la conferma "ready"
                    bool esito = await AttendiRispostaAsync("ready", 20000); // Timeout di 20 secondi

                    if (esito)
                    {
                        Console.WriteLine("Movimento completato.");
                    }
                    else
                    {
                        Console.WriteLine("Timeout: nessuna risposta dall'Arduino.");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore di comunicazione: {ex.Message}");
                    break;
                }
            }

            Console.WriteLine("Tutti i movimenti sono stati inviati.");
        }

        private Task<bool> AttendiRispostaAsync(string attesa, int timeout)
        {
            return Task.Run(() =>
            {
                DateTime startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < timeout)
                {
                    try
                    {
                        if (serial.BytesToRead > 0)
                        {
                            string risposta = serial.ReadLine().Trim();
                            Console.WriteLine($"Risposta ricevuta: {risposta}");

                            if (risposta == attesa)
                                return true;
                        }
                    }
                    catch (TimeoutException)
                    {
                        // Ignora il timeout
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Errore di lettura: {ex.Message}");
                        break;
                    }

                    // Evita di bloccare completamente il thread
                    Thread.Sleep(10);
                }

                return false;
            });
        }



        private void btnGoBase_Click(object sender, EventArgs e)
        {
            InviaComando('Y', trackBarBase.Value);
        }

        private void btnGo1_Click(object sender, EventArgs e)
        {
            InviaComando('Z', trackBar1.Value);
        }

        private void btnGo2_Click(object sender, EventArgs e)
        {
            InviaComando('X', trackBar2.Value);
        }

        private void btnGo3_Click(object sender, EventArgs e)
        {
            InviaComando('A', trackBar3.Value);
        }

        private void btnGoAll_Click(object sender, EventArgs e)
        {
            InviaComandiMultipli();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        private void GoHome()
        {
            trackBarBase.Value = 0;
            trackBar1.Value = 0;
            trackBar2.Value = 0;
            trackBar3.Value = 0;
            InviaComandiMultipli();
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            btnGo3.Text = $"GO ({trackBar3.Value})";
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            btnGo1.Text = $"GO ({trackBar1.Value})";
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            btnGo2.Text = $"GO ({trackBar2.Value})";
        }

        private void trackBarBase_ValueChanged(object sender, EventArgs e)
        {
            btnGoBase.Text = $"GO ({trackBarBase.Value})";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ListaMovimenti = new System.Collections.Generic.List<movimento>();
            listBoxPositions.Items.Clear(); 
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
