using Braccio_Robotico.Helper; 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
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
        private bool TestEndStopEnabled = false;
        private bool Running = false;
        private bool Stream = false;
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

            if (message.StartsWith("ENDSTOP"))
            {
                string[] sliptString = message.Substring(6).Trim().Split(':');

                if (sliptString.Length < 4)
                    return;

                string valueEND1 = sliptString[1].Substring(0,1).Trim();
                string valueEND2 = sliptString[2].Substring(0, 1).Trim();
                string valueEND3 = sliptString[3].Substring(0, 1).Trim();
                string valueEND4 = sliptString[4].Substring(0, 1).Trim();

                if (valueEND1 == "0")
                {
                    END1.StateCommon.Back.Color1 = Color.Red;
                    END1.StateCommon.Back.Color2 = Color.Red;
                }
                else
                {
                    END1.StateCommon.Back.Color1 = Color.Lime;
                    END1.StateCommon.Back.Color2 = Color.Lime;
                }

                if (valueEND2 == "0")
                {
                    END2.StateCommon.Back.Color1 = Color.Red;
                    END2.StateCommon.Back.Color2 = Color.Red;
                }
                else
                {
                    END2.StateCommon.Back.Color1 = Color.Lime;
                    END2.StateCommon.Back.Color2 = Color.Lime;
                }

                if (valueEND3 == "0")
                {
                    END3.StateCommon.Back.Color1 = Color.Red;
                    END3.StateCommon.Back.Color2 = Color.Red;
                }
                else
                {
                    END3.StateCommon.Back.Color1 = Color.Lime;
                    END3.StateCommon.Back.Color2 = Color.Lime;
                }

                if (valueEND4 == "0")
                {
                    END4.StateCommon.Back.Color1 = Color.Red;
                    END4.StateCommon.Back.Color2 = Color.Red;
                }
                else
                {
                    END4.StateCommon.Back.Color1 = Color.Lime;
                    END4.StateCommon.Back.Color2 = Color.Lime;
                }
            }

            if (message.StartsWith("EN_DEG"))
            {
                string[] sliptString = message.Substring(7).Trim().Split('M');   

                if(sliptString.Length < 4)
                    return;

                string M1 = sliptString[1].Substring(2).Trim();
                string M2 = sliptString[2].Substring(2).Trim();
                string M3 = sliptString[3].Substring(2).Trim();
                string M4 = sliptString[4] != null ? sliptString[4].Substring(2).Trim() : "0";


                if (double.TryParse(M1, NumberStyles.Float, CultureInfo.InvariantCulture, out var deg))
                {
                    trackBarNumeric1.Value = (int)Math.Round(deg);
                        trackBar1.Value = (int)Math.Round(deg);
                 
                }


                if (double.TryParse(M2, NumberStyles.Float, CultureInfo.InvariantCulture, out var deg2))
                { 

                    trackBarNumeric2.Value = (int)Math.Round(deg2);
                    trackBar2.Value = (int)Math.Round(deg2);
                    
                }


                if (double.TryParse(M3, NumberStyles.Float, CultureInfo.InvariantCulture, out var deg3))
                {
                    trackBarNumeric3.Value = (int)Math.Round(deg3);
                    trackBar3.Value = (int)Math.Round(deg3);
                }


                if (double.TryParse(M4, NumberStyles.Float, CultureInfo.InvariantCulture, out var deg4))
                {
                    trackBarNumeric4.Value = (int)Math.Round(deg4);
                    trackBar4.Value = (int)Math.Round(deg4);
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
                GRIP =  pinza ? "GRIP:120" : "GRIP:0"
            };

            movimentoManager.Add(mov);

            listBoxPositions.Items.Clear();
            foreach (var item in movimentoManager.GetFormattedList())
                listBoxPositions.Items.Add(item);

            btnPlayPosition.Enabled = movimentoManager.HasMovements;
        }

        private async void btnPlayPosition_Click(object sender, EventArgs e)
        {
           playAsync();
        }

        private async Task playAsync()
        {
            var history = new List<Movimento>();

            // Ottieni i movimenti salvati
            var movements = movimentoManager.GetMovimenti();

            // OPZIONE: Ottimizza la sequenza per minimizzare lo sforzo sui giunti
            // Decommenta la riga seguente per abilitare l'ottimizzazione
            //movements = TrajectoryOptimizer.OptimizeSequence(movements);
            //LogMessage(TrajectoryOptimizer.GetSequenceStatistics(movements));

            foreach (var movi in movements)
            {
                try
                {
                    string comando = $"M1:{movi.M1}\nM2:{movi.M2}\nM4:{movi.M4}\nM3:{movi.M3}\n{movi.GRIP}\nRUN\n";
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
                     $"MP:{trackBarP.Value};" +
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
                GRIP = pinza ? "GRIP:120" : "GRIP:0"
            };

             viewer3D.UpdateAngles(
                 trackBar1.Value,  // Y → base
                 trackBar2.Value,     // X → snodo 1
                 trackBar3.Value,     // Z → snodo 2
                 trackBar4.Value      // A → snodo 3 (calamita)
             ); 
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            trackBarNumeric2.Value = trackBar2.Value;

            var mov = new Movimento
            {
                M1 = trackBar1.Value,
                M2 = trackBar2.Value,
                M3 = trackBar3.Value,
                M4 = trackBar4.Value,
                GRIP = pinza ? "GRIP:120" : "GRIP:0"
            };
            viewer3D.UpdateAngles(
                trackBar1.Value,  // Y → base
                trackBar2.Value,     // X → snodo 1
                trackBar3.Value,     // Z → snodo 2
                trackBar4.Value      // A → snodo 3 (calamita)
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
                GRIP = pinza ? "GRIP:120" : "GRIP:0"
            };

            viewer3D.UpdateAngles(
                trackBar1.Value,  // Y → base
                trackBar2.Value,     // X → snodo 1
                trackBar3.Value,     // Z → snodo 2
                trackBar4.Value      // A → snodo 3 (calamita)
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
                GRIP = pinza ? "GRIP:120" : "GRIP:0"
            };

            viewer3D.UpdateAngles(
                 trackBar1.Value,  // Y → base
                 trackBar2.Value,     // X → snodo 1
                 trackBar3.Value,     // Z → snodo 2
                 trackBar4.Value      // A → snodo 3 (calamita)
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
            if (trackBarNumeric2.Value <= 160)
                trackBar2.Value = int.Parse(trackBarNumeric2.Value.ToString());
            else
                trackBar2.Value = 160;
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
                        GRIP = pinza ? "GRIP:120" : "GRIP:0"
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
                        GRIP = d.GRIP ?? string.Empty
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

       
        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            string MotorStream = Stream ? "STREAM:1" : "STREAM:0";
            serialManager.Write(MotorStream);
            Stream = !Stream;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            serialManager.Write(txtToSend.Text);
            txtToSend.Text = "";
        }

        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            listViewLog.Clear();
        }

        private void trackBarP_ValueChanged(object sender, EventArgs e)
        { 
            serialManager.Write("GRIP:" + trackBarP.Value.ToString());
        }

        private async void ckPlayLoop_CheckedChanged(object sender, EventArgs e)
        {
            if (ckPlayLoop.Checked)
            {
                while (ckPlayLoop.Checked)
                {
                    await playAsync();
                    await Task.Delay(200);
                }
            }
        }

        bool pinza = false;

        private void button1_Click(object sender, EventArgs e)
        {
            string MotorStream = pinza ? "GRIP:120" : "GRIP:0";
            serialManager.Write(MotorStream);
            pinza = !pinza;
        }

        private void btnTestEndStop_Click(object sender, EventArgs e)
        {
            TestEndStopEnabled = !TestEndStopEnabled;
            serialManager.Write("TE:" + (TestEndStopEnabled ? "1" : "0"));
        }
    }
}
