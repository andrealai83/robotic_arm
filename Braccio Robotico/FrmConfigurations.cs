using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Braccio_Robotico
{
    public partial class FrmConfigurations : Form
    {
        public FrmConfigurations()
        {
            InitializeComponent();
        }

        private void FrmConfigurations_Load(object sender, EventArgs e)
        {
            valPassiPerGiro.Text = RobotConfig.PassiPerGiro.ToString();
            valMaxAccel.Text = RobotConfig.MaxAccel.ToString();
            valMaxSpeed.Text = RobotConfig.MaxSpeed.ToString();
            valMicrostep.Text = RobotConfig.Microstep.ToString();
            valENA1.Text = RobotConfig.ENA1.ToString();
            valDIR1.Text = RobotConfig.DIR1.ToString();
            valPUL1.Text = RobotConfig.PUL1.ToString();
            valENA2.Text = RobotConfig.ENA2.ToString();
            valDIR2.Text = RobotConfig.DIR2.ToString();
            valPUL2.Text = RobotConfig.PUL2.ToString();
            valENA3.Text = RobotConfig.ENA3.ToString();
            valDIR3.Text = RobotConfig.DIR3.ToString();
            valPUL3.Text = RobotConfig.PUL3.ToString();
            valENA4.Text = RobotConfig.ENA4.ToString();
            valDIR4.Text = RobotConfig.DIR4.ToString();
            valPUL4.Text = RobotConfig.PUL4.ToString();
            valTransistorPin.Text = RobotConfig.TransistorPin.ToString();
        }

         
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                RobotConfig.PassiPerGiro = int.Parse(valPassiPerGiro.Text);
                RobotConfig.MaxAccel = int.Parse(valMaxAccel.Text);
                RobotConfig.MaxSpeed = int.Parse(valMaxSpeed.Text);
                RobotConfig.Microstep = int.Parse(valMicrostep.Text);

                RobotConfig.ENA1 = int.Parse(valENA1.Text);
                RobotConfig.DIR1 = int.Parse(valDIR1.Text);
                RobotConfig.PUL1 = int.Parse(valPUL1.Text);
                RobotConfig.ENA2 = int.Parse(valENA2.Text);
                RobotConfig.DIR2 = int.Parse(valDIR2.Text);
                RobotConfig.PUL2 = int.Parse(valPUL2.Text);
                RobotConfig.ENA3 = int.Parse(valENA3.Text);
                RobotConfig.DIR3 = int.Parse(valDIR3.Text);
                RobotConfig.PUL3 = int.Parse(valPUL3.Text);
                RobotConfig.ENA4 = int.Parse(valENA4.Text);
                RobotConfig.DIR4 = int.Parse(valDIR4.Text);
                RobotConfig.PUL4 = int.Parse(valPUL4.Text);
                RobotConfig.TransistorPin = int.Parse(valTransistorPin.Text);

                RobotConfig.SalvaConfigurazioni();

                MessageBox.Show("Configuration saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
         

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
