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
            this.ckPlayLoop = new System.Windows.Forms.CheckBox();
            this.btnSalvaSetCorrente = new Krypton.Toolkit.KryptonButton();
            this.btnClear = new Krypton.Toolkit.KryptonButton();
            this.btnSavePosition = new Krypton.Toolkit.KryptonButton();
            this.btnPlayPosition = new Krypton.Toolkit.KryptonButton();
            this.magnetON = new Krypton.Toolkit.KryptonButton();
            this.magnetOFF = new Krypton.Toolkit.KryptonButton();
            this.gpMagnetState = new Krypton.Toolkit.KryptonGroupBox();
            this.trackBar3 = new Krypton.Toolkit.KryptonTrackBar();
            this.trackBar1 = new Krypton.Toolkit.KryptonTrackBar();
            this.trackBar4 = new Krypton.Toolkit.KryptonTrackBar();
            this.trackBar2 = new Krypton.Toolkit.KryptonTrackBar();
            this.btnGoAll = new Krypton.Toolkit.KryptonButton();
            this.kryptonButton1 = new Krypton.Toolkit.KryptonButton();
            this.btnConfig = new Krypton.Toolkit.KryptonButton();
            this.comboBoxComPorts = new Krypton.Toolkit.KryptonComboBox();
            this.btnConnect = new Krypton.Toolkit.KryptonButton();
            this.btnDisconnect = new Krypton.Toolkit.KryptonButton();
            this.GrpComPorts = new Krypton.Toolkit.KryptonGroupBox();
            this.GrpSavePosition = new Krypton.Toolkit.KryptonGroupBox();
            this.GrpLog = new Krypton.Toolkit.KryptonGroupBox();
            this.btnClearLogs = new Krypton.Toolkit.KryptonButton();
            this.txtToSend = new System.Windows.Forms.TextBox();
            this.btnSend = new Krypton.Toolkit.KryptonButton();
            this.btnGestionePosizioni = new Krypton.Toolkit.KryptonButton();
            this.trackBarNumeric3 = new Krypton.Toolkit.KryptonNumericUpDown();
            this.trackBarNumeric4 = new Krypton.Toolkit.KryptonNumericUpDown();
            this.trackBarNumeric1 = new Krypton.Toolkit.KryptonNumericUpDown();
            this.trackBarNumeric2 = new Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonGroupBox1 = new Krypton.Toolkit.KryptonGroupBox();
            this.pnlSimulation = new System.Windows.Forms.Panel();
            this.BtnSetHome = new Krypton.Toolkit.KryptonButton();
            this.btnESDisabled = new Krypton.Toolkit.KryptonButton();
            this.btnGoHome3 = new Krypton.Toolkit.KryptonButton();
            this.btnGoHome4 = new Krypton.Toolkit.KryptonButton();
            this.btnGoHome1 = new Krypton.Toolkit.KryptonButton();
            this.btnGoHome2 = new Krypton.Toolkit.KryptonButton();
            this.btnSTOP = new Krypton.Toolkit.KryptonButton();
            this.btnImport = new Krypton.Toolkit.KryptonButton();
            this.grpMotor = new Krypton.Toolkit.KryptonGroupBox();
            this.btnMotorON = new Krypton.Toolkit.KryptonButton();
            this.btnMotorOFF = new Krypton.Toolkit.KryptonButton();
            this.trackBarP = new Krypton.Toolkit.KryptonTrackBar();
            this.kryptonButton2 = new Krypton.Toolkit.KryptonButton();
            this.button1 = new System.Windows.Forms.Button();
            this.btnTestEndStop = new Krypton.Toolkit.KryptonButton();
            this.END1 = new Krypton.Toolkit.KryptonButton();
            this.END2 = new Krypton.Toolkit.KryptonButton();
            this.END3 = new Krypton.Toolkit.KryptonButton();
            this.END4 = new Krypton.Toolkit.KryptonButton();
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
            ((System.ComponentModel.ISupportInitialize)(this.grpMotor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpMotor.Panel)).BeginInit();
            this.grpMotor.Panel.SuspendLayout();
            this.grpMotor.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxPositions
            // 
            this.listBoxPositions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxPositions.FormattingEnabled = true;
            this.listBoxPositions.Location = new System.Drawing.Point(6, 84);
            this.listBoxPositions.Name = "listBoxPositions";
            this.listBoxPositions.Size = new System.Drawing.Size(305, 784);
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
            this.listViewLog.Location = new System.Drawing.Point(6, 45);
            this.listViewLog.Name = "listViewLog";
            this.listViewLog.Size = new System.Drawing.Size(524, 828);
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
            this.panel1.Controls.Add(this.ckPlayLoop);
            this.panel1.Controls.Add(this.btnSalvaSetCorrente);
            this.panel1.Controls.Add(this.btnClear);
            this.panel1.Controls.Add(this.btnSavePosition);
            this.panel1.Controls.Add(this.btnPlayPosition);
            this.panel1.Location = new System.Drawing.Point(6, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(305, 63);
            this.panel1.TabIndex = 17;
            // 
            // ckPlayLoop
            // 
            this.ckPlayLoop.AutoSize = true;
            this.ckPlayLoop.Location = new System.Drawing.Point(4, 36);
            this.ckPlayLoop.Name = "ckPlayLoop";
            this.ckPlayLoop.Size = new System.Drawing.Size(73, 17);
            this.ckPlayLoop.TabIndex = 28;
            this.ckPlayLoop.Text = "Play Loop";
            this.ckPlayLoop.UseVisualStyleBackColor = true;
            this.ckPlayLoop.CheckedChanged += new System.EventHandler(this.ckPlayLoop_CheckedChanged);
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
            this.trackBar3.Location = new System.Drawing.Point(11, 128);
            this.trackBar3.Maximum = 360;
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
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(12, 205);
            this.trackBar1.Maximum = 360;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(686, 34);
            this.trackBar1.StateNormal.Tick.Color1 = System.Drawing.Color.White;
            this.trackBar1.StateNormal.Tick.Color2 = System.Drawing.Color.Empty;
            this.trackBar1.StateNormal.Tick.Color3 = System.Drawing.Color.Empty;
            this.trackBar1.StateNormal.Tick.Color4 = System.Drawing.Color.Empty;
            this.trackBar1.StateNormal.Tick.Color5 = System.Drawing.Color.Empty;
            this.trackBar1.TabIndex = 25;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar1.TrackBarSize = Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.trackBar1.VolumeControl = true;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // trackBar4
            // 
            this.trackBar4.Location = new System.Drawing.Point(11, 92);
            this.trackBar4.Maximum = 360;
            this.trackBar4.Name = "trackBar4";
            this.trackBar4.Size = new System.Drawing.Size(686, 34);
            this.trackBar4.StateNormal.Tick.Color1 = System.Drawing.Color.White;
            this.trackBar4.StateNormal.Tick.Color2 = System.Drawing.Color.Empty;
            this.trackBar4.StateNormal.Tick.Color3 = System.Drawing.Color.Empty;
            this.trackBar4.StateNormal.Tick.Color4 = System.Drawing.Color.Empty;
            this.trackBar4.StateNormal.Tick.Color5 = System.Drawing.Color.Empty;
            this.trackBar4.TabIndex = 26;
            this.trackBar4.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar4.TrackBarSize = Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.trackBar4.VolumeControl = true;
            this.trackBar4.ValueChanged += new System.EventHandler(this.trackBar4_ValueChanged);
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(11, 167);
            this.trackBar2.Maximum = 360;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(686, 34);
            this.trackBar2.StateNormal.Tick.Color1 = System.Drawing.Color.White;
            this.trackBar2.StateNormal.Tick.Color2 = System.Drawing.Color.Empty;
            this.trackBar2.StateNormal.Tick.Color3 = System.Drawing.Color.Empty;
            this.trackBar2.StateNormal.Tick.Color4 = System.Drawing.Color.Empty;
            this.trackBar2.StateNormal.Tick.Color5 = System.Drawing.Color.Empty;
            this.trackBar2.TabIndex = 27;
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar2.TrackBarSize = Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.trackBar2.VolumeControl = true;
            this.trackBar2.ValueChanged += new System.EventHandler(this.trackBar2_ValueChanged);
            // 
            // btnGoAll
            // 
            this.btnGoAll.Location = new System.Drawing.Point(860, 90);
            this.btnGoAll.Name = "btnGoAll";
            this.btnGoAll.Size = new System.Drawing.Size(109, 108);
            this.btnGoAll.StateCommon.Back.Color1 = System.Drawing.Color.Lime;
            this.btnGoAll.StateCommon.Back.Color2 = System.Drawing.Color.Lime;
            this.btnGoAll.StateCommon.Border.Rounding = 100F;
            this.btnGoAll.StateCommon.Border.Width = 2;
            this.btnGoAll.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.btnGoAll.StateCommon.Content.ShortText.Color2 = System.Drawing.Color.White;
            this.btnGoAll.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGoAll.TabIndex = 32;
            this.btnGoAll.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnGoAll.Values.Text = "PLAY";
            this.btnGoAll.Click += new System.EventHandler(this.btnGoAll_Click);
            // 
            // kryptonButton1
            // 
            this.kryptonButton1.Location = new System.Drawing.Point(1095, 55);
            this.kryptonButton1.Name = "kryptonButton1";
            this.kryptonButton1.Size = new System.Drawing.Size(151, 31);
            this.kryptonButton1.TabIndex = 33;
            this.kryptonButton1.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.kryptonButton1.Values.Text = "Go Home";
            this.kryptonButton1.Click += new System.EventHandler(this.kryptonButton1_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.Location = new System.Drawing.Point(813, 20);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(117, 66);
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
            this.GrpSavePosition.Location = new System.Drawing.Point(1254, 12);
            // 
            // GrpSavePosition.Panel
            // 
            this.GrpSavePosition.Panel.Controls.Add(this.listBoxPositions);
            this.GrpSavePosition.Panel.Controls.Add(this.panel1);
            this.GrpSavePosition.Size = new System.Drawing.Size(321, 900);
            this.GrpSavePosition.TabIndex = 43;
            this.GrpSavePosition.Values.Heading = "Save Positions";
            // 
            // GrpLog
            // 
            this.GrpLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GrpLog.CaptionStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.GrpLog.Location = new System.Drawing.Point(1581, 12);
            // 
            // GrpLog.Panel
            // 
            this.GrpLog.Panel.Controls.Add(this.btnClearLogs);
            this.GrpLog.Panel.Controls.Add(this.txtToSend);
            this.GrpLog.Panel.Controls.Add(this.btnSend);
            this.GrpLog.Panel.Controls.Add(this.listViewLog);
            this.GrpLog.Size = new System.Drawing.Size(539, 900);
            this.GrpLog.TabIndex = 45;
            this.GrpLog.Values.Heading = "Logs";
            // 
            // btnClearLogs
            // 
            this.btnClearLogs.Location = new System.Drawing.Point(456, 14);
            this.btnClearLogs.Name = "btnClearLogs";
            this.btnClearLogs.Size = new System.Drawing.Size(74, 25);
            this.btnClearLogs.TabIndex = 67;
            this.btnClearLogs.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnClearLogs.Values.Text = "Clear";
            this.btnClearLogs.Click += new System.EventHandler(this.btnClearLogs_Click);
            // 
            // txtToSend
            // 
            this.txtToSend.Location = new System.Drawing.Point(6, 16);
            this.txtToSend.Name = "txtToSend";
            this.txtToSend.Size = new System.Drawing.Size(364, 20);
            this.txtToSend.TabIndex = 66;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(376, 14);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(74, 25);
            this.btnSend.TabIndex = 65;
            this.btnSend.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnSend.Values.Text = "Send";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
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
            this.trackBarNumeric3.Location = new System.Drawing.Point(704, 131);
            this.trackBarNumeric3.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.trackBarNumeric3.Minimum = new decimal(new int[] {
            360,
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
            // trackBarNumeric4
            // 
            this.trackBarNumeric4.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.trackBarNumeric4.Location = new System.Drawing.Point(704, 94);
            this.trackBarNumeric4.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.trackBarNumeric4.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.trackBarNumeric4.Name = "trackBarNumeric4";
            this.trackBarNumeric4.Size = new System.Drawing.Size(71, 30);
            this.trackBarNumeric4.StateActive.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackBarNumeric4.TabIndex = 49;
            this.trackBarNumeric4.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.trackBarNumeric4.ValueChanged += new System.EventHandler(this.trackBarNumeric4_ValueChanged);
            // 
            // trackBarNumeric1
            // 
            this.trackBarNumeric1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.trackBarNumeric1.Location = new System.Drawing.Point(704, 205);
            this.trackBarNumeric1.Maximum = new decimal(new int[] {
            360,
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
            this.trackBarNumeric1.TabIndex = 50;
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
            360,
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
            this.trackBarNumeric2.TabIndex = 51;
            this.trackBarNumeric2.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.trackBarNumeric2.ValueChanged += new System.EventHandler(this.trackBarNumeric2_ValueChanged);
            // 
            // kryptonGroupBox1
            // 
            this.kryptonGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.kryptonGroupBox1.CaptionStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.kryptonGroupBox1.Location = new System.Drawing.Point(12, 244);
            // 
            // kryptonGroupBox1.Panel
            // 
            this.kryptonGroupBox1.Panel.Controls.Add(this.pnlSimulation);
            this.kryptonGroupBox1.Size = new System.Drawing.Size(1236, 668);
            this.kryptonGroupBox1.TabIndex = 53;
            this.kryptonGroupBox1.Values.Heading = "Graph";
            // 
            // pnlSimulation
            // 
            this.pnlSimulation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSimulation.Location = new System.Drawing.Point(0, 0);
            this.pnlSimulation.Name = "pnlSimulation";
            this.pnlSimulation.Size = new System.Drawing.Size(1232, 646);
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
            // btnESDisabled
            // 
            this.btnESDisabled.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnESDisabled.Location = new System.Drawing.Point(739, 20);
            this.btnESDisabled.Name = "btnESDisabled";
            this.btnESDisabled.Size = new System.Drawing.Size(68, 66);
            this.btnESDisabled.StateCommon.Back.Color1 = System.Drawing.Color.White;
            this.btnESDisabled.TabIndex = 55;
            this.btnESDisabled.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnESDisabled.Values.Text = "E/S";
            this.btnESDisabled.Click += new System.EventHandler(this.btnESDisabled_Click);
            // 
            // btnGoHome3
            // 
            this.btnGoHome3.Location = new System.Drawing.Point(781, 132);
            this.btnGoHome3.Name = "btnGoHome3";
            this.btnGoHome3.Size = new System.Drawing.Size(73, 31);
            this.btnGoHome3.TabIndex = 56;
            this.btnGoHome3.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnGoHome3.Values.Text = "Go Home";
            this.btnGoHome3.Click += new System.EventHandler(this.btnGoHome3_Click);
            // 
            // btnGoHome4
            // 
            this.btnGoHome4.Location = new System.Drawing.Point(781, 95);
            this.btnGoHome4.Name = "btnGoHome4";
            this.btnGoHome4.Size = new System.Drawing.Size(73, 29);
            this.btnGoHome4.TabIndex = 57;
            this.btnGoHome4.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnGoHome4.Values.Text = "Go Home";
            this.btnGoHome4.Click += new System.EventHandler(this.btnGoHome4_Click);
            // 
            // btnGoHome1
            // 
            this.btnGoHome1.Location = new System.Drawing.Point(781, 208);
            this.btnGoHome1.Name = "btnGoHome1";
            this.btnGoHome1.Size = new System.Drawing.Size(73, 29);
            this.btnGoHome1.TabIndex = 58;
            this.btnGoHome1.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnGoHome1.Values.Text = "Go Home";
            this.btnGoHome1.Click += new System.EventHandler(this.btnGoHome1_Click);
            // 
            // btnGoHome2
            // 
            this.btnGoHome2.Location = new System.Drawing.Point(781, 171);
            this.btnGoHome2.Name = "btnGoHome2";
            this.btnGoHome2.Size = new System.Drawing.Size(73, 29);
            this.btnGoHome2.TabIndex = 59;
            this.btnGoHome2.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnGoHome2.Values.Text = "Go Home";
            this.btnGoHome2.Click += new System.EventHandler(this.btnGoHome2_Click);
            // 
            // btnSTOP
            // 
            this.btnSTOP.Location = new System.Drawing.Point(975, 90);
            this.btnSTOP.Name = "btnSTOP";
            this.btnSTOP.Size = new System.Drawing.Size(109, 108);
            this.btnSTOP.StateCommon.Back.Color1 = System.Drawing.Color.Red;
            this.btnSTOP.StateCommon.Back.Color2 = System.Drawing.Color.Red;
            this.btnSTOP.StateCommon.Border.Color1 = System.Drawing.Color.Black;
            this.btnSTOP.StateCommon.Border.Color2 = System.Drawing.Color.Black;
            this.btnSTOP.StateCommon.Border.Rounding = 100F;
            this.btnSTOP.StateCommon.Content.LongText.Color1 = System.Drawing.Color.White;
            this.btnSTOP.StateCommon.Content.LongText.Color2 = System.Drawing.Color.White;
            this.btnSTOP.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.btnSTOP.StateCommon.Content.ShortText.Color2 = System.Drawing.Color.White;
            this.btnSTOP.StateCommon.Content.ShortText.ColorStyle = Krypton.Toolkit.PaletteColorStyle.Solid;
            this.btnSTOP.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSTOP.TabIndex = 60;
            this.btnSTOP.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnSTOP.Values.Text = "STOP";
            this.btnSTOP.Click += new System.EventHandler(this.btnSTOP_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(1095, 20);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(151, 25);
            this.btnImport.TabIndex = 61;
            this.btnImport.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnImport.Values.Text = "Clear All";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // grpMotor
            // 
            this.grpMotor.CaptionStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.grpMotor.Location = new System.Drawing.Point(936, 9);
            // 
            // grpMotor.Panel
            // 
            this.grpMotor.Panel.Controls.Add(this.btnMotorON);
            this.grpMotor.Panel.Controls.Add(this.btnMotorOFF);
            this.grpMotor.Size = new System.Drawing.Size(144, 77);
            this.grpMotor.TabIndex = 62;
            this.grpMotor.Values.Heading = "Motor state (OFF)";
            // 
            // btnMotorON
            // 
            this.btnMotorON.Enabled = false;
            this.btnMotorON.Location = new System.Drawing.Point(11, 15);
            this.btnMotorON.Name = "btnMotorON";
            this.btnMotorON.Size = new System.Drawing.Size(56, 22);
            this.btnMotorON.TabIndex = 21;
            this.btnMotorON.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnMotorON.Values.Text = "ON";
            this.btnMotorON.Click += new System.EventHandler(this.btnMotorON_Click);
            // 
            // btnMotorOFF
            // 
            this.btnMotorOFF.Location = new System.Drawing.Point(75, 15);
            this.btnMotorOFF.Name = "btnMotorOFF";
            this.btnMotorOFF.Size = new System.Drawing.Size(56, 22);
            this.btnMotorOFF.TabIndex = 22;
            this.btnMotorOFF.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnMotorOFF.Values.Text = "OFF";
            this.btnMotorOFF.Click += new System.EventHandler(this.btnMotorOFF_Click);
            // 
            // trackBarP
            // 
            this.trackBarP.Location = new System.Drawing.Point(865, 205);
            this.trackBarP.Maximum = 120;
            this.trackBarP.Name = "trackBarP";
            this.trackBarP.Size = new System.Drawing.Size(165, 34);
            this.trackBarP.StateNormal.Tick.Color1 = System.Drawing.Color.White;
            this.trackBarP.StateNormal.Tick.Color2 = System.Drawing.Color.Empty;
            this.trackBarP.StateNormal.Tick.Color3 = System.Drawing.Color.Empty;
            this.trackBarP.StateNormal.Tick.Color4 = System.Drawing.Color.Empty;
            this.trackBarP.StateNormal.Tick.Color5 = System.Drawing.Color.Empty;
            this.trackBarP.TabIndex = 63;
            this.trackBarP.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarP.TrackBarSize = Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.trackBarP.VolumeControl = true;
            this.trackBarP.ValueChanged += new System.EventHandler(this.trackBarP_ValueChanged);
            // 
            // kryptonButton2
            // 
            this.kryptonButton2.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.kryptonButton2.Location = new System.Drawing.Point(1095, 97);
            this.kryptonButton2.Name = "kryptonButton2";
            this.kryptonButton2.Size = new System.Drawing.Size(151, 29);
            this.kryptonButton2.StateCommon.Back.Color1 = System.Drawing.Color.White;
            this.kryptonButton2.TabIndex = 64;
            this.kryptonButton2.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.kryptonButton2.Values.Text = "Stream ON/OFF";
            this.kryptonButton2.Click += new System.EventHandler(this.kryptonButton2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1036, 204);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(63, 35);
            this.button1.TabIndex = 65;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnTestEndStop
            // 
            this.btnTestEndStop.ButtonStyle = Krypton.Toolkit.ButtonStyle.Custom1;
            this.btnTestEndStop.Location = new System.Drawing.Point(1095, 134);
            this.btnTestEndStop.Name = "btnTestEndStop";
            this.btnTestEndStop.Size = new System.Drawing.Size(151, 29);
            this.btnTestEndStop.StateCommon.Back.Color1 = System.Drawing.Color.White;
            this.btnTestEndStop.TabIndex = 66;
            this.btnTestEndStop.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnTestEndStop.Values.Text = "Test EndStop";
            this.btnTestEndStop.Click += new System.EventHandler(this.btnTestEndStop_Click);
            // 
            // END1
            // 
            this.END1.Location = new System.Drawing.Point(1118, 167);
            this.END1.Name = "END1";
            this.END1.Size = new System.Drawing.Size(40, 40);
            this.END1.StateCommon.Back.Color1 = System.Drawing.Color.Lime;
            this.END1.StateCommon.Back.Color2 = System.Drawing.Color.Lime;
            this.END1.StateCommon.Border.Rounding = 100F;
            this.END1.StateCommon.Border.Width = 2;
            this.END1.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.END1.StateCommon.Content.ShortText.Color2 = System.Drawing.Color.White;
            this.END1.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.END1.TabIndex = 67;
            this.END1.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.END1.Values.Text = "1";
            // 
            // END2
            // 
            this.END2.Location = new System.Drawing.Point(1177, 167);
            this.END2.Name = "END2";
            this.END2.Size = new System.Drawing.Size(40, 40);
            this.END2.StateCommon.Back.Color1 = System.Drawing.Color.Lime;
            this.END2.StateCommon.Back.Color2 = System.Drawing.Color.Lime;
            this.END2.StateCommon.Border.Rounding = 100F;
            this.END2.StateCommon.Border.Width = 2;
            this.END2.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.END2.StateCommon.Content.ShortText.Color2 = System.Drawing.Color.White;
            this.END2.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.END2.TabIndex = 68;
            this.END2.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.END2.Values.Text = "1";
            // 
            // END3
            // 
            this.END3.Location = new System.Drawing.Point(1118, 208);
            this.END3.Name = "END3";
            this.END3.Size = new System.Drawing.Size(40, 40);
            this.END3.StateCommon.Back.Color1 = System.Drawing.Color.Lime;
            this.END3.StateCommon.Back.Color2 = System.Drawing.Color.Lime;
            this.END3.StateCommon.Border.Rounding = 100F;
            this.END3.StateCommon.Border.Width = 2;
            this.END3.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.END3.StateCommon.Content.ShortText.Color2 = System.Drawing.Color.White;
            this.END3.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.END3.TabIndex = 69;
            this.END3.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.END3.Values.Text = "1";
            // 
            // END4
            // 
            this.END4.Location = new System.Drawing.Point(1177, 208);
            this.END4.Name = "END4";
            this.END4.Size = new System.Drawing.Size(40, 40);
            this.END4.StateCommon.Back.Color1 = System.Drawing.Color.Lime;
            this.END4.StateCommon.Back.Color2 = System.Drawing.Color.Lime;
            this.END4.StateCommon.Border.Rounding = 100F;
            this.END4.StateCommon.Border.Width = 2;
            this.END4.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.END4.StateCommon.Content.ShortText.Color2 = System.Drawing.Color.White;
            this.END4.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.END4.TabIndex = 70;
            this.END4.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.END4.Values.Text = "1";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2132, 924);
            this.Controls.Add(this.END4);
            this.Controls.Add(this.END3);
            this.Controls.Add(this.END2);
            this.Controls.Add(this.END1);
            this.Controls.Add(this.btnTestEndStop);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.kryptonButton2);
            this.Controls.Add(this.trackBarP);
            this.Controls.Add(this.grpMotor);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnSTOP);
            this.Controls.Add(this.btnGoHome2);
            this.Controls.Add(this.btnGoAll);
            this.Controls.Add(this.btnGoHome1);
            this.Controls.Add(this.btnGoHome4);
            this.Controls.Add(this.btnGoHome3);
            this.Controls.Add(this.btnESDisabled);
            this.Controls.Add(this.BtnSetHome);
            this.Controls.Add(this.kryptonGroupBox1);
            this.Controls.Add(this.trackBarNumeric2);
            this.Controls.Add(this.trackBarNumeric1);
            this.Controls.Add(this.trackBarNumeric4);
            this.Controls.Add(this.trackBarNumeric3);
            this.Controls.Add(this.btnGestionePosizioni);
            this.Controls.Add(this.GrpLog);
            this.Controls.Add(this.GrpSavePosition);
            this.Controls.Add(this.GrpComPorts);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.kryptonButton1);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.trackBar4);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.trackBar3);
            this.Controls.Add(this.gpMagnetState);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Robotic Arm Experiment";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
            this.GrpLog.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GrpLog)).EndInit();
            this.GrpLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1.Panel)).EndInit();
            this.kryptonGroupBox1.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).EndInit();
            this.kryptonGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpMotor.Panel)).EndInit();
            this.grpMotor.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpMotor)).EndInit();
            this.grpMotor.ResumeLayout(false);
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
        private Krypton.Toolkit.KryptonTrackBar trackBar1;
        private Krypton.Toolkit.KryptonTrackBar trackBar4;
        private Krypton.Toolkit.KryptonTrackBar trackBar2;
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
        private Krypton.Toolkit.KryptonNumericUpDown trackBarNumeric4;
        private Krypton.Toolkit.KryptonNumericUpDown trackBarNumeric1;
        private Krypton.Toolkit.KryptonNumericUpDown trackBarNumeric2;
        private Krypton.Toolkit.KryptonGroupBox kryptonGroupBox1;
        private System.Windows.Forms.Panel pnlSimulation;
        private Krypton.Toolkit.KryptonButton BtnSetHome;
        private Krypton.Toolkit.KryptonButton btnESDisabled;
        private Krypton.Toolkit.KryptonButton btnGoHome3;
        private Krypton.Toolkit.KryptonButton btnGoHome4;
        private Krypton.Toolkit.KryptonButton btnGoHome1;
        private Krypton.Toolkit.KryptonButton btnGoHome2;
        private Krypton.Toolkit.KryptonButton btnSTOP;
        private Krypton.Toolkit.KryptonButton btnImport;
        private Krypton.Toolkit.KryptonGroupBox grpMotor;
        private Krypton.Toolkit.KryptonButton btnMotorON;
        private Krypton.Toolkit.KryptonButton btnMotorOFF;
        private Krypton.Toolkit.KryptonTrackBar trackBarP;
        private Krypton.Toolkit.KryptonButton kryptonButton2;
        private System.Windows.Forms.TextBox txtToSend;
        private Krypton.Toolkit.KryptonButton btnSend;
        private Krypton.Toolkit.KryptonButton btnClearLogs;
        private System.Windows.Forms.CheckBox ckPlayLoop;
        private System.Windows.Forms.Button button1;
        private Krypton.Toolkit.KryptonButton btnTestEndStop;
        private Krypton.Toolkit.KryptonButton END1;
        private Krypton.Toolkit.KryptonButton END2;
        private Krypton.Toolkit.KryptonButton END3;
        private Krypton.Toolkit.KryptonButton END4;
    }
}

