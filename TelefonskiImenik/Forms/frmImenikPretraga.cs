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
        public DataTable tblSearchResults;

        public frmImenikPretraga()
        {
            InitializeComponent();
            tblSearchResults = new DataTable();
            dgvRezultati.DataSource = tblSearchResults;
        }

        private void btnPretrazi_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                int pageSize = 100;
                int currentPageIndex = 1;
                int TotalPageCount = 0;
                tblSearchResults = cTelefonPretraga.SearchTelefonskiImenik(txtGrad.Text, tbPostanskiBroj.Text, tbPredbroj.Text, tbTelefonskiBroj.Text, tbUlica.Text, tbIme.Text, tbPrezime.Text, false, pageSize, currentPageIndex, ref TotalPageCount);
                dgvRezultati.ReadOnly = true;
                dgvRezultati.Refresh();
            }
            catch (Exception exInsertLog)
            {
                string exDescription = "";
                if (exInsertLog != null)
                {
                    exDescription += Environment.NewLine + "-------------------------------------------------------------------";
                    exDescription += Environment.NewLine + "-------------------------------------------------------------------";
                    exDescription += Environment.NewLine + "Exception: " + exInsertLog.Message.ToString();
                    exDescription += Environment.NewLine + "Exception stack trace: " + exInsertLog.StackTrace.ToString();
                    exDescription += Environment.NewLine + "Exception source:" + exInsertLog.Source.ToString();
                    exDescription += Environment.NewLine + "Exception target site" + exInsertLog.TargetSite.ToString();
                    if (exInsertLog.InnerException != null)
                    {
                        exDescription += Environment.NewLine + "Inner exception: " + exInsertLog.InnerException.Message.ToString();
                        exDescription += (!string.IsNullOrEmpty(exInsertLog.InnerException.StackTrace) ? Environment.NewLine + "Inner exception stack trace: " + exInsertLog.InnerException.StackTrace.ToString() : "");
                        exDescription += (!string.IsNullOrEmpty(exInsertLog.InnerException.Source) ? Environment.NewLine + "Inner exception source: " + exInsertLog.InnerException.Source.ToString() : "");
                        exDescription += (!string.IsNullOrEmpty(exInsertLog.InnerException.TargetSite.ToString()) ? Environment.NewLine + "Inner exception target site: " + exInsertLog.InnerException.TargetSite.ToString().ToString() : "");
                        exDescription += Environment.NewLine;
                    }
                    exDescription += Environment.NewLine + "-------------------------------------------------------------------";
                    exDescription += Environment.NewLine + "-------------------------------------------------------------------";
                }
                MessageBox.Show(exDescription);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if ( (chkCheckNeZoviRegistar.Checked || (chkCheckNeZoviRegistar.Checked == false &&  MessageBox.Show("Export podataka bez provjere registra NE ZOVI --> Nastaviti ?", "Export podataka", MessageBoxButtons.YesNo ) == DialogResult.Yes)))
            {
                    
            }
        }
    }
}

