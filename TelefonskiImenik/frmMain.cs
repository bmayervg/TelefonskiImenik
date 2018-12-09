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
       
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("EXIT ?", "Exit", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            var postData = "xpp=5&xf1=4&xf5=1";
            var data = Encoding.ASCII.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://spys.one/free-proxy-list/ALL/");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.ReadWriteTimeout = 3000;//200000;
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = noCachePolicy;
            request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko";
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(responseFromServer);
            HtmlAgilityPack.HtmlNodeCollection rows = doc.DocumentNode.SelectNodes("//tr[@class='spy1xx']");
            foreach (HtmlNode n in rows)
            {
                if(n.ChildNodes[4].InnerText.ToString().Contains("elite proxy") )
                {
                    MessageBox.Show( n.ChildNodes[0].InnerText.ToString() + ":" + n.ChildNodes[1].InnerText.ToString());
                }
            }
        }

        private void pretragaImenikaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.frmImenikPretraga frm = new Forms.frmImenikPretraga();
            frm.MdiParent = this;
            frm.Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string contents = File.ReadAllText(@"C:\Users\Bruno\Desktop\test2.txt");
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(contents);
            //HtmlAgilityPack.HtmlNodeCollection rows = doc.DocumentNode.SelectNodes("//td[@class='c_32']");
            //foreach (HtmlNode n in rows)
            //{
            //    //if (n.ChildNodes[4].InnerText.ToString().Contains("elite proxy"))
            //    //{
            //    //    MessageBox.Show(n.ChildNodes[0].InnerText.ToString() + ":" + n.ChildNodes[1].InnerText.ToString());
            //    //}
            //}
                HtmlAgilityPack.HtmlNodeCollection temRezPretrage = doc.DocumentNode.SelectNodes("//td[@class='c_32']");
                foreach (HtmlNode n in temRezPretrage)
                {
                    if (n != null && n.InnerHtml.Contains("PRONA") && n.InnerHtml.Contains("PRIBLI") && n.InnerHtml.Contains("REZULTATA"))
                    {
                        
                    }

                }
            
        }
    }
}
