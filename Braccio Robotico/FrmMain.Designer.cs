namespace Braccio_Robotico
{
    partial class FrmMain
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.button6 = new System.Windows.Forms.Button();
            this.listBoxPositions = new System.Windows.Forms.ListBox();
            this.listViewLog = new System.Windows.Forms.ListView();
            this.colTimestamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClear = new Krypton.Toolkit.KryptonButton();
            this.btnSavePosition = new Krypton.Toolkit.KryptonButton();
            this.btnPlayPosition = new Krypton.Toolkit.KryptonButton();
            this.magnetON = new Krypton.Toolkit.KryptonButton();
            this.magnetOFF = new Krypton.Toolkit.KryptonButton();
            this.gpMagnetState = new Krypton.Toolkit.KryptonGroupBox();
            this.trackBar3 = new Krypton.Toolkit.KryptonTrackBar();
            this.trackBar2 = new Krypton.Toolkit.KryptonTrackBar();
            this.trackBar1 = new Krypton.Toolkit.KryptonTrackBar();
            this.trackBarBase = new Krypton.Toolkit.KryptonTrackBar();
            this.btnGoBase = new Krypton.Toolkit.KryptonButton();
            this.btnGo1 = new Krypton.Toolkit.KryptonButton();
            this.btnGo2 = new Krypton.Toolkit.KryptonButton();
            this.btnGo3 = new Krypton.Toolkit.KryptonButton();
            this.btnGoAll = new Krypton.Toolkit.KryptonButton();
            this.kryptonButton1 = new Krypton.Toolkit.KryptonButton();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gpMagnetState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gpMagnetState.Panel)).BeginInit();
            this.gpMagnetState.Panel.SuspendLayout();
            this.gpMagnetState.SuspendLayout();
            this.SuspendLayout();
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(565, 24);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 13;
            this.button6.Text = "DEMO";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // listBoxPositions
            // 
            this.listBoxPositions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxPositions.FormattingEnabled = true;
            this.listBoxPositions.Location = new System.Drawing.Point(6, 54);
            this.listBoxPositions.Name = "listBoxPositions";
            this.listBoxPositions.Size = new System.Drawing.Size(302, 628);
            this.listBoxPositions.TabIndex = 16;
            // 
            // listViewLog
            // 
            this.listViewLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTimestamp,
            this.colMessage});
            this.listViewLog.Dock = System.Windows.Forms.DockStyle.Right;
            this.listViewLog.FullRowSelect = true;
            this.listViewLog.GridLines = true;
            this.listViewLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewLog.HideSelection = false;
            this.listViewLog.Location = new System.Drawing.Point(1196, 0);
            this.listViewLog.Name = "listViewLog";
            this.listViewLog.Size = new System.Drawing.Size(380, 683);
            this.listViewLog.TabIndex = 19;
            this.listViewLog.UseCompatibleStateImageBehavior = false;
            this.listViewLog.View = System.Windows.Forms.View.Details;
            // 
            // colTimestamp
            // 
            this.colTimestamp.Text = "Timestamp";
            this.colTimestamp.Width = 150;
            // 
            // colMessage
            // 
            this.colMessage.Text = "Message";
            this.colMessage.Width = 600;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.listBoxPositions);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(885, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(311, 683);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnClear);
            this.panel1.Controls.Add(this.btnSavePosition);
            this.panel1.Controls.Add(this.btnPlayPosition);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(305, 31);
            this.panel1.TabIndex = 17;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(3, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(90, 25);
            this.btnClear.TabIndex = 26;
            this.btnClear.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnClear.Values.Text = "Clear All";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSavePosition
            // 
            this.btnSavePosition.Location = new System.Drawing.Point(107, 3);
            this.btnSavePosition.Name = "btnSavePosition";
            this.btnSavePosition.Size = new System.Drawing.Size(90, 25);
            this.btnSavePosition.TabIndex = 24;
            this.btnSavePosition.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnSavePosition.Values.Text = "Save Position";
            this.btnSavePosition.Click += new System.EventHandler(this.btnSavePosition_Click);
            // 
            // btnPlayPosition
            // 
            this.btnPlayPosition.Location = new System.Drawing.Point(211, 3);
            this.btnPlayPosition.Name = "btnPlayPosition";
            this.btnPlayPosition.Size = new System.Drawing.Size(90, 25);
            this.btnPlayPosition.TabIndex = 25;
            this.btnPlayPosition.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnPlayPosition.Values.Text = "Play";
            this.btnPlayPosition.Click += new System.EventHandler(this.btnPlayPosition_Click);
            // 
            // magnetON
            // 
            this.magnetON.Location = new System.Drawing.Point(14, 18);
            this.magnetON.Name = "magnetON";
            this.magnetON.Size = new System.Drawing.Size(90, 25);
            this.magnetON.TabIndex = 21;
            this.magnetON.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.magnetON.Values.Text = "ON";
            this.magnetON.Click += new System.EventHandler(this.magnetON_Click);
            // 
            // magnetOFF
            // 
            this.magnetOFF.Location = new System.Drawing.Point(107, 18);
            this.magnetOFF.Name = "magnetOFF";
            this.magnetOFF.Size = new System.Drawing.Size(90, 25);
            this.magnetOFF.TabIndex = 22;
            this.magnetOFF.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.magnetOFF.Values.Text = "OFF";
            this.magnetOFF.Click += new System.EventHandler(this.magnetOFF_Click);
            // 
            // gpMagnetState
            // 
            this.gpMagnetState.CaptionStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.gpMagnetState.Location = new System.Drawing.Point(12, 12);
            // 
            // gpMagnetState.Panel
            // 
            this.gpMagnetState.Panel.Controls.Add(this.magnetON);
            this.gpMagnetState.Panel.Controls.Add(this.magnetOFF);
            this.gpMagnetState.Size = new System.Drawing.Size(217, 77);
            this.gpMagnetState.TabIndex = 23;
            // 
            // trackBar3
            // 
            this.trackBar3.Location = new System.Drawing.Point(12, 92);
            this.trackBar3.Maximum = 360;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(603, 34);
            this.trackBar3.TabIndex = 24;
            this.trackBar3.TrackBarSize = Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.trackBar3.ValueChanged += new System.EventHandler(this.trackBar3_ValueChanged);
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(12, 168);
            this.trackBar2.Maximum = 360;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(603, 34);
            this.trackBar2.TabIndex = 25;
            this.trackBar2.TrackBarSize = Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.trackBar2.ValueChanged += new System.EventHandler(this.trackBar2_ValueChanged);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(11, 128);
            this.trackBar1.Maximum = 360;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(603, 34);
            this.trackBar1.TabIndex = 26;
            this.trackBar1.TrackBarSize = Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // trackBarBase
            // 
            this.trackBarBase.Location = new System.Drawing.Point(12, 208);
            this.trackBarBase.Maximum = 360;
            this.trackBarBase.Name = "trackBarBase";
            this.trackBarBase.Size = new System.Drawing.Size(603, 34);
            this.trackBarBase.TabIndex = 27;
            this.trackBarBase.TrackBarSize = Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.trackBarBase.ValueChanged += new System.EventHandler(this.trackBarBase_ValueChanged);
            // 
            // btnGoBase
            // 
            this.btnGoBase.Location = new System.Drawing.Point(621, 208);
            this.btnGoBase.Name = "btnGoBase";
            this.btnGoBase.Size = new System.Drawing.Size(90, 34);
            this.btnGoBase.TabIndex = 28;
            this.btnGoBase.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnGoBase.Values.Text = "GO (0)";
            this.btnGoBase.Click += new System.EventHandler(this.btnGoBase_Click);
            // 
            // btnGo1
            // 
            this.btnGo1.Location = new System.Drawing.Point(621, 128);
            this.btnGo1.Name = "btnGo1";
            this.btnGo1.Size = new System.Drawing.Size(90, 34);
            this.btnGo1.TabIndex = 29;
            this.btnGo1.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnGo1.Values.Text = "GO (0)";
            this.btnGo1.Click += new System.EventHandler(this.btnGo1_Click);
            // 
            // btnGo2
            // 
            this.btnGo2.Location = new System.Drawing.Point(621, 168);
            this.btnGo2.Name = "btnGo2";
            this.btnGo2.Size = new System.Drawing.Size(90, 34);
            this.btnGo2.TabIndex = 30;
            this.btnGo2.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnGo2.Values.Text = "GO (0)";
            this.btnGo2.Click += new System.EventHandler(this.btnGo2_Click);
            // 
            // btnGo3
            // 
            this.btnGo3.Location = new System.Drawing.Point(621, 92);
            this.btnGo3.Name = "btnGo3";
            this.btnGo3.Size = new System.Drawing.Size(90, 30);
            this.btnGo3.TabIndex = 31;
            this.btnGo3.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnGo3.Values.Text = "GO (0)";
            this.btnGo3.Click += new System.EventHandler(this.btnGo3_Click);
            // 
            // btnGoAll
            // 
            this.btnGoAll.Location = new System.Drawing.Point(717, 92);
            this.btnGoAll.Name = "btnGoAll";
            this.btnGoAll.Size = new System.Drawing.Size(68, 150);
            this.btnGoAll.TabIndex = 32;
            this.btnGoAll.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnGoAll.Values.Text = "GO ALL";
            this.btnGoAll.Click += new System.EventHandler(this.btnGoAll_Click);
            // 
            // kryptonButton1
            // 
            this.kryptonButton1.Location = new System.Drawing.Point(791, 92);
            this.kryptonButton1.Name = "kryptonButton1";
            this.kryptonButton1.Size = new System.Drawing.Size(68, 150);
            this.kryptonButton1.TabIndex = 33;
            this.kryptonButton1.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.kryptonButton1.Values.Text = "HOME";
            this.kryptonButton1.Click += new System.EventHandler(this.kryptonButton1_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1576, 683);
            this.Controls.Add(this.kryptonButton1);
            this.Controls.Add(this.btnGoAll);
            this.Controls.Add(this.btnGo3);
            this.Controls.Add(this.btnGo2);
            this.Controls.Add(this.btnGo1);
            this.Controls.Add(this.btnGoBase);
            this.Controls.Add(this.trackBarBase);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.trackBar3);
            this.Controls.Add(this.gpMagnetState);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listViewLog);
            this.Controls.Add(this.button6);
            this.Name = "FrmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gpMagnetState.Panel)).EndInit();
            this.gpMagnetState.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gpMagnetState)).EndInit();
            this.gpMagnetState.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ListBox listBoxPositions;
        private System.Windows.Forms.ListView listViewLog;
        private System.Windows.Forms.ColumnHeader colTimestamp;
        private System.Windows.Forms.ColumnHeader colMessage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private Krypton.Toolkit.KryptonButton magnetON;
        private Krypton.Toolkit.KryptonButton magnetOFF;
        private Krypton.Toolkit.KryptonGroupBox gpMagnetState;
        private Krypton.Toolkit.KryptonButton btnSavePosition;
        private Krypton.Toolkit.KryptonButton btnPlayPosition;
        private Krypton.Toolkit.KryptonTrackBar trackBar3;
        private Krypton.Toolkit.KryptonTrackBar trackBar2;
        private Krypton.Toolkit.KryptonTrackBar trackBar1;
        private Krypton.Toolkit.KryptonTrackBar trackBarBase;
        private Krypton.Toolkit.KryptonButton btnGoBase;
        private Krypton.Toolkit.KryptonButton btnGo1;
        private Krypton.Toolkit.KryptonButton btnGo2;
        private Krypton.Toolkit.KryptonButton btnGo3;
        private Krypton.Toolkit.KryptonButton btnGoAll;
        private Krypton.Toolkit.KryptonButton kryptonButton1;
        private Krypton.Toolkit.KryptonButton btnClear;
    }
}

