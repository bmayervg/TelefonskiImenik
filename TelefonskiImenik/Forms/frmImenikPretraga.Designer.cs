namespace TelefonskiImenik.Forms
{
    partial class frmImenikPretraga
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tbTelefonskiBroj = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbPredbroj = new System.Windows.Forms.TextBox();
            this.tbPrezime = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbIme = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbUlica = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtGrad = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPostanskiBroj = new System.Windows.Forms.TextBox();
            this.Ulica = new System.Windows.Forms.Label();
            this.btnPretrazi = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgvRezultati = new System.Windows.Forms.DataGridView();
            this.chkCheckNeZoviRegistar = new System.Windows.Forms.CheckBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chkCheckNeZoviRegistarSnimiUBazuRezultat = new System.Windows.Forms.CheckBox();
            this.rtbResponse = new System.Windows.Forms.RichTextBox();
            this.tbRazmakMS = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRezultati)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.tbTelefonskiBroj);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbPredbroj);
            this.groupBox1.Controls.Add(this.tbPrezime);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbIme);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbUlica);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtGrad);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbPostanskiBroj);
            this.groupBox1.Controls.Add(this.Ulica);
            this.groupBox1.Controls.Add(this.btnPretrazi);
            this.groupBox1.Location = new System.Drawing.Point(4, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(392, 187);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parametri pretrage";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox1.Location = new System.Drawing.Point(15, 155);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(68, 17);
            this.checkBox1.TabIndex = 17;
            this.checkBox1.Text = "Exported";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // tbTelefonskiBroj
            // 
            this.tbTelefonskiBroj.Location = new System.Drawing.Point(182, 47);
            this.tbTelefonskiBroj.Name = "tbTelefonskiBroj";
            this.tbTelefonskiBroj.Size = new System.Drawing.Size(200, 20);
            this.tbTelefonskiBroj.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(131, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Tel. broj";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Predbroj";
            // 
            // tbPredbroj
            // 
            this.tbPredbroj.Location = new System.Drawing.Point(64, 47);
            this.tbPredbroj.Name = "tbPredbroj";
            this.tbPredbroj.Size = new System.Drawing.Size(61, 20);
            this.tbPredbroj.TabIndex = 11;
            // 
            // tbPrezime
            // 
            this.tbPrezime.Location = new System.Drawing.Point(64, 125);
            this.tbPrezime.Name = "tbPrezime";
            this.tbPrezime.Size = new System.Drawing.Size(318, 20);
            this.tbPrezime.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Prezime";
            // 
            // tbIme
            // 
            this.tbIme.Location = new System.Drawing.Point(64, 99);
            this.tbIme.Name = "tbIme";
            this.tbIme.Size = new System.Drawing.Size(318, 20);
            this.tbIme.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Ime";
            // 
            // tbUlica
            // 
            this.tbUlica.Location = new System.Drawing.Point(64, 73);
            this.tbUlica.Name = "tbUlica";
            this.tbUlica.Size = new System.Drawing.Size(318, 20);
            this.tbUlica.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Ulica";
            // 
            // txtGrad
            // 
            this.txtGrad.Location = new System.Drawing.Point(64, 19);
            this.txtGrad.Name = "txtGrad";
            this.txtGrad.Size = new System.Drawing.Size(216, 20);
            this.txtGrad.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Grad";
            // 
            // tbPostanskiBroj
            // 
            this.tbPostanskiBroj.Location = new System.Drawing.Point(321, 19);
            this.tbPostanskiBroj.Name = "tbPostanskiBroj";
            this.tbPostanskiBroj.Size = new System.Drawing.Size(61, 20);
            this.tbPostanskiBroj.TabIndex = 2;
            // 
            // Ulica
            // 
            this.Ulica.AutoSize = true;
            this.Ulica.Location = new System.Drawing.Point(286, 22);
            this.Ulica.Name = "Ulica";
            this.Ulica.Size = new System.Drawing.Size(29, 13);
            this.Ulica.TabIndex = 1;
            this.Ulica.Text = "P.br.";
            // 
            // btnPretrazi
            // 
            this.btnPretrazi.Location = new System.Drawing.Point(218, 151);
            this.btnPretrazi.Name = "btnPretrazi";
            this.btnPretrazi.Size = new System.Drawing.Size(164, 23);
            this.btnPretrazi.TabIndex = 0;
            this.btnPretrazi.Text = "Pretraži";
            this.btnPretrazi.UseVisualStyleBackColor = true;
            this.btnPretrazi.Click += new System.EventHandler(this.btnPretrazi_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.dgvRezultati);
            this.groupBox2.Location = new System.Drawing.Point(4, 206);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1059, 566);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rezultati pretrage";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Location = new System.Drawing.Point(6, 20);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1044, 41);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            // 
            // dgvRezultati
            // 
            this.dgvRezultati.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvRezultati.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRezultati.Location = new System.Drawing.Point(8, 67);
            this.dgvRezultati.Name = "dgvRezultati";
            this.dgvRezultati.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRezultati.Size = new System.Drawing.Size(1045, 491);
            this.dgvRezultati.TabIndex = 0;
            // 
            // chkCheckNeZoviRegistar
            // 
            this.chkCheckNeZoviRegistar.AutoSize = true;
            this.chkCheckNeZoviRegistar.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCheckNeZoviRegistar.Checked = true;
            this.chkCheckNeZoviRegistar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCheckNeZoviRegistar.Location = new System.Drawing.Point(8, 19);
            this.chkCheckNeZoviRegistar.Name = "chkCheckNeZoviRegistar";
            this.chkCheckNeZoviRegistar.Size = new System.Drawing.Size(175, 17);
            this.chkCheckNeZoviRegistar.TabIndex = 16;
            this.chkCheckNeZoviRegistar.Text = "Provjera registar NE ZOVI         ";
            this.chkCheckNeZoviRegistar.UseVisualStyleBackColor = true;
            this.chkCheckNeZoviRegistar.CheckedChanged += new System.EventHandler(this.chkCheckNeZoviRegistar_CheckedChanged);
            // 
            // btnExport
            // 
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnExport.Location = new System.Drawing.Point(283, 22);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(188, 87);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Controls.Add(this.rtbResponse);
            this.groupBox4.Controls.Add(this.btnExport);
            this.groupBox4.Location = new System.Drawing.Point(416, 13);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(482, 187);
            this.groupBox4.TabIndex = 35;
            this.groupBox4.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tbRazmakMS);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.chkCheckNeZoviRegistarSnimiUBazuRezultat);
            this.groupBox5.Controls.Add(this.chkCheckNeZoviRegistar);
            this.groupBox5.Location = new System.Drawing.Point(10, 16);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(267, 93);
            this.groupBox5.TabIndex = 36;
            this.groupBox5.TabStop = false;
            this.groupBox5.Tag = "NE ZOVI";
            this.groupBox5.Text = "Registar NE ZOVI";
            // 
            // chkCheckNeZoviRegistarSnimiUBazuRezultat
            // 
            this.chkCheckNeZoviRegistarSnimiUBazuRezultat.AutoSize = true;
            this.chkCheckNeZoviRegistarSnimiUBazuRezultat.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCheckNeZoviRegistarSnimiUBazuRezultat.Checked = true;
            this.chkCheckNeZoviRegistarSnimiUBazuRezultat.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCheckNeZoviRegistarSnimiUBazuRezultat.Location = new System.Drawing.Point(8, 42);
            this.chkCheckNeZoviRegistarSnimiUBazuRezultat.Name = "chkCheckNeZoviRegistarSnimiUBazuRezultat";
            this.chkCheckNeZoviRegistarSnimiUBazuRezultat.Size = new System.Drawing.Size(174, 17);
            this.chkCheckNeZoviRegistarSnimiUBazuRezultat.TabIndex = 19;
            this.chkCheckNeZoviRegistarSnimiUBazuRezultat.Text = "Rezultat provjere spremi u bazu";
            this.chkCheckNeZoviRegistarSnimiUBazuRezultat.UseVisualStyleBackColor = true;
            // 
            // rtbResponse
            // 
            this.rtbResponse.Location = new System.Drawing.Point(10, 115);
            this.rtbResponse.Name = "rtbResponse";
            this.rtbResponse.Size = new System.Drawing.Size(461, 58);
            this.rtbResponse.TabIndex = 18;
            this.rtbResponse.Text = "";
            // 
            // tbRazmakMS
            // 
            this.tbRazmakMS.Location = new System.Drawing.Point(211, 60);
            this.tbRazmakMS.Name = "tbRazmakMS";
            this.tbRazmakMS.Size = new System.Drawing.Size(40, 20);
            this.tbRazmakMS.TabIndex = 19;
            this.tbRazmakMS.Text = "500";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(172, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Razmak ms izmedju upita u registar";
            // 
            // frmImenikPretraga
            // 
            this.AcceptButton = this.btnPretrazi;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1066, 776);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmImenikPretraga";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pretraga imenika";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRezultati)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtGrad;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPostanskiBroj;
        private System.Windows.Forms.Label Ulica;
        private System.Windows.Forms.Button btnPretrazi;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvRezultati;
        private System.Windows.Forms.TextBox tbUlica;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.TextBox tbTelefonskiBroj;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbPredbroj;
        private System.Windows.Forms.TextBox tbPrezime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbIme;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkCheckNeZoviRegistar;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox chkCheckNeZoviRegistarSnimiUBazuRezultat;
        private System.Windows.Forms.RichTextBox rtbResponse;
        private System.Windows.Forms.TextBox tbRazmakMS;
        private System.Windows.Forms.Label label7;
    }
}