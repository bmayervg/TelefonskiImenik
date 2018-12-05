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

namespace TelefonskiImenik
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
       
        private void webScraperToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmWebScraper frm = new frmWebScraper();
            frm.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("EXIT ?", "Exit", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://free-proxy-list.net");
            request.Method = "GET";
            request.ReadWriteTimeout = 1000;//200000;
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = noCachePolicy;
            request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(responseFromServer);
            HtmlAgilityPack.HtmlNodeCollection rows = doc.DocumentNode.SelectNodes("//table[@id='proxylisttable']//tbody//tr");
            foreach (HtmlNode n in rows)
            {
                if(n.ChildNodes[4].InnerText.ToString().Contains("elite proxy") )
                {
                    MessageBox.Show( n.ChildNodes[0].InnerText.ToString() + ":" + n.ChildNodes[1].InnerText.ToString());
                }
            }
        }
    }
}
