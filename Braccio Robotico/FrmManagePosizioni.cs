using Braccio_Robotico.Helper;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Braccio_Robotico
{
    public partial class FrmManagePosizioni : Form
    { 
        private MovimentoManager movimentoManager = new MovimentoManager();
        private SerialManager serialManager;
        public FrmManagePosizioni(SerialManager manager)
        {
            InitializeComponent();
            this.serialManager = manager;
            CaricaPosizioni();

            dgvPosizioni.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPosizioni.MultiSelect = false;
        }

        private void CaricaPosizioni()
        {
            dgvPosizioni.Rows.Clear();
            var lista = PosizioneStorage.GetElencoSetPosizioni();

            foreach (var pos in lista)
            {
                dgvPosizioni.Rows.Add(pos.Id, pos.Nome);
            }
        }

        private void btnElimina_Click(object sender, EventArgs e)
        {
            if (dgvPosizioni.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvPosizioni.SelectedRows[0].Cells[0].Value);
                PosizioneStorage.EliminaSetPosizione(id);
                CaricaPosizioni();
            }
        }

        private async void btnCarica_Click(object sender, EventArgs e)
        {
            if (dgvPosizioni.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvPosizioni.SelectedRows[0].Cells[0].Value);
                var movimenti = PosizioneStorage.CaricaSetPosizioni(id);

                foreach (var movi in movimenti)
                {
                    try
                    {
                        string comando = $"X:{movi.X}\nY:{movi.Y}\nZ:{movi.Z}\nA:{movi.A}\n{movi.C}\nRUN\n";
                        serialManager.Port.Write(comando);
                        Console.WriteLine($"Command sent:\n{comando}");

                        bool success = await WaitForResponse("ready", 20000);
                        if (!success)
                        {
                            MessageBox.Show("Timeout: no response from Arduino.", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error while sending command: {ex.Message}", "Communication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            }
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


        private void btnRinomina_Click(object sender, EventArgs e)
        {
            if (dgvPosizioni.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvPosizioni.SelectedRows[0].Cells[0].Value);
                string nomeCorrente = dgvPosizioni.SelectedRows[0].Cells[1].Value.ToString();

                string nome = InputDialog.Show("Inserisci il nuovo nome del set:", "Salva Posizione", "Nuovo Set");
                if (!string.IsNullOrWhiteSpace(nome))
                {
                    PosizioneStorage.RinominaSet(id, nome);
                    CaricaPosizioni();
                }
            }
        }

        private void btnSalvaSetCorrente_Click(object sender, EventArgs e)
        {
            string nome = InputDialog.Show("Inserisci il nuovo nome del set:", "Salva Posizione", "Nuovo Set");
            if (!string.IsNullOrWhiteSpace(nome))
            {
                PosizioneStorage.SalvaSetPosizioni(nome, movimentoManager.GetMovimenti());
                CaricaPosizioni();
            }
        }

        private void btnChiudi_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvPosizioni_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                dgvPosizioni.Rows[e.RowIndex].Selected = true;
        }
    }
}
