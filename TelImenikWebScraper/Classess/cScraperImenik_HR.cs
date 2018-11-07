using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.Net;
using System.IO;
using System.Threading.Tasks;

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
                    runInParallel();
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

        private void rowProcessor()
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
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this._link + "ulica:" + ulicaNaziv + "%20mjesto:" + mjestoNaziv + ".html");
                    request.Method = "GET";
                    //request.UserAgent = "PostmanRuntime / 7.3.0";
                    request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko";
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
                        HtmlAgilityPack.HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), ' adresa ')]");
                        if (htmlNodes != null && htmlNodes.Count != 0)
                        {

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

        private void runInParallel()
        {
            if (this._numberOfThreads > 1)
            {
                for (int i = 0; i < this._numberOfThreads; i++)
                {
                    Thread t = new Thread(rowProcessor);
                    t.Name = "t_nr_" + i.ToString();
                    t.Start();
                }
            }
            else
            {
                this.rowProcessor();
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
