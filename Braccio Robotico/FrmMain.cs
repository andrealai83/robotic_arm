using Braccio_Robotico.Helper; 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Braccio_Robotico
{
    public partial class FrmMain : Form
    {
        private SerialManager serialManager;
        private MovimentoManager movimentoManager = new MovimentoManager();
        private string MagState = "C:0";
        private string MotorState = "ENA:1";
        private bool EndStopEnabled = true;
        private bool Running = false;
        private Braccio_Robotico.Braccio3DWindow viewer3D = new Braccio_Robotico.Braccio3DWindow();
        public FrmMain()
        {
            InitializeComponent(); 
            serialManager = new SerialManager();
            serialManager.OnDataReceived += LogMessage; 
            RobotConfig.InizializzaDatabaseSeNecessario();
            RobotConfig.CaricaConfigurazioni(); 
        }

        private void LogMessage(string message)
        {
            if (listViewLog.InvokeRequired)
            {
                listViewLog.Invoke(new Action(() => LogMessage(message)));
                return;
            }

            if (message.StartsWith("EN_DEG_M1:"))
            {
                if (double.TryParse(message.Substring(10), NumberStyles.Float, CultureInfo.InvariantCulture, out var deg))
                {
                    trackBarNumeric1.Value = (int)Math.Round(deg);
                    trackBar1.Value = (int)Math.Round(deg);
                }
            }

            var item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            item.SubItems.Add(message);
            listViewLog.Items.Add(item);
            listViewLog.Items[listViewLog.Items.Count - 1].EnsureVisible();
             
            //AggiornaTrackbarDaPosizione(message);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            serialManager = new SerialManager(comboBoxComPorts.Text);
            serialManager.OnDataReceived += LogMessage;
            serialManager.Open();
              
            viewer3D.UpdateAngles(
                 63,    // Y → base
                 -125,  // X → snodo 1
                 140,   // Z → snodo 2
                 -66    // A → snodo 3 (calamita)
             );

            if (serialManager.Port.IsOpen)
            { 
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true; 
                RobotConfig.SendConfiguration(serialManager.Port);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            serialManager.Close();
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxComPorts.Items.Clear();
            comboBoxComPorts.Items.AddRange(SerialPort.GetPortNames());
            if (comboBoxComPorts.Items.Count > 0)
                comboBoxComPorts.SelectedIndex = 0;

            var host = new ElementHost();
            host.Dock = DockStyle.Fill;  
            host.Child = viewer3D; 
            pnlSimulation.Controls.Add(host);
        }

        private void btnSavePosition_Click(object sender, EventArgs e)
        { 
            var mov = new Movimento
            {
                M2 = trackBar2.Value,
                M1 = trackBar1.Value,
                M4 = trackBar4.Value,
                M3 = trackBar3.Value,
                C = MagState
            };

            movimentoManager.Add(mov);

            listBoxPositions.Items.Clear();
            foreach (var item in movimentoManager.GetFormattedList())
                listBoxPositions.Items.Add(item);

            btnPlayPosition.Enabled = movimentoManager.HasMovements;
        }

        private async void btnPlayPosition_Click(object sender, EventArgs e)
        {
            var history = new List<Movimento>();

            foreach (var movi in movimentoManager.GetMovimenti())
            {
                try
                {
                    string comando = $"M1:{movi.M1}\nM3:{movi.M2}\nM4:{movi.M4}\nM3:{movi.M3}\n{movi.C}\nRUN\n";
                    serialManager.Port.Write(comando);
                    history.Add(movi);  

                    viewer3D.UpdateAngles(
                         movi.M2,    // Y → base
                         movi.M1,    // X → snodo 1
                         movi.M4,    // Z → snodo 2
                         movi.M3     // A → snodo 3 (calamita)
                     );

                    LogMessage($"Command sent:\n{comando}");

                    bool success = await WaitForResponse("ready", 20000);
                    if (!success)
                    {
                        MessageBox.Show("Timeout: no response from Arduino.", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }

                    Console.WriteLine("Movement completed.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while sending command: {ex.Message}", "Communication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
             
            Console.WriteLine("All movements sent.");
        }
         
        private Task<bool> WaitForResponse(string attesa, int timeout)
        {
            return Task.Run(() =>
            {
                serialManager.DisableDataReceived();

                DateTime startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < timeout)
                {
                    try
                    {
                        if (serialManager.Port.BytesToRead > 0)
                        {
                            string risposta = serialManager.Port.ReadLine().Trim();
                            Console.WriteLine($"Risposta ricevuta: {risposta}");
                             
                            if (risposta == attesa)
                            {
                                serialManager.EnableDataReceived();
                                return true;
                            }
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

                    Thread.Sleep(10);
                }

                serialManager.EnableDataReceived();
                return false;
            });
        }
         
        private void btnClear_Click(object sender, EventArgs e)
        {
            movimentoManager.Clear();
            listBoxPositions.Items.Clear();
            btnPlayPosition.Enabled = false;
        }

        private void btnGoAll_Click(object sender, EventArgs e)
        {
            string command = $"M1:{trackBar1.Value};" +
                     $"M2:{trackBar2.Value};" +
                     $"M3:{trackBar3.Value};" +
                     $"M4:{trackBar4.Value};" +
                     $"{MagState};" +
                     "EXEC\n";

            serialManager.Write(command);
            Running = true;
        }

        private void GoHome()
        { 
            serialManager.Write("HOME");
            trackBar2.Value = 0;
            trackBar4.Value = 0;
            trackBar1.Value = 0;
            trackBar3.Value = 0;

        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        private void magnetON_Click(object sender, EventArgs e)
        {
            ToggleMagnet(true);
        }

        private void magnetOFF_Click(object sender, EventArgs e)
        {
            ToggleMagnet(false);
        }

        private void ToggleMagnet(bool state)
        {
            MagState = state ? "C:1" : "C:0";
            serialManager.Write(MagState);
            gpMagnetState.Text = state ? "Magnet state (ON)" : "Magnet state (OFF)";
            magnetON.Enabled = !state;
            magnetOFF.Enabled = state;
        }

        private void ToggleMotor(bool state)
        {
            MotorState = state ? "ENA:1" : "ENA:0";
            serialManager.Write(MotorState);
            grpMotor.Text = state ? "Motor state (ON)" : "Motor state (OFF)";
            btnMotorON.Enabled = !state;
            btnMotorOFF.Enabled = state;
        }


        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            trackBarNumeric1.Value = trackBar1.Value;
            var mov = new Movimento
            {
                M1 = trackBar1.Value,
                M2 = trackBar2.Value,
                M3 = trackBar3.Value,
                M4 = trackBar4.Value,
                C = MagState
            };

            viewer3D.UpdateAngles(
                 trackBar2.Value,  // Y → base
                 trackBar1.Value,     // X → snodo 1
                 trackBar4.Value,     // Z → snodo 2
                 trackBar3.Value      // A → snodo 3 (calamita)
             ); 
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            trackBarNumeric1.Value = trackBar1.Value;
            var mov = new Movimento
            {
                M1 = trackBar1.Value,
                M2 = trackBar2.Value,
                M3 = trackBar3.Value,
                M4 = trackBar4.Value,
                C = MagState
            };
            viewer3D.UpdateAngles(
                 trackBar2.Value,  // Y → base
                 trackBar1.Value,     // X → snodo 1
                 trackBar4.Value,     // Z → snodo 2
                 trackBar3.Value      // A → snodo 3 (calamita)
             ); 
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            trackBarNumeric3.Value = trackBar3.Value;

            var mov = new Movimento
            {
                M1 = trackBar1.Value,
                M2 = trackBar2.Value,
                M3 = trackBar3.Value,
                M4 = trackBar4.Value,
                C = MagState
            };

            viewer3D.UpdateAngles(
                 trackBar2.Value,  // Y → base
                 trackBar1.Value,     // X → snodo 1
                 trackBar4.Value,     // Z → snodo 2
                 trackBar3.Value      // A → snodo 3 (calamita)
             ); 
        }

        private void trackBar4_ValueChanged(object sender, EventArgs e)
        {
            trackBarNumeric4.Value = trackBar4.Value;

            var mov = new Movimento
            {
                M1 = trackBar1.Value,
                M2 = trackBar2.Value,
                M3 = trackBar3.Value,
                M4 = trackBar4.Value, 
                C = MagState
            };

            viewer3D.UpdateAngles(
                 trackBar2.Value,  // Y → base
                 trackBar1.Value,     // X → snodo 1
                 trackBar4.Value,     // Z → snodo 2
                 trackBar3.Value      // A → snodo 3 (calamita)
             );
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            FrmConfigurations frmConfigurations = new FrmConfigurations();
            frmConfigurations.ShowDialog();
        }

        private void btnGestionePosizioni_Click(object sender, EventArgs e)
        {
            var frm = new FrmManagePosizioni(serialManager);
            frm.ShowDialog(); 
        }

        private void btnSalvaSetCorrente_Click(object sender, EventArgs e)
        {
            string nome = InputDialog.Show("Inserisci il nome del set:", "Salva Posizione", "Nuovo Set");
            if (!string.IsNullOrWhiteSpace(nome))
            {
                PosizioneStorage.SalvaSetPosizioni(nome, movimentoManager.GetMovimenti());
                MessageBox.Show("Set salvato correttamente!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } 
        }
 

        private void trackBarNumeric3_ValueChanged(object sender, EventArgs e)
        {
           trackBar3.Value = int.Parse(trackBarNumeric3.Value.ToString());
        }

        private void trackBarNumeric1_ValueChanged(object sender, EventArgs e)
        {
            trackBar4.Value = int.Parse(trackBarNumeric4.Value.ToString());
        }

        private void trackBarNumericBase_ValueChanged(object sender, EventArgs e)
        {
           trackBar2.Value = int.Parse(trackBarNumeric2.Value.ToString());
        }

        private void trackBarNumeric2_ValueChanged(object sender, EventArgs e)
        {
            trackBar2.Value = int.Parse(trackBarNumeric2.Value.ToString());
        }

        private void trackBarNumeric4_ValueChanged(object sender, EventArgs e)
        {
            trackBar4.Value = int.Parse(trackBarNumeric4.Value.ToString());
        }

        private void BtnSetHome_Click(object sender, EventArgs e)
        {
            trackBar1.Value = 0;
            trackBar2.Value = 0;
            trackBar4.Value = 0;
            trackBar3.Value = 0;

            serialManager.Write("SETHOME");
        }   
    

    private void AggiornaTrackbarDaPosizione(string message)
    {
        if (!message.StartsWith("NEWPOSITION:")) return;

        try
        {
            // Rimuove il prefisso
            string valori = message.Substring("NEWPOSITION:".Length);
            // Esempio: X:-1;Y:9;Z:-1;A:-1
            string[] componenti = valori.Split(';');

            int x = 0, y = 0, z = 0, a = 0, c = 0, save = 0;

            foreach (var comp in componenti)
            {
                var coppia = comp.Split(':');
                if (coppia.Length != 2) continue;

                string label = coppia[0];
                int valore = int.TryParse(coppia[1], out int v) ? v : 0;

                switch (label)
                {
                    case "X": x = valore; break;
                    case "Y": y = valore; break;
                    case "Z": z = valore; break;
                    case "A": a = valore; break;
                    case "C": c = valore; break;
                    case "SAVE": save = valore; break;
                }
            }

            // Imposta i valori sui TrackBar e i NumericUpDown
            trackBar1.Value = Math.Max(trackBar1.Minimum, Math.Min(trackBar1.Maximum, x));
            trackBar2.Value = Math.Max(trackBar2.Minimum, Math.Min(trackBar2.Maximum, y));
            trackBar4.Value = Math.Max(trackBar4.Minimum, Math.Min(trackBar4.Maximum, z));
            trackBar3.Value = Math.Max(trackBar3.Minimum, Math.Min(trackBar3.Maximum, a));
                 
            ToggleMagnet(c == 1 ? true : false);

                if (save == 1)
                { 
                    var mov = new Movimento
                    {
                        M2 = trackBar2.Value,
                        M1 = trackBar1.Value,
                        M4 = trackBar4.Value,
                        M3 = trackBar3.Value,
                        C = MagState
                    };

                    movimentoManager.Add(mov);

                    listBoxPositions.Items.Clear();
                    foreach (var item in movimentoManager.GetFormattedList())
                        listBoxPositions.Items.Add(item);

                    btnPlayPosition.Enabled = movimentoManager.HasMovements;
                }

            // Forza l'aggiornamento della simulazione 3D
            viewer3D.UpdateAngles(y, x, z, a);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore nel parsing NEWPOSITION: " + ex.Message);
        }
        }
           
        private void btnESDisabled_Click(object sender, EventArgs e)
        {
            if (EndStopEnabled)
                serialManager.Write("ENDSTOP_ENABLED_0");
            else
                serialManager.Write("ENDSTOP_ENABLED_1");

            EndStopEnabled = !EndStopEnabled;

            if (EndStopEnabled) btnESDisabled.StateCommon.Back.Color1 = Color.White; else btnESDisabled.StateCommon.Back.Color1 = Color.Red;

            btnESDisabled.StateNormal.Back.Color1 = btnESDisabled.StateCommon.Back.Color1;
            btnESDisabled.StatePressed.Back.Color1 = btnESDisabled.StateCommon.Back.Color1;
        }

        private void btnSTOP_Click(object sender, EventArgs e)
        {
            serialManager.Write("EMERGENCY_STOP");
        }

        private void btnGoHome4_Click(object sender, EventArgs e)
        {
            serialManager.Write("HOME_4");
            trackBar4.Value = 0; 
        }

        private void btnGoHome3_Click(object sender, EventArgs e)
        {
            serialManager.Write("HOME_3");
            trackBar3.Value = 0;
        }

        private void btnGoHome2_Click(object sender, EventArgs e)
        {
            serialManager.Write("HOME_2");
            trackBar2.Value = 0;
        }

        private void btnGoHome1_Click(object sender, EventArgs e)
        {
            serialManager.Write("HOME_1");
            trackBar1.Value = 0;
        }

        private static int Clamp360(int v)
        {
            // porta in [0,360)
            v %= 360;
            if (v < 0) v += 360;
            return v;
        }


        private void btnImport_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleziona file posizioni JSON";
                ofd.Filter = "File JSON (*.json)|*.json|Tutti i file (*.*)|*.*";

                if (ofd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    string json = File.ReadAllText(ofd.FileName);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var listDto = JsonSerializer.Deserialize<List<Movimento>>(json, options);

                    if (listDto == null || listDto.Count == 0)
                    {
                        MessageBox.Show("Il file JSON non contiene posizioni valide.",
                            "Importazione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // mappa a int con arrotondamento e clamp
                    var mapped = listDto.Select(d => new Movimento
                    {
                        M1 = Clamp360((int)Math.Round(d.M1)),
                        M2 = Clamp360((int)Math.Round(d.M2)),
                        M3 = Clamp360((int)Math.Round(d.M3)),
                        M4 = Clamp360((int)Math.Round(d.M4)),
                        C = d.C ?? string.Empty
                    }).ToList();

                    foreach (var mov in mapped)
                        movimentoManager.Add(mov);

                    listBoxPositions.Items.Clear();
                    foreach (var item in movimentoManager.GetFormattedList())
                        listBoxPositions.Items.Add(item);

                    btnPlayPosition.Enabled = movimentoManager.HasMovements;

                    MessageBox.Show($"{mapped.Count} posizioni importate con successo.",
                        "Importazione completata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Errore durante l'importazione: {ex.Message}",
                        "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnMotorON_Click(object sender, EventArgs e)
        {
            ToggleMotor(true);
        }

        private void btnMotorOFF_Click(object sender, EventArgs e)
        {
            ToggleMotor(false);
        }
    }
}
