using System;
using System.Drawing;
using System.Windows.Forms;

namespace Braccio_Robotico
{
    public class FrmLogConsole : Form
    {
        private readonly ListView listViewLog;
        private readonly ColumnHeader colTimestamp;
        private readonly ColumnHeader colMessage;
        private readonly TextBox txtToSend;
        private readonly Button btnSend;
        private readonly Button btnClear;

        public event Action<string> SendRequested;

        public FrmLogConsole()
        {
            Text = "Serial Console";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 540);
            MinimumSize = new Size(720, 420);

            listViewLog = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                HideSelection = false
            };

            colTimestamp = new ColumnHeader { Text = "Timestamp", Width = 150 };
            colMessage = new ColumnHeader { Text = "Message", Width = 700 };
            listViewLog.Columns.AddRange(new[] { colTimestamp, colMessage });

            var bottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 42
            };

            txtToSend = new TextBox
            {
                Left = 8,
                Top = 10,
                Width = 640,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            btnSend = new Button
            {
                Text = "Send",
                Width = 90,
                Height = 24,
                Left = 658,
                Top = 8,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnSend.Click += (_, __) =>
            {
                string text = txtToSend.Text;
                txtToSend.Text = string.Empty;
                SendRequested?.Invoke(text);
            };

            btnClear = new Button
            {
                Text = "Clear",
                Width = 90,
                Height = 24,
                Left = 756,
                Top = 8,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClear.Click += (_, __) => listViewLog.Items.Clear();

            bottom.Controls.Add(txtToSend);
            bottom.Controls.Add(btnSend);
            bottom.Controls.Add(btnClear);

            Controls.Add(listViewLog);
            Controls.Add(bottom);

            Resize += (_, __) =>
            {
                txtToSend.Width = Math.Max(240, Width - 280);
                btnSend.Left = txtToSend.Right + 8;
                btnClear.Left = btnSend.Right + 8;
                colMessage.Width = Math.Max(300, listViewLog.ClientSize.Width - colTimestamp.Width - 12);
            };
        }

        public void AppendLog(string timestamp, string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => AppendLog(timestamp, message)));
                return;
            }

            var item = new ListViewItem(timestamp);
            item.SubItems.Add(message);
            listViewLog.Items.Add(item);
            if (listViewLog.Items.Count > 0)
                listViewLog.Items[listViewLog.Items.Count - 1].EnsureVisible();
        }
    }
}
