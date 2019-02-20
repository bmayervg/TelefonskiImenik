using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;
using System.Windows.Forms;
using TelefonskiImenik.Business;


namespace TelefonskiImenik.Forms
{
    public partial class frmImenikPretraga : Form
    {

        private int currentBojaPostDataRegistar = 1;
        private string getBojaForPostData()
        {
            string boja = "";
            switch (currentBojaPostDataRegistar)
            {
                case 1:
                    boja = "crna";
                    break;
                case 2:
                    boja = "plava";
                    break;
                case 3:
                    boja = "crvena";
                    break;
                case 4:
                    boja = "žuta";
                    break;
                case 5:
                    boja = "zelena";
                    break;
                default:
                    boja = "zelena";
                    break;
            }
            if (currentBojaPostDataRegistar == 5)
            {
                currentBojaPostDataRegistar = 1;
            }
            else
            {
                currentBojaPostDataRegistar++;
            }
            return boja;
        }

        public DataTable tblSearchResults;
        private int currentPageIndex = 1;
        private int TotalPageCount = 0;
        private int PageSize = 100;

        #region PROXY_SERVER

        private List<string> _proxyServerListFromWeb;
        private string proxyIP = "";

        private void fillProxyServerDirectlyFromWebFreeProxy(string userAgent)
        {
            Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + (!string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.Name) ? System.Threading.Thread.CurrentThread.Name.ToString() : "t_nr_1") + " --> FILLING PROXY LIST FROM  https://free-proxy-list.net!");
            string proxyServer = "";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://free-proxy-list.net");
            request.Method = "GET";
            request.ReadWriteTimeout = 4000;//200000;
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = noCachePolicy;
            request.UserAgent = userAgent;
            string responseFromServer = "";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            if (dataStream != null)
            {
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(responseFromServer);
                HtmlAgilityPack.HtmlNodeCollection rows = doc.DocumentNode.SelectNodes("//table[@id='proxylisttable']//tbody//tr");
                int counter = 0;
                foreach (HtmlNode n in rows)
                {
                    if (counter == 4)
                    {
                        break;
                    }
                    if (n.ChildNodes[4].InnerText.ToString().Contains("elite proxy"))
                    {
                        proxyServer = n.ChildNodes[0].InnerText.ToString() + ":" + n.ChildNodes[1].InnerText.ToString();

                        if (_proxyServerListFromWeb == null)
                        {
                            _proxyServerListFromWeb = new List<string>();
                        }

                        string ip = "";
                        int port = 0;
                        string[] serverData = proxyServer.Split(':');
                        if (serverData != null && serverData.Length == 2)
                        {
                            ip = serverData[0].ToString();
                            port = Convert.ToInt32(serverData[1]);
                        }
                        if (!string.IsNullOrEmpty(ip))
                        {
                            try
                            {
                                TcpClient client = new TcpClient(ip, port);
                            }
                            catch (Exception ex)
                            {
                                proxyServer = "";
                            }
                        }
                        if (!string.IsNullOrEmpty(proxyServer) && !_proxyServerListFromWeb.Contains(proxyServer))
                        {
                            _proxyServerListFromWeb.Add(proxyServer);
                            counter++;
                        }
                    }
                }
            }
        }

        private string getProxyServerFromExistingListFromWeb()
        {
            string proxyServer = "";
            if (_proxyServerListFromWeb != null && _proxyServerListFromWeb.Count != 0)
            {
                proxyServer = _proxyServerListFromWeb[0].ToString();
                _proxyServerListFromWeb.RemoveAt(0);
            }
            else
            {
                fillProxyServerDirectlyFromWebFreeProxy(getUserAgent());
                proxyServer = _proxyServerListFromWeb[0].ToString();
                _proxyServerListFromWeb.RemoveAt(0);
            }
            return proxyServer;
        }

        #endregion

        #region USER_AGENT

        private string userAgent = "";
        private List<String> _userAgentList;
        private List<String> _usedUserAgentList;

        private void loadUserAgentListFromFile()
        {
            if (!string.IsNullOrEmpty(Program._userAgentListFilePath))
            {
                _userAgentList = System.IO.File.ReadAllLines(Program._userAgentListFilePath).ToList<String>();
            }
        }

        private string getUserAgent()
        {
            string userAgent = "";
            if (_userAgentList != null && _userAgentList.Count != 0)
            {
                userAgent = _userAgentList[0].ToString();
                _userAgentList.RemoveAt(0);
                if (_usedUserAgentList == null)
                {
                    _usedUserAgentList = new List<string>();
                }
                _usedUserAgentList.Add(userAgent);
            }
            else
            {
                if (_userAgentList != null && _userAgentList.Count == 0 && _usedUserAgentList != null && _usedUserAgentList.Count != 0)
                {
                    _userAgentList.AddRange(_usedUserAgentList);
                    _usedUserAgentList.Clear();
                    userAgent = _userAgentList[0].ToString();
                    _userAgentList.RemoveAt(0);
                }
           
            }
            return userAgent;
        }

        #endregion

        public frmImenikPretraga()
        {
            InitializeComponent();
            tblSearchResults = new DataTable();
            dgvRezultati.DataSource = tblSearchResults;
            loadUserAgentListFromFile();
        }

        private void fillSearchResultHeader()
        {
            groupBox3.Controls.Clear();
            GC.Collect();

            Label lblUkupanBrojRezultata = new Label();
            lblUkupanBrojRezultata.AutoSize = true;
            lblUkupanBrojRezultata.Location = new System.Drawing.Point(826, 17);
            lblUkupanBrojRezultata.Name = "lblUkupanBrojRezultata";
            lblUkupanBrojRezultata.Size = new System.Drawing.Size(103, 13);
            lblUkupanBrojRezultata.TabIndex = 34;
            lblUkupanBrojRezultata.Text = "Rezultata po stranici";
            lblUkupanBrojRezultata.Text = "Ukupan broj stranica: " + (TotalPageCount != 0 ? TotalPageCount / PageSize : 0) + " ukupan broj redaka:" + TotalPageCount.ToString();
            groupBox3.Controls.Add(lblUkupanBrojRezultata);
            Button btnNext = new Button();
            btnNext.Location = new System.Drawing.Point(226, 13);
            btnNext.Name = "btnNext";
            btnNext.Size = new System.Drawing.Size(48, 21);
            btnNext.TabIndex = 20;
            btnNext.Text = "-->";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += new System.EventHandler(this.btnNext_Click);
            groupBox3.Controls.Add(btnNext);

            Button btnPrevious = new Button();
            btnPrevious.Location = new System.Drawing.Point(176, 13);
            btnPrevious.Name = "btnPrevious";
            btnPrevious.Size = new System.Drawing.Size(48, 21);
            btnPrevious.TabIndex = 19;
            btnPrevious.Text = "<--";
            btnPrevious.UseVisualStyleBackColor = true;
            btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            groupBox3.Controls.Add(btnPrevious);

            Label label7 = new Label();
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(6, 16);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(103, 13);
            label7.TabIndex = 18;
            label7.Text = "Rezultata po stranici";
            groupBox3.Controls.Add(label7);

            NumericUpDown npPageSize = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(npPageSize)).BeginInit();
            npPageSize.Location = new System.Drawing.Point(115, 13);
            npPageSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            npPageSize.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            npPageSize.Name = "npPageSize";
            npPageSize.Size = new System.Drawing.Size(55, 20);
            npPageSize.TabIndex = 0;
            npPageSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            npPageSize.Value = new decimal(new int[] {
            PageSize,
            0,
            0,
            0});
            ((System.ComponentModel.ISupportInitialize)(npPageSize)).EndInit();
            PageSize = 50;

            groupBox3.Controls.Add(npPageSize);

            for (int i = 1; i < 14; i++)
            {
                LinkLabel lbl = new LinkLabel();
                lbl.Parent = this.groupBox3;
                lbl.AutoSize = true;
                lbl.Location = new System.Drawing.Point(280 + (i == 0 ? 23 : i * 23), 15);
                lbl.Name = "linkLabel_" + i.ToString();
                lbl.Size = new System.Drawing.Size(23, 23);
                lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                lbl.Text = " " + (i + currentPageIndex).ToString() + " ";
                lbl.Tag = "SEARCHRESULTPAGE";
                lbl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
                this.groupBox3.Controls.Add(lbl);
            }
        }

        private void searchImenik()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                for (int i = 0; i < groupBox3.Controls.Count; i++)
                {
                    if (groupBox3.Controls[i].Name == "npPageSize")
                    {
                        PageSize = (int)((NumericUpDown)groupBox3.Controls[i]).Value;
                    }
                }
                tblSearchResults = cTelefonPretraga.SearchTelefonskiImenik(txtGrad.Text, tbPostanskiBroj.Text, tbPredbroj.Text, tbTelefonskiBroj.Text, tbUlica.Text, tbIme.Text, tbPrezime.Text, false, PageSize, currentPageIndex, ref TotalPageCount);
                dgvRezultati.DataSource = tblSearchResults;
                dgvRezultati.ReadOnly = true;
                dgvRezultati.Refresh();
                fillSearchResultHeader();
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

        private void btnPretrazi_Click(object sender, EventArgs e)
        {
            currentPageIndex = 1;
            searchImenik();
            for (int i = 0; i < groupBox3.Controls.Count; i++)
            {
                if (groupBox3.Controls[i].Name == "btnPrevious")
                {
                    groupBox3.Controls[i].Enabled = false;
                }
            }
        }

        private string provjeriRegistarNeZovi( int id_Telefon, string predBroj, string brojTelefona)
        {
            string provjera = "";
            string boja = getBojaForPostData();
            try
            {
                var client = new RestClient("https://www.hakom.hr/default.aspx?id=8391");
                var request = new RestRequest(Method.POST);
                client.UserAgent = getUserAgent();
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
                request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"brojTel\"\r\n\r\n" + "385"+ predBroj.Substring(1, predBroj.Length-1) + brojTelefona  +"\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"_validacija\"\r\n\r\n" + boja + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"_valid\"\r\n\r\n" + boja + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                string responseFromServer = response.Content;
                if (!string.IsNullOrEmpty(responseFromServer))
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(responseFromServer);
                    HtmlNodeCollection texts = doc.DocumentNode.SelectNodes("//table[@class='prijenosRez2']");
                    
                    if (texts != null)
                    {
                        foreach (var item in texts)
                        {
                            provjera  += item.InnerText.Replace("\n", " ").Replace("\t", "").Replace("\r", "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string exDescription = "";
                if (ex != null)
                {
                    exDescription += Environment.NewLine + "-------------------------------------------------------------------";
                    exDescription += Environment.NewLine + "-------------------------------------------------------------------";
                    exDescription += Environment.NewLine + "Exception: " + ex.Message.ToString();
                    exDescription += Environment.NewLine + "Exception stack trace: " + ex.StackTrace.ToString();
                    exDescription += Environment.NewLine + "Exception source:" + ex.Source.ToString();
                    exDescription += Environment.NewLine + "Exception target site" + ex.TargetSite.ToString();
                    if (ex.InnerException != null)
                    {
                        exDescription += Environment.NewLine + "Inner exception: " + ex.InnerException.Message.ToString();
                        exDescription += (!string.IsNullOrEmpty(ex.InnerException.StackTrace) ? Environment.NewLine + "Inner exception stack trace: " + ex.InnerException.StackTrace.ToString() : "");
                        exDescription += (!string.IsNullOrEmpty(ex.InnerException.Source) ? Environment.NewLine + "Inner exception source: " + ex.InnerException.Source.ToString() : "");
                        exDescription += (!string.IsNullOrEmpty(ex.InnerException.TargetSite.ToString()) ? Environment.NewLine + "Inner exception target site: " + ex.InnerException.TargetSite.ToString().ToString() : "");
                        exDescription += Environment.NewLine;
                    }
                    exDescription += Environment.NewLine + "-------------------------------------------------------------------";
                    exDescription += Environment.NewLine + "-------------------------------------------------------------------";
                }
            }
            if (chkCheckNeZoviRegistarSnimiUBazuRezultat.Checked)
            {
                string postData = "multipart / form - data; boundary = ----WebKitFormBoundary7MA4YWxkTrZu0gW -->, ------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent - Disposition: form - data; name =\"brojTel\"\r\n\r\n" + "385" + predBroj.Substring(1, predBroj.Length - 1) + brojTelefona + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"_validacija\"\r\n\r\n" + boja + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"_valid\"\r\n\r\n" + boja + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--";
                cTelefon.SnimiUpitOdgovorRegistarNeZovi(id_Telefon, postData, provjera, 1);
            }
            return provjera;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if( (chkCheckNeZoviRegistar.Checked || (chkCheckNeZoviRegistar.Checked == false &&  MessageBox.Show("Export podataka bez provjere registra NE ZOVI --> Nastaviti ?", "Export podataka", MessageBoxButtons.YesNo ) == DialogResult.Yes)))
            {
                btnPretrazi.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
                try
                {
                    if (tblSearchResults == null || tblSearchResults.Rows.Count == 0)
                    {
                        MessageBox.Show("Nema podataka za eksport");
                    }
                    else
                    {
                        this.Cursor = Cursors.WaitCursor;
                        for (int i = 0; i < tblSearchResults.Rows.Count; i++)
                        {
                            if (chkCheckNeZoviRegistar.Checked)
                            {
                                string result = provjeriRegistarNeZovi(Convert.ToInt32(tblSearchResults.Rows[i]["id_Telefon"]), tblSearchResults.Rows[i]["predBroj"].ToString(), tblSearchResults.Rows[i]["broj"].ToString());
                                tblSearchResults.Rows[i]["RegistarNeZovi"] = result;
                            }
                            int waitMS = 300;
                            if (!string.IsNullOrEmpty(tbRazmakMS.Text))
                            {
                                Int32.TryParse(tbRazmakMS.Text, out waitMS);
                            }
                            System.Threading.Thread.Sleep(waitMS);
                        }
                        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                        saveFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            ExcelUtils.ExportDataTableToExcel2007(tblSearchResults, saveFileDialog1.FileName, "TelefonskiBrojevi");
                            for (int i = 0; i < tblSearchResults.Rows.Count; i++)
                            {
                                cTelefon.UpdateTelefonExported(Convert.ToInt32(tblSearchResults.Rows[i]["id_Telefon"]), saveFileDialog1.FileName);
                            }
                            MessageBox.Show("Export uspješan!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    string exDescription = "";
                    exDescription += Environment.NewLine + "-------------------------------------------------------------------";
                    exDescription += Environment.NewLine + "-------------------------------------------------------------------";
                    exDescription += Environment.NewLine + "Exception: " + ex.Message.ToString();
                    exDescription += Environment.NewLine + "Exception stack trace: " + ex.StackTrace.ToString();
                    exDescription += Environment.NewLine + "Exception source:" + ex.Source.ToString();
                    exDescription += Environment.NewLine + "Exception target site" + ex.TargetSite.ToString();
                    if (ex.InnerException != null)
                    {
                        exDescription += Environment.NewLine + "Inner exception: " + ex.InnerException.Message.ToString();
                        exDescription += (!string.IsNullOrEmpty(ex.InnerException.StackTrace) ? Environment.NewLine + "Inner exception stack trace: " + ex.InnerException.StackTrace.ToString() : "");
                        exDescription += (!string.IsNullOrEmpty(ex.InnerException.Source) ? Environment.NewLine + "Inner exception source: " + ex.InnerException.Source.ToString() : "");
                        exDescription += (!string.IsNullOrEmpty(ex.InnerException.TargetSite.ToString()) ? Environment.NewLine + "Inner exception target site: " + ex.InnerException.TargetSite.ToString().ToString() : "");
                        exDescription += Environment.NewLine;
                    }
                    exDescription += Environment.NewLine + "-------------------------------------------------------------------";
                    exDescription += Environment.NewLine + "-------------------------------------------------------------------";

                    MessageBox.Show(exDescription);
                    rtbResponse.Text = exDescription;   
                }
                finally
                {
                    btnPretrazi.Enabled = true;
                    groupBox3.Enabled = true;
                    groupBox4.Enabled = true;
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.currentPageIndex = Convert.ToInt32( ((LinkLabel)sender).Text.ToString());
            searchImenik();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (this.currentPageIndex > 1)
            {
                this.currentPageIndex--;
                searchImenik();
            }
            if (this.currentPageIndex == 1)
            {
                for (int i = 0; i < groupBox3.Controls.Count; i++)
                {
                    if (groupBox3.Controls[i].Name == "btnPrevious")
                    {
                        groupBox3.Controls[i].Enabled = false;
                    }
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (this.currentPageIndex < TotalPageCount)
            {
                currentPageIndex++;
                searchImenik();
            }

            if (this.currentPageIndex > 1)
            {
                for (int i = 0; i < groupBox3.Controls.Count; i++)
                {
                    if (groupBox3.Controls[i].Name == "btnPrevious")
                    {
                        groupBox3.Controls[i].Enabled = true;
                    }
                }
            }
        }

        private void chkCheckNeZoviRegistar_CheckedChanged(object sender, EventArgs e)
        {
            chkCheckNeZoviRegistarSnimiUBazuRezultat.Checked = chkCheckNeZoviRegistar.Checked;
        }
    }
}

