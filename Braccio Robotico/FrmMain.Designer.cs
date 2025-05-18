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
            this.listBoxPositions = new System.Windows.Forms.ListBox();
            this.listViewLog = new System.Windows.Forms.ListView();
            this.colTimestamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSalvaSetCorrente = new Krypton.Toolkit.KryptonButton();
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
            this.btnGoAll = new Krypton.Toolkit.KryptonButton();
            this.kryptonButton1 = new Krypton.Toolkit.KryptonButton();
            this.btnConfig = new Krypton.Toolkit.KryptonButton();
            this.comboBoxComPorts = new Krypton.Toolkit.KryptonComboBox();
            this.btnConnect = new Krypton.Toolkit.KryptonButton();
            this.btnDisconnect = new Krypton.Toolkit.KryptonButton();
            this.GrpComPorts = new Krypton.Toolkit.KryptonGroupBox();
            this.GrpSavePosition = new Krypton.Toolkit.KryptonGroupBox();
            this.GrpLog = new Krypton.Toolkit.KryptonGroupBox();
            this.btnGestionePosizioni = new Krypton.Toolkit.KryptonButton();
            this.trackBarNumeric3 = new Krypton.Toolkit.KryptonNumericUpDown();
            this.trackBarNumeric1 = new Krypton.Toolkit.KryptonNumericUpDown();
            this.trackBarNumeric2 = new Krypton.Toolkit.KryptonNumericUpDown();
            this.trackBarNumericBase = new Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonGroupBox1 = new Krypton.Toolkit.KryptonGroupBox();
            this.pnlSimulation = new System.Windows.Forms.Panel();
            this.BtnSetHome = new Krypton.Toolkit.KryptonButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gpMagnetState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gpMagnetState.Panel)).BeginInit();
            this.gpMagnetState.Panel.SuspendLayout();
            this.gpMagnetState.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxComPorts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GrpComPorts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GrpComPorts.Panel)).BeginInit();
            this.GrpComPorts.Panel.SuspendLayout();
            this.GrpComPorts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GrpSavePosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GrpSavePosition.Panel)).BeginInit();
            this.GrpSavePosition.Panel.SuspendLayout();
            this.GrpSavePosition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GrpLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GrpLog.Panel)).BeginInit();
            this.GrpLog.Panel.SuspendLayout();
            this.GrpLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1.Panel)).BeginInit();
            this.kryptonGroupBox1.Panel.SuspendLayout();
            this.kryptonGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxPositions
            // 
            this.listBoxPositions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxPositions.FormattingEnabled = true;
            this.listBoxPositions.Location = new System.Drawing.Point(6, 45);
            this.listBoxPositions.Name = "listBoxPositions";
            this.listBoxPositions.Size = new System.Drawing.Size(306, 589);
            this.listBoxPositions.TabIndex = 16;
            // 
            // listViewLog
            // 
            this.listViewLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTimestamp,
            this.colMessage});
            this.listViewLog.FullRowSelect = true;
            this.listViewLog.GridLines = true;
            this.listViewLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewLog.HideSelection = false;
            this.listViewLog.Location = new System.Drawing.Point(6, 6);
            this.listViewLog.Name = "listViewLog";
            this.listViewLog.Size = new System.Drawing.Size(478, 626);
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
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSalvaSetCorrente);
            this.panel1.Controls.Add(this.btnClear);
            this.panel1.Controls.Add(this.btnSavePosition);
            this.panel1.Controls.Add(this.btnPlayPosition);
            this.panel1.Location = new System.Drawing.Point(6, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(305, 31);
            this.panel1.TabIndex = 17;
            // 
            // btnSalvaSetCorrente
            // 
            this.btnSalvaSetCorrente.Location = new System.Drawing.Point(154, 3);
            this.btnSalvaSetCorrente.Name = "btnSalvaSetCorrente";
            this.btnSalvaSetCorrente.Size = new System.Drawing.Size(69, 25);
            this.btnSalvaSetCorrente.TabIndex = 27;
            this.btnSalvaSetCorrente.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnSalvaSetCorrente.Values.Text = "Save Set";
            this.btnSalvaSetCorrente.Click += new System.EventHandler(this.btnSalvaSetCorrente_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(3, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(74, 25);
            this.btnClear.TabIndex = 26;
            this.btnClear.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnClear.Values.Text = "Clear All";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSavePosition
            // 
            this.btnSavePosition.Location = new System.Drawing.Point(83, 3);
            this.btnSavePosition.Name = "btnSavePosition";
            this.btnSavePosition.Size = new System.Drawing.Size(65, 25);
            this.btnSavePosition.TabIndex = 24;
            this.btnSavePosition.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnSavePosition.Values.Text = "Save Pos.";
            this.btnSavePosition.Click += new System.EventHandler(this.btnSavePosition_Click);
            // 
            // btnPlayPosition
            // 
            this.btnPlayPosition.Enabled = false;
            this.btnPlayPosition.Location = new System.Drawing.Point(229, 3);
            this.btnPlayPosition.Name = "btnPlayPosition";
            this.btnPlayPosition.Size = new System.Drawing.Size(72, 25);
            this.btnPlayPosition.TabIndex = 25;
            this.btnPlayPosition.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnPlayPosition.Values.Text = "Play";
            this.btnPlayPosition.Click += new System.EventHandler(this.btnPlayPosition_Click);
            // 
            // magnetON
            // 
            this.magnetON.Location = new System.Drawing.Point(11, 15);
            this.magnetON.Name = "magnetON";
            this.magnetON.Size = new System.Drawing.Size(56, 22);
            this.magnetON.TabIndex = 21;
            this.magnetON.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.magnetON.Values.Text = "ON";
            this.magnetON.Click += new System.EventHandler(this.magnetON_Click);
            // 
            // magnetOFF
            // 
            this.magnetOFF.Enabled = false;
            this.magnetOFF.Location = new System.Drawing.Point(75, 15);
            this.magnetOFF.Name = "magnetOFF";
            this.magnetOFF.Size = new System.Drawing.Size(56, 22);
            this.magnetOFF.TabIndex = 22;
            this.magnetOFF.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.magnetOFF.Values.Text = "OFF";
            this.magnetOFF.Click += new System.EventHandler(this.magnetOFF_Click);
            // 
            // gpMagnetState
            // 
            this.gpMagnetState.CaptionStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.gpMagnetState.Location = new System.Drawing.Point(388, 8);
            // 
            // gpMagnetState.Panel
            // 
            this.gpMagnetState.Panel.Controls.Add(this.magnetON);
            this.gpMagnetState.Panel.Controls.Add(this.magnetOFF);
            this.gpMagnetState.Size = new System.Drawing.Size(144, 77);
            this.gpMagnetState.TabIndex = 23;
            this.gpMagnetState.Values.Heading = "Magnet state (OFF)";
            // 
            // trackBar3
            // 
            this.trackBar3.Location = new System.Drawing.Point(12, 92);
            this.trackBar3.Maximum = 180;
            this.trackBar3.Minimum = -180;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(686, 34);
            this.trackBar3.StateNormal.Tick.Color1 = System.Drawing.Color.White;
            this.trackBar3.StateNormal.Tick.Color2 = System.Drawing.Color.Empty;
            this.trackBar3.StateNormal.Tick.Color3 = System.Drawing.Color.Empty;
            this.trackBar3.StateNormal.Tick.Color4 = System.Drawing.Color.Empty;
            this.trackBar3.StateNormal.Tick.Color5 = System.Drawing.Color.Empty;
            this.trackBar3.TabIndex = 24;
            this.trackBar3.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar3.TrackBarSize = Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.trackBar3.VolumeControl = true;
            this.trackBar3.ValueChanged += new System.EventHandler(this.trackBar3_ValueChanged);
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(12, 168);
            this.trackBar2.Maximum = 180;
            this.trackBar2.Minimum = -180;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(686, 34);
            this.trackBar2.StateNormal.Tick.Color1 = System.Drawing.Color.White;
            this.trackBar2.StateNormal.Tick.Color2 = System.Drawing.Color.Empty;
            this.trackBar2.StateNormal.Tick.Color3 = System.Drawing.Color.Empty;
            this.trackBar2.StateNormal.Tick.Color4 = System.Drawing.Color.Empty;
            this.trackBar2.StateNormal.Tick.Color5 = System.Drawing.Color.Empty;
            this.trackBar2.TabIndex = 25;
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar2.TrackBarSize = Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.trackBar2.VolumeControl = true;
            this.trackBar2.ValueChanged += new System.EventHandler(this.trackBar2_ValueChanged);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(11, 128);
            this.trackBar1.Maximum = 180;
            this.trackBar1.Minimum = -180;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(686, 34);
            this.trackBar1.StateNormal.Tick.Color1 = System.Drawing.Color.White;
            this.trackBar1.StateNormal.Tick.Color2 = System.Drawing.Color.Empty;
            this.trackBar1.StateNormal.Tick.Color3 = System.Drawing.Color.Empty;
            this.trackBar1.StateNormal.Tick.Color4 = System.Drawing.Color.Empty;
            this.trackBar1.StateNormal.Tick.Color5 = System.Drawing.Color.Empty;
            this.trackBar1.TabIndex = 26;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar1.TrackBarSize = Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.trackBar1.VolumeControl = true;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // trackBarBase
            // 
            this.trackBarBase.Location = new System.Drawing.Point(12, 208);
            this.trackBarBase.Maximum = 180;
            this.trackBarBase.Minimum = -180;
            this.trackBarBase.Name = "trackBarBase";
            this.trackBarBase.Size = new System.Drawing.Size(686, 34);
            this.trackBarBase.StateNormal.Tick.Color1 = System.Drawing.Color.White;
            this.trackBarBase.StateNormal.Tick.Color2 = System.Drawing.Color.Empty;
            this.trackBarBase.StateNormal.Tick.Color3 = System.Drawing.Color.Empty;
            this.trackBarBase.StateNormal.Tick.Color4 = System.Drawing.Color.Empty;
            this.trackBarBase.StateNormal.Tick.Color5 = System.Drawing.Color.Empty;
            this.trackBarBase.TabIndex = 27;
            this.trackBarBase.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarBase.TrackBarSize = Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.trackBarBase.VolumeControl = true;
            this.trackBarBase.ValueChanged += new System.EventHandler(this.trackBarBase_ValueChanged);
            // 
            // btnGoAll
            // 
            this.btnGoAll.Location = new System.Drawing.Point(781, 91);
            this.btnGoAll.Name = "btnGoAll";
            this.btnGoAll.Size = new System.Drawing.Size(68, 66);
            this.btnGoAll.TabIndex = 32;
            this.btnGoAll.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnGoAll.Values.Text = "GO ALL";
            this.btnGoAll.Click += new System.EventHandler(this.btnGoAll_Click);
            // 
            // kryptonButton1
            // 
            this.kryptonButton1.Location = new System.Drawing.Point(781, 168);
            this.kryptonButton1.Name = "kryptonButton1";
            this.kryptonButton1.Size = new System.Drawing.Size(68, 70);
            this.kryptonButton1.TabIndex = 33;
            this.kryptonButton1.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.kryptonButton1.Values.Text = "Go Home";
            this.kryptonButton1.Click += new System.EventHandler(this.kryptonButton1_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.Location = new System.Drawing.Point(781, 19);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(68, 66);
            this.btnConfig.TabIndex = 38;
            this.btnConfig.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnConfig.Values.Text = "Config";
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // comboBoxComPorts
            // 
            this.comboBoxComPorts.DropBackStyle = Krypton.Toolkit.PaletteBackStyle.ButtonCluster;
            this.comboBoxComPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxComPorts.DropDownWidth = 127;
            this.comboBoxComPorts.Location = new System.Drawing.Point(16, 15);
            this.comboBoxComPorts.Name = "comboBoxComPorts";
            this.comboBoxComPorts.Size = new System.Drawing.Size(127, 22);
            this.comboBoxComPorts.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.comboBoxComPorts.TabIndex = 39;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(149, 15);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(104, 22);
            this.btnConnect.TabIndex = 40;
            this.btnConnect.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnConnect.Values.Text = "Connect...";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(259, 15);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(95, 22);
            this.btnDisconnect.TabIndex = 41;
            this.btnDisconnect.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnDisconnect.Values.Text = "Disconnect...";
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // GrpComPorts
            // 
            this.GrpComPorts.CaptionOverlap = 0.6D;
            this.GrpComPorts.CaptionStyle = Krypton.Toolkit.LabelStyle.BoldPanel;
            this.GrpComPorts.Location = new System.Drawing.Point(11, 8);
            // 
            // GrpComPorts.Panel
            // 
            this.GrpComPorts.Panel.Controls.Add(this.comboBoxComPorts);
            this.GrpComPorts.Panel.Controls.Add(this.btnDisconnect);
            this.GrpComPorts.Panel.Controls.Add(this.btnConnect);
            this.GrpComPorts.Size = new System.Drawing.Size(371, 77);
            this.GrpComPorts.TabIndex = 42;
            this.GrpComPorts.Values.Heading = "COM Ports";
            // 
            // GrpSavePosition
            // 
            this.GrpSavePosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.GrpSavePosition.CaptionStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.GrpSavePosition.Location = new System.Drawing.Point(860, 12);
            // 
            // GrpSavePosition.Panel
            // 
            this.GrpSavePosition.Panel.Controls.Add(this.listBoxPositions);
            this.GrpSavePosition.Panel.Controls.Add(this.panel1);
            this.GrpSavePosition.Size = new System.Drawing.Size(322, 659);
            this.GrpSavePosition.TabIndex = 43;
            this.GrpSavePosition.Values.Heading = "Save Positions";
            // 
            // GrpLog
            // 
            this.GrpLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GrpLog.CaptionStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.GrpLog.Location = new System.Drawing.Point(1188, 12);
            // 
            // GrpLog.Panel
            // 
            this.GrpLog.Panel.Controls.Add(this.listViewLog);
            this.GrpLog.Size = new System.Drawing.Size(493, 659);
            this.GrpLog.TabIndex = 45;
            this.GrpLog.Values.Heading = "Logs";
            // 
            // btnGestionePosizioni
            // 
            this.btnGestionePosizioni.Location = new System.Drawing.Point(538, 20);
            this.btnGestionePosizioni.Name = "btnGestionePosizioni";
            this.btnGestionePosizioni.Size = new System.Drawing.Size(121, 66);
            this.btnGestionePosizioni.TabIndex = 46;
            this.btnGestionePosizioni.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnGestionePosizioni.Values.Text = "Position Manager";
            this.btnGestionePosizioni.Click += new System.EventHandler(this.btnGestionePosizioni_Click);
            // 
            // trackBarNumeric3
            // 
            this.trackBarNumeric3.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.trackBarNumeric3.Location = new System.Drawing.Point(704, 92);
            this.trackBarNumeric3.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.trackBarNumeric3.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.trackBarNumeric3.Name = "trackBarNumeric3";
            this.trackBarNumeric3.Size = new System.Drawing.Size(71, 30);
            this.trackBarNumeric3.StateActive.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackBarNumeric3.TabIndex = 48;
            this.trackBarNumeric3.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.trackBarNumeric3.ValueChanged += new System.EventHandler(this.trackBarNumeric3_ValueChanged);
            // 
            // trackBarNumeric1
            // 
            this.trackBarNumeric1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.trackBarNumeric1.Location = new System.Drawing.Point(704, 128);
            this.trackBarNumeric1.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.trackBarNumeric1.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.trackBarNumeric1.Name = "trackBarNumeric1";
            this.trackBarNumeric1.Size = new System.Drawing.Size(71, 30);
            this.trackBarNumeric1.StateActive.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackBarNumeric1.TabIndex = 49;
            this.trackBarNumeric1.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.trackBarNumeric1.ValueChanged += new System.EventHandler(this.trackBarNumeric1_ValueChanged);
            // 
            // trackBarNumeric2
            // 
            this.trackBarNumeric2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.trackBarNumeric2.Location = new System.Drawing.Point(704, 168);
            this.trackBarNumeric2.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.trackBarNumeric2.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.trackBarNumeric2.Name = "trackBarNumeric2";
            this.trackBarNumeric2.Size = new System.Drawing.Size(71, 30);
            this.trackBarNumeric2.StateActive.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackBarNumeric2.TabIndex = 50;
            this.trackBarNumeric2.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.trackBarNumeric2.ValueChanged += new System.EventHandler(this.trackBarNumeric2_ValueChanged);
            // 
            // trackBarNumericBase
            // 
            this.trackBarNumericBase.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.trackBarNumericBase.Location = new System.Drawing.Point(704, 208);
            this.trackBarNumericBase.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.trackBarNumericBase.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.trackBarNumericBase.Name = "trackBarNumericBase";
            this.trackBarNumericBase.Size = new System.Drawing.Size(71, 30);
            this.trackBarNumericBase.StateActive.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackBarNumericBase.TabIndex = 51;
            this.trackBarNumericBase.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.trackBarNumericBase.ValueChanged += new System.EventHandler(this.trackBarNumericBase_ValueChanged);
            // 
            // kryptonGroupBox1
            // 
            this.kryptonGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.kryptonGroupBox1.CaptionStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.kryptonGroupBox1.Location = new System.Drawing.Point(12, 248);
            // 
            // kryptonGroupBox1.Panel
            // 
            this.kryptonGroupBox1.Panel.Controls.Add(this.pnlSimulation);
            this.kryptonGroupBox1.Size = new System.Drawing.Size(842, 423);
            this.kryptonGroupBox1.TabIndex = 53;
            this.kryptonGroupBox1.Values.Heading = "Graph";
            // 
            // pnlSimulation
            // 
            this.pnlSimulation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSimulation.Location = new System.Drawing.Point(0, 0);
            this.pnlSimulation.Name = "pnlSimulation";
            this.pnlSimulation.Size = new System.Drawing.Size(838, 401);
            this.pnlSimulation.TabIndex = 54;
            // 
            // BtnSetHome
            // 
            this.BtnSetHome.Location = new System.Drawing.Point(665, 20);
            this.BtnSetHome.Name = "BtnSetHome";
            this.BtnSetHome.Size = new System.Drawing.Size(68, 66);
            this.BtnSetHome.TabIndex = 54;
            this.BtnSetHome.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.BtnSetHome.Values.Text = "Set Home";
            this.BtnSetHome.Click += new System.EventHandler(this.BtnSetHome_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1693, 683);
            this.Controls.Add(this.BtnSetHome);
            this.Controls.Add(this.kryptonGroupBox1);
            this.Controls.Add(this.trackBarNumericBase);
            this.Controls.Add(this.trackBarNumeric2);
            this.Controls.Add(this.trackBarNumeric1);
            this.Controls.Add(this.trackBarNumeric3);
            this.Controls.Add(this.btnGestionePosizioni);
            this.Controls.Add(this.GrpLog);
            this.Controls.Add(this.GrpSavePosition);
            this.Controls.Add(this.GrpComPorts);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.kryptonButton1);
            this.Controls.Add(this.btnGoAll);
            this.Controls.Add(this.trackBarBase);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.trackBar3);
            this.Controls.Add(this.gpMagnetState);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Robotic Arm Experiment";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gpMagnetState.Panel)).EndInit();
            this.gpMagnetState.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gpMagnetState)).EndInit();
            this.gpMagnetState.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxComPorts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GrpComPorts.Panel)).EndInit();
            this.GrpComPorts.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GrpComPorts)).EndInit();
            this.GrpComPorts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GrpSavePosition.Panel)).EndInit();
            this.GrpSavePosition.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GrpSavePosition)).EndInit();
            this.GrpSavePosition.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GrpLog.Panel)).EndInit();
            this.GrpLog.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GrpLog)).EndInit();
            this.GrpLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1.Panel)).EndInit();
            this.kryptonGroupBox1.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).EndInit();
            this.kryptonGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox listBoxPositions;
        private System.Windows.Forms.ListView listViewLog;
        private System.Windows.Forms.ColumnHeader colTimestamp;
        private System.Windows.Forms.ColumnHeader colMessage;
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
        private Krypton.Toolkit.KryptonButton btnGoAll;
        private Krypton.Toolkit.KryptonButton kryptonButton1;
        private Krypton.Toolkit.KryptonButton btnClear;
        private Krypton.Toolkit.KryptonButton btnConfig;
        private Krypton.Toolkit.KryptonComboBox comboBoxComPorts;
        private Krypton.Toolkit.KryptonButton btnConnect;
        private Krypton.Toolkit.KryptonButton btnDisconnect;
        private Krypton.Toolkit.KryptonGroupBox GrpComPorts;
        private Krypton.Toolkit.KryptonGroupBox GrpSavePosition;
        private Krypton.Toolkit.KryptonGroupBox GrpLog;
        private Krypton.Toolkit.KryptonButton btnGestionePosizioni;
        private Krypton.Toolkit.KryptonButton btnSalvaSetCorrente;
        private Krypton.Toolkit.KryptonNumericUpDown trackBarNumeric3;
        private Krypton.Toolkit.KryptonNumericUpDown trackBarNumeric1;
        private Krypton.Toolkit.KryptonNumericUpDown trackBarNumeric2;
        private Krypton.Toolkit.KryptonNumericUpDown trackBarNumericBase;
        private Krypton.Toolkit.KryptonGroupBox kryptonGroupBox1;
        private System.Windows.Forms.Panel pnlSimulation;
        private Krypton.Toolkit.KryptonButton BtnSetHome;
    }
}

