using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using TelefonskiImenik;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Net.Cache;
using HtmlAgilityPack;

namespace TelefonskiImenik
{
    public partial class frmWebScraper : Form
    {
        public frmWebScraper()
        {
            InitializeComponent();
        }

        private async Task TestAsync()
        {
            try
            {
                //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.imenik.hr/imenik/trazi/1/ulica:Ilica%20mjesto:zagreb.html");
                //request.Method = "GET";
                //request.UserAgent = "PostmanRuntime / 7.3.0";
                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Stream dataStream = response.GetResponseStream();
                //StreamReader reader = new StreamReader(dataStream);
                //string responseFromServer = reader.ReadToEnd();
                //reader.Close();
                //dataStream.Close();
                //response.Close();


                var doc = new HtmlAgilityPack.HtmlDocument();
                //doc.Load(@"‪C:\Users\Bruno Mayer\Desktop\Test.html");
                doc.Load("Greska.html");

                //HtmlAgilityPack.HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), ' adresa ')]");
                               //var web = new HtmlWeb();
                //var doc = web.Load("https://www.hakom.hr/default.aspx?id=8391");
                //var hiddenValue = doc.GetElementbyId("frm_nezovi").SelectSingleNode(".//input[@type='hidden']").Attributes["value"].Value;
                
                ////var value = docroot.SelectSingleNode("//input[@type='hidden' and @name='foo']")
                ////.Attributes["value"].Value;
                ////foreach (HtmlNode node in doc.GetElementbyId("frm_nezovi").SelectNodes(".//input"))
                ////{

                ////    richTextBox1.Text += Environment.NewLine + "----------------------------------------------";
                ////    richTextBox1.Text += Environment.NewLine + "node: " + node.Name;
                ////    for (int i = 0; i < node.Attributes.Count; i++)
                ////    {
                ////        richTextBox1.Text += Environment.NewLine + "Att:" + node.Attributes[i].Name + "--> " + node.Attributes[i].Value;
                ////    }

                ////    richTextBox1.Text += Environment.NewLine + " --> node value" +  node.Attributes["value"].Value.ToString();
                ////    richTextBox1.Text += Environment.NewLine + "----------------------------------------------";
                ////}




                //using (var client = new HttpClient())
                //{
                //    var values = new Dictionary<string, string> { { "brojTel", "385916121237" }, { "_validacija", hiddenValue }, { "_valid", hiddenValue } };

                //    var content = new FormUrlEncodedContent(values);

                //    var response = await client.PostAsync("https://www.hakom.hr/default.aspx?id=8391", content);

                //    string responseString = await response.Content.ReadAsStringAsync();
                //}
            }
            catch (Exception ex)
            {
                
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                Task t =  TestAsync();
            }
            catch (Exception ex)
            {

            }

        }
    }
}
