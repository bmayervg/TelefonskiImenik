namespace TelefonskiImenik
{
    partial class frmMain
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.webScraperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webScraperToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pretragaImenikaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.webScraperToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(825, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // webScraperToolStripMenuItem
            // 
            this.webScraperToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.webScraperToolStripMenuItem1,
            this.pretragaImenikaToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.webScraperToolStripMenuItem.Name = "webScraperToolStripMenuItem";
            this.webScraperToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.webScraperToolStripMenuItem.Text = "Main";
            // 
            // webScraperToolStripMenuItem1
            // 
            this.webScraperToolStripMenuItem1.Name = "webScraperToolStripMenuItem1";
            this.webScraperToolStripMenuItem1.Size = new System.Drawing.Size(163, 22);
            this.webScraperToolStripMenuItem1.Text = "Web Scraper";
            this.webScraperToolStripMenuItem1.Click += new System.EventHandler(this.webScraperToolStripMenuItem1_Click);
            // 
            // pretragaImenikaToolStripMenuItem
            // 
            this.pretragaImenikaToolStripMenuItem.Name = "pretragaImenikaToolStripMenuItem";
            this.pretragaImenikaToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.pretragaImenikaToolStripMenuItem.Text = "Pretraga imenika";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(439, 95);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 461);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.Text = "Imenik";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem webScraperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem webScraperToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pretragaImenikaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button button1;
    }
}

