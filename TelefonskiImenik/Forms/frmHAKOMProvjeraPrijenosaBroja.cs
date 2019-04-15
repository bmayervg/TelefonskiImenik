using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TelefonskiImenik.Forms
{
    public partial class frmHAKOMProvjeraPrijenosaBroja : Form
    {
        public frmHAKOMProvjeraPrijenosaBroja()
        {
            InitializeComponent();
        }

        public static frmHAKOMProvjeraPrijenosaBroja GetForm()
        {
            frmHAKOMProvjeraPrijenosaBroja frm = new frmHAKOMProvjeraPrijenosaBroja();
            frm.Show();
            frm.UseWaitCursor = false;
            return frm;
        }

        public void FillText(string text)
        {
            this.richTextBox1.Text += Environment.NewLine + "--------------------------------------------------------------------------------------" + Environment.NewLine +  text + Environment.NewLine + "--------------------------------------------------------------------------------------";
        }

        private void btnEXIT_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
