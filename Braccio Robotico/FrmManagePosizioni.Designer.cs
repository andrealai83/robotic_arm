namespace Braccio_Robotico
{
    partial class FrmManagePosizioni
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvPosizioni = new Krypton.Toolkit.KryptonDataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCarica = new Krypton.Toolkit.KryptonButton();
            this.btnElimina = new Krypton.Toolkit.KryptonButton();
            this.btnRinomina = new Krypton.Toolkit.KryptonButton();
            this.btnSalvaSetCorrente = new Krypton.Toolkit.KryptonButton();
            this.btnChiudi = new Krypton.Toolkit.KryptonButton();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Nome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPosizioni)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvPosizioni
            // 
            this.dgvPosizioni.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPosizioni.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPosizioni.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Nome});
            this.dgvPosizioni.Location = new System.Drawing.Point(8, 12);
            this.dgvPosizioni.Name = "dgvPosizioni";
            this.dgvPosizioni.Size = new System.Drawing.Size(787, 396);
            this.dgvPosizioni.TabIndex = 0;
            this.dgvPosizioni.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPosizioni_CellClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnChiudi);
            this.panel1.Controls.Add(this.btnSalvaSetCorrente);
            this.panel1.Controls.Add(this.btnCarica);
            this.panel1.Controls.Add(this.btnElimina);
            this.panel1.Controls.Add(this.btnRinomina);
            this.panel1.Location = new System.Drawing.Point(8, 414);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(787, 31);
            this.panel1.TabIndex = 18;
            // 
            // btnCarica
            // 
            this.btnCarica.Location = new System.Drawing.Point(4, 3);
            this.btnCarica.Name = "btnCarica";
            this.btnCarica.Size = new System.Drawing.Size(90, 25);
            this.btnCarica.TabIndex = 26;
            this.btnCarica.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnCarica.Values.Text = "Load";
            this.btnCarica.Click += new System.EventHandler(this.btnCarica_Click);
            // 
            // btnElimina
            // 
            this.btnElimina.Location = new System.Drawing.Point(174, 3);
            this.btnElimina.Name = "btnElimina";
            this.btnElimina.Size = new System.Drawing.Size(90, 25);
            this.btnElimina.TabIndex = 24;
            this.btnElimina.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnElimina.Values.Text = "Delete";
            this.btnElimina.Click += new System.EventHandler(this.btnElimina_Click);
            // 
            // btnRinomina
            // 
            this.btnRinomina.Location = new System.Drawing.Point(345, 3);
            this.btnRinomina.Name = "btnRinomina";
            this.btnRinomina.Size = new System.Drawing.Size(90, 25);
            this.btnRinomina.TabIndex = 25;
            this.btnRinomina.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnRinomina.Values.Text = "Rename";
            this.btnRinomina.Click += new System.EventHandler(this.btnRinomina_Click);
            // 
            // btnSalvaSetCorrente
            // 
            this.btnSalvaSetCorrente.Location = new System.Drawing.Point(516, 3);
            this.btnSalvaSetCorrente.Name = "btnSalvaSetCorrente";
            this.btnSalvaSetCorrente.Size = new System.Drawing.Size(90, 25);
            this.btnSalvaSetCorrente.TabIndex = 27;
            this.btnSalvaSetCorrente.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnSalvaSetCorrente.Values.Text = "Save Current";
            this.btnSalvaSetCorrente.Click += new System.EventHandler(this.btnSalvaSetCorrente_Click);
            // 
            // btnChiudi
            // 
            this.btnChiudi.Location = new System.Drawing.Point(694, 3);
            this.btnChiudi.Name = "btnChiudi";
            this.btnChiudi.Size = new System.Drawing.Size(90, 25);
            this.btnChiudi.TabIndex = 28;
            this.btnChiudi.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.btnChiudi.Values.Text = "Close";
            this.btnChiudi.Click += new System.EventHandler(this.btnChiudi_Click);
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            // 
            // Nome
            // 
            this.Nome.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Nome.HeaderText = "Nome";
            this.Nome.Name = "Nome";
            // 
            // FrmManagePosizioni
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvPosizioni);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FrmManagePosizioni";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Positions";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPosizioni)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Krypton.Toolkit.KryptonDataGridView dgvPosizioni;
        private System.Windows.Forms.Panel panel1;
        private Krypton.Toolkit.KryptonButton btnChiudi;
        private Krypton.Toolkit.KryptonButton btnSalvaSetCorrente;
        private Krypton.Toolkit.KryptonButton btnCarica;
        private Krypton.Toolkit.KryptonButton btnElimina;
        private Krypton.Toolkit.KryptonButton btnRinomina;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nome;
    }
}