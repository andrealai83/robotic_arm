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
        public event Action<string> OnDataSent;
        private readonly StringBuilder rxBuffer = new StringBuilder();

        public void DisableDataReceived() => Port.DataReceived -= HandleDataReceived;
        public void EnableDataReceived() => Port.DataReceived += HandleDataReceived;


        public SerialManager(string portName = "COM5", int baudRate = 115200)
        {
            Port = new SerialPort(portName, baudRate);
            Port.NewLine = "\n";
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
                {
                    Port.WriteLine(data);
                    OnDataSent?.Invoke($"TX:[{NormalizeForLog(data)}]");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to write to serial port: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void WriteRaw(string data)
        {
            try
            {
                if (Port.IsOpen)
                {
                    Port.Write(data);
                    OnDataSent?.Invoke($"TX:[{NormalizeForLog(data)}]");
                }
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
                string chunk = Port.ReadExisting();
                if (string.IsNullOrEmpty(chunk))
                    return;

                rxBuffer.Append(chunk);

                while (true)
                {
                    string current = rxBuffer.ToString();
                    int newLineIndex = current.IndexOf('\n');
                    if (newLineIndex < 0)
                        break;

                    string line = current.Substring(0, newLineIndex).Trim('\r', '\n', '\0', ' ');
                    rxBuffer.Remove(0, newLineIndex + 1);

                    if (!string.IsNullOrEmpty(line))
                        OnDataReceived?.Invoke(line);
                }
            }
            catch (Exception ex)
            {
                OnDataReceived?.Invoke($"SERIAL_ERR: {ex.Message}");
            }
        }

        private static string NormalizeForLog(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            return data.Replace("\r", "\\r").Replace("\n", "\\n");
        }

    }

}
