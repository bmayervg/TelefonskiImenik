using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelefonskiImenik.Business;


namespace TelefonskiImenik.Forms
{
    public partial class frmImenikPretraga : Form
    {
        public frmImenikPretraga()
        {
            InitializeComponent();
        }

        private void btnPretrazi_Click(object sender, EventArgs e)
        {
            int pageSize = 100;
            int currentPageIndex = 1;
            int TotalPageCount = 0;
            DataTable tbl = cTelefonPretraga.SearchTelefonskiImenik(txtGrad.Text, tbPostanskiBroj.Text, tbPredbroj.Text, tbTelefonskiBroj.Text, tbUlica.Text, tbIme.Text, tbPrezime.Text, false, pageSize, currentPageIndex, ref TotalPageCount);
            dgvRezultati.DataSource = tbl;
            dgvRezultati.ReadOnly = true;
        }
    }
}

