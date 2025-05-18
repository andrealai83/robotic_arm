using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Braccio_Robotico.Helper
{
    // Classe per la gestione della seriale
    public class SerialManager
    {
        public SerialPort Port { get; private set; }
        public event Action<string> OnDataReceived;

        public void DisableDataReceived() => Port.DataReceived -= HandleDataReceived;
        public void EnableDataReceived() => Port.DataReceived += HandleDataReceived;


        public SerialManager(string portName = "COM5", int baudRate = 115200)
        {
            Port = new SerialPort(portName, baudRate);
            Port.DataReceived += HandleDataReceived;
        }

        public void Open()
        {
            try
            {
                Port.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open serial port: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Close()
        {
            try
            {
                if (Port.IsOpen)
                    Port.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to close serial port: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Write(string data)
        {
            try
            {
                if (Port.IsOpen)
                    Port.WriteLine(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to write to serial port: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = Port.ReadLine().Trim();
                OnDataReceived?.Invoke(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Serial read error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }

}
