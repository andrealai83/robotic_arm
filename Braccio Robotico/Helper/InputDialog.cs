using System;
using System.Windows.Forms;

namespace Braccio_Robotico.Helper
{
    public class InputDialog : Form
    {
        private TextBox inputBox;
        private Button btnOk;
        private Button btnCancel;

        public string InputText => inputBox.Text;

        public InputDialog(string prompt, string title = "Input", string defaultValue = "")
        {
            this.Text = title;
            this.Width = 400;
            this.Height = 150;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            var lblPrompt = new Label() { Left = 10, Top = 10, Text = prompt, Width = 360 };
            inputBox = new TextBox() { Left = 10, Top = 35, Width = 360, Text = defaultValue };

            btnOk = new Button() { Text = "OK", Left = 210, Width = 75, Top = 70, DialogResult = DialogResult.OK };
            btnCancel = new Button() { Text = "Cancel", Left = 295, Width = 75, Top = 70, DialogResult = DialogResult.Cancel };

            this.Controls.Add(lblPrompt);
            this.Controls.Add(inputBox);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
        }

        public static string Show(string prompt, string title = "Input", string defaultValue = "")
        {
            using (var form = new InputDialog(prompt, title, defaultValue))
            {
                return form.ShowDialog() == DialogResult.OK ? form.InputText : null;
            }
        }
    }
}
