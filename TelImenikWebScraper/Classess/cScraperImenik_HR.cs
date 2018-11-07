using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Collections.Generic;


namespace TelImenikWebScraper.Classess
{
    public class cScraperImenik_HR
    {
        private int _timeBetweenHTTPRequests_MS = 2000;
        private string _link = "";
        private string _connectionString;
        private int _numberOfThreads = 1;
        
        private DataTable _tblNeprocesiraneUlice;
        private int _currentRow = -1;
        private int _totalRows = 0;
        private int _threadsRunningCount = 0;
        private int _brojacGresaka = 0;
        private bool _isSnimiLog = true;


        public cScraperImenik_HR(string connectionString, int numberOfThreads, string link, int timeBetweenHTTPRequests_MS)
        {
            _timeBetweenHTTPRequests_MS = timeBetweenHTTPRequests_MS;
            _link = link;
            _connectionString = connectionString;
            if (numberOfThreads > 1)
            {
                _numberOfThreads = numberOfThreads;
            }
            else
            {
                _numberOfThreads = 1;
            }

            using (SqlConnection Conn = new SqlConnection(connectionString))
            {
                try
                {
                    Conn.Open();
                    Conn.Close();
                }
                catch (Exception ex)
                {
                    
                }
            }
            this._currentRow = -1;
            this._totalRows = 0;
            this._threadsRunningCount = 0;
            this._brojacGresaka = 0;
            this._isSnimiLog = true;
    }

        public void Start()
        {
            if (!string.IsNullOrEmpty(_link))
            {
                DataTable tblNeprocesiraneUlice = new DataTable();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("spUlica_GetNeprocesirane_Select", conn);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(tblNeprocesiraneUlice);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (tblNeprocesiraneUlice != null && tblNeprocesiraneUlice.Rows.Count != 0)
                {
                    this._tblNeprocesiraneUlice = tblNeprocesiraneUlice;
                    this._totalRows = _tblNeprocesiraneUlice.Rows.Count;
                    runScraper();
                }
                else
                {
                    Console.WriteLine("tblNeprocesiraneUlice ROW COUNT == 0");
                }
            }
            else
            {

            }
        }

        private void SnimiOsobu(int id_Ulica, string imePrezime, string linkOsobaTelBroj, CookieCollection cookies )
        {
            if (id_Ulica != -1 && !string.IsNullOrEmpty(linkOsobaTelBroj) && !string.IsNullOrEmpty(imePrezime))
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this._link + System.Web.HttpUtility.UrlEncode(linkOsobaTelBroj.Replace("/imenik/", "")));
                request.Method = "GET";
                request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko";
                CookieContainer cc = new CookieContainer();
                request.CookieContainer = cc;
                for (int i = 0; i < cookies.Count; i++)
                {
                    Cookie c = new Cookie();
                    c.Secure = cookies[i].Secure;
                    c.Port = cookies[i].Port;
                    c.Path = cookies[i].Path;
                    c.Name = cookies[i].Name;
                    c.HttpOnly = cookies[i].HttpOnly;
                    c.Expires = cookies[i].Expires;
                    c.Domain = cookies[i].Domain;
                    c.Value = cookies[i].Value;
                    c.Discard = cookies[i].Discard;
                    c.CommentUri = cookies[i].CommentUri;
                    c.Comment = cookies[i].Comment;
                    c.Expired = cookies[i].Expired;
                    c.Version = cookies[i].Version;
                    cc.Add(c);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();

                dataStream.Close();
                dataStream.Dispose();

                response.Close();
                response.Dispose();
                if (!string.IsNullOrEmpty(responseFromServer))
                {
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(responseFromServer);
                    
                }
            }
        }

        private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjesto(string link)
        {
            Tuple<CookieCollection, HtmlNodeCollection> t;
            HtmlNodeCollection nodes = new HtmlNodeCollection(null);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(link);
            request.Method = "GET";
            request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko";
            CookieContainer cc = new CookieContainer();
            request.CookieContainer = cc;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            if (!string.IsNullOrEmpty(responseFromServer))
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(responseFromServer);
                nodes = doc.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), ' adresa ')]");
            }
            t = new Tuple<CookieCollection, HtmlNodeCollection>(response.Cookies, nodes);
            return t;
        }

        private void processUlicaMjesto()
        {
            lock (_tblNeprocesiraneUlice)
            {
                _threadsRunningCount++;
            }
            do
            {
                int id_Ulica = -1;
                string mjestoNaziv = "";
                string ulicaNaziv = "";
                lock (_tblNeprocesiraneUlice)
                {
                    if (_currentRow < _totalRows - 1)
                    {
                        _currentRow++;
                        id_Ulica = Convert.ToInt32(_tblNeprocesiraneUlice.Rows[_currentRow]["id_Ulica"]);
                        mjestoNaziv = _tblNeprocesiraneUlice.Rows[_currentRow]["NaseljeNaziv"].ToString();
                        ulicaNaziv = _tblNeprocesiraneUlice.Rows[_currentRow]["UlicaNaziv"].ToString();
                    }
                }
                string log = (!string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.Name ) ? System.Threading.Thread.CurrentThread.Name.ToString() : "t_nr_1" ) + " --> id_Ulica = " + id_Ulica.ToString() + ", mjesto: " + mjestoNaziv + ", ulica: " + ulicaNaziv;
                Console.WriteLine(log);
                if (id_Ulica != -1)
                {
                    int currentResultsPage = 1;
                    
                    if (!string.IsNullOrEmpty(this._link))
                    {
                        Tuple<CookieCollection, HtmlNodeCollection> t = GetUsersForUlicaMjesto(this._link + "/trazi/" + currentResultsPage.ToString() + "/" + "ulica:" + ulicaNaziv + "%20mjesto:" + mjestoNaziv + ".html");
                        HtmlNodeCollection nodes = (HtmlNodeCollection)t.Item2;
                        while( nodes != null && nodes.Count != 0 )
                        {
                            for (int i = 0; i < nodes.Count; i++)
                            {
                                SnimiOsobu(id_Ulica, nodes[0].ChildNodes[1].Attributes[0].Value.ToString(), nodes[0].ChildNodes[1].Attributes[1].Value.ToString(), (CookieCollection)t.Item1);
                            }
                            currentResultsPage++;
                            t = GetUsersForUlicaMjesto(this._link + "/" + currentResultsPage.ToString() + "/" + "ulica:" + ulicaNaziv + "%20mjesto:" + mjestoNaziv + ".html");
                            nodes = (HtmlNodeCollection)t.Item2;
                        }
                    }
                    else
                    {

                    }
                }
                System.Threading.Thread.Sleep(_timeBetweenHTTPRequests_MS);
            }
            while (_currentRow < _totalRows - 1);

            lock (_tblNeprocesiraneUlice)
            {
                _threadsRunningCount--;
            }
        }

        private void runScraper()
        {
            if (this._numberOfThreads > 1)
            {
                for (int i = 0; i < this._numberOfThreads; i++)
                {
                    Thread t = new Thread(processUlicaMjesto);
                    t.Name = "t_nr_" + i.ToString();
                    t.Start();
                }
            }
            else
            {
                this.processUlicaMjesto();
            }
            // progress bar
            int nextPercentage = 5;
            Console.WriteLine();
            Console.WriteLine("|-------------------| Running " + _numberOfThreads.ToString() + " threads on " + _totalRows.ToString() + " items");
            Console.Write("|");
            while (_currentRow < _totalRows - 1 || _threadsRunningCount > 0)
            {
                while (_currentRow * 100 / _currentRow > nextPercentage)
                {
                    Console.Write(".");
                    nextPercentage += 5;
                }
                Thread.Sleep(1000);
            }
            while (nextPercentage < 100)
            {
                Console.Write(".");
                nextPercentage += 5;
            }
            Console.WriteLine("|");
        }

    }
}
