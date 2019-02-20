using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.Cache;
using System.Text;

namespace TelImenikWebScraper.Classess
{
    public class cScraperImenik_HR
    {
        private int _id_WebScraperSession = -1;
        private int _timeBetweenHTTPRequests_MS = 2000;
        private string _link = "";
        private string _connectionString;
        private int _numberOfThreads = 1;

        private bool _loadProxyServerFromWeb = true;
        private string _proxyServerListFilePath;
        private List<String> _proxyServerList;
        private List<String> _usedProxyServerList;
        private List<String> _proxyServerListFromWeb;
        private List<String> _usedproxyServerListFromWeb;

        private string _userAgentListFilePath;
        private List<String> _userAgentList;
        private List<String> _usedUserAgentList;

        private DataTable _tblNeprocesiraneUlice;
        private int _currentRow = -1;
        private int _totalRows = 0;
        private int _threadsRunningCount = 0;

         

        #region USER_AGENT

        private void loadUserAgentListFromFile()
        {
            Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + "OBTAINING USER AGENT LIST FROM FILE");

            if (!string.IsNullOrEmpty(_userAgentListFilePath))
            {
                _userAgentList = System.IO.File.ReadAllLines(_userAgentListFilePath).ToList<String>();
            }
            Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + "OBTAINING USER AGENT LIST FROM FILE --> DONE ");
        }

        private string getUserAgent()
        {
            string userAgent = "";
            lock (_userAgentList)
            {
                lock (_usedUserAgentList)
                {
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
                }
            }
            return userAgent;
        }

        #endregion

        #region PROXY_SERVER_LIST

        private void loadProxyServerListFromFile()
        {
            if (!_loadProxyServerFromWeb)
            {
                Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + "OBTAINING PROXY SERVER LIST FROM FILE");
                if (!string.IsNullOrEmpty(_proxyServerListFilePath))
                {
                    _proxyServerList = System.IO.File.ReadAllLines(_proxyServerListFilePath).ToList<String>();
                    for (int i = 0; i < _proxyServerList.Count; i++)
                    {
                        Console.Write("\r{0} / {1}", i.ToString(), _proxyServerList.Count.ToString());
                        string ip = "";
                        int port = 0;
                        string[] serverData = _proxyServerList[i].ToString().Split(":");
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
                                _proxyServerList.Remove(_proxyServerList[i].ToString());
                            }
                        }
                    }
                }
                Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + "OBTAINING PROXY SERVER LIST FROM FILE-> DONE --> PROXY SERVER LIST COUNT" + _proxyServerList.Count.ToString());
            }
        }

        private string getProxyServerFromExistingList()
        {
            string proxyServer = "";
            lock (_proxyServerList)
            {
                lock (_usedProxyServerList)
                {
                    if (_proxyServerList != null && _proxyServerList.Count != 0)
                    {
                        proxyServer = _proxyServerList[0].ToString();
                        _proxyServerList.RemoveAt(0);
                        if (_usedProxyServerList == null)
                        {
                            _usedProxyServerList = new List<string>();
                        }
                        _usedProxyServerList.Add(proxyServer);
                    }
                    else
                    {
                        if (_proxyServerList == null)
                        {
                            _proxyServerList = new List<string>();
                            _usedProxyServerList = new List<string>();
                        }

                        if (_proxyServerList.Count == 0)
                        {
                            lock (_proxyServerListFromWeb)
                            {
                                lock (_usedproxyServerListFromWeb)
                                {
                                    Console.WriteLine("proxyServer list --> COUNT == 0 --> redirecting to web list proxy server!");
                                    this._loadProxyServerFromWeb = false;
                                    if (this._proxyServerListFromWeb == null)
                                    {
                                        this._proxyServerListFromWeb = new List<string>();
                                        this._usedproxyServerListFromWeb = new List<string>();
                                    }

                                    if (this._proxyServerListFromWeb.Count == 0)
                                    {
                                        fillProxyServerDirectlyFromWebFreeProxy(getUserAgent());
                                    }
                                    proxyServer = getProxyServerFromExistingListFromWeb();
                                }
                            }
                        }
                    }
                }
            }
            return proxyServer;
        }

        private string getProxyServerFromExistingListFromWeb()
        {
            string proxyServer = "";
            lock (_proxyServerListFromWeb)
            {
                lock (_usedproxyServerListFromWeb)
                {
                    if (_proxyServerListFromWeb != null && _proxyServerListFromWeb.Count != 0)
                    {
                        proxyServer = _proxyServerListFromWeb[0].ToString();
                        _proxyServerListFromWeb.RemoveAt(0);
                        if (_usedProxyServerList == null)
                        {
                            _usedproxyServerListFromWeb = new List<string>();
                        }
                        _usedproxyServerListFromWeb.Add(proxyServer);
                    }
                    else
                    {
                        if (_proxyServerListFromWeb == null)
                        {
                            _proxyServerListFromWeb = new List<string>();
                            _usedproxyServerListFromWeb = new List<string>();
                        }
                        if (_proxyServerListFromWeb.Count == 0)
                        {
                            fillProxyServerDirectlyFromWebFreeProxy(getUserAgent());
                        }
                        proxyServer = _proxyServerListFromWeb[0].ToString();
                        _proxyServerListFromWeb.RemoveAt(0);
                    }
                }
            }
            return proxyServer;
        }

        private string getProxyServer()
        {
            string ps = "";
            if (_loadProxyServerFromWeb)
            {
                ps = getProxyServerFromExistingListFromWeb();
            }
            else
            {
                ps = getProxyServerFromExistingList();
            }
            Console.WriteLine( "\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + (!string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.Name) ? System.Threading.Thread.CurrentThread.Name.ToString() : "t_nr_1"  ) + " --> Getting new proxy server!");
            return ps;
        }

        private void fillProxyServerDirectlyFromWebFreeProxy(string userAgent)
        {
            Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + (!string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.Name) ? System.Threading.Thread.CurrentThread.Name.ToString() : "t_nr_1") + " --> FILLING PROXY LIST FROM  https://free-proxy-list.net!");
            string proxyServer = "";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://free-proxy-list.net");
            request.Method = "GET";
            request.ReadWriteTimeout = 6000;//200000;
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
                    if (n.ChildNodes[4].InnerText.ToString().Contains("elite proxy"))
                    {
                        proxyServer = n.ChildNodes[0].InnerText.ToString() + ":" + n.ChildNodes[1].InnerText.ToString();

                        if (_proxyServerListFromWeb == null)
                        {
                            _proxyServerListFromWeb = new List<string>();
                        }

                        string ip = "";
                        int port = 0;
                        string[] serverData = proxyServer.Split(":");
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
                        }
                        counter++;
                        switch (counter % 4)
                        {
                            case 0: Console.Write("    /"); counter = 0; break;
                            case 1: Console.Write("    -"); break;
                            case 2: Console.Write("    \\"); break;
                            case 3: Console.Write("    |"); break;
                        }
                        Console.SetCursorPosition(Console.CursorLeft - 5, Console.CursorTop);
                    }
                }
            }
            Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + (!string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.Name) ? System.Threading.Thread.CurrentThread.Name.ToString() : "t_nr_1") + " --> FILLING PROXY LIST FROM  https://free-proxy-list.net --> DONE!");
        }

        #endregion

        #region SESSION_START_FINISH

        private void startSession()
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("spWebScraperSession_StartSession", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_WebScraperType", 1);
                        cmd.Parameters.AddWithValue("@NumberOfThreadsUsed", _numberOfThreads);
                        SqlParameter p = new SqlParameter("@id_WebScraperSession", SqlDbType.Int);
                        p.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(p);
                        cmd.ExecuteNonQuery();
                        this._id_WebScraperSession = Convert.ToInt32(p.Value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + exceptionToString(ex));
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + "IMENIKHR --> _connectionString NOT INITIALIZED!");
            }
        }

        private void finishSession()
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("spWebScraperSession_FinishSession", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_WebScraperSession", this._id_WebScraperSession);
                        cmd.Parameters.AddWithValue("@ProcessedRecords", this._totalRows);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + exceptionToString(ex));
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + "IMENIKHR --> _connectionString NOT INITIALIZED!");
            }
        }

        #endregion

        #region SESSION_LOG

        private string exceptionToString(Exception exInsertLog)
        {
            string exDescription = "";
            try
            {
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
            }
            catch (Exception e)
            {
                Console.WriteLine("|");
                Console.WriteLine("ERROR IN exceptionToString(Exception ex)");
                Console.WriteLine(e.Message.ToString());
                Console.WriteLine("|");
            }
            return exDescription;
        }

        private void saveSessionLog(Exception ex)
        {
            string exceptionDesc = exceptionToString(ex);
            saveSessionLog(exceptionDesc, true);
        }
       
        private void saveSessionLog(string logText, bool isException)
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + logText);
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("spWebScraperSessionLog_SaveLog", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_WebScraperSession", this._id_WebScraperSession);
                        cmd.Parameters.AddWithValue("@logText",logText);
                        cmd.Parameters.AddWithValue("@isException", isException);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("|");
                        Console.WriteLine("ERROR WHILE TRYING saveSessionLog");
                        Console.WriteLine("|");
                    }
                }
            }
        }

        #endregion

        public cScraperImenik_HR(string connectionString, int numberOfThreads, string link, int timeBetweenHTTPRequests_MS, string proxyServerListFilePath, string userAgentListFilePath, bool loadProxyServerFromWeb)
        {
            _loadProxyServerFromWeb = loadProxyServerFromWeb;
            _proxyServerListFilePath = proxyServerListFilePath;
            _usedProxyServerList = new List<string>();
            _usedUserAgentList = new List<string>();
            _proxyServerListFromWeb = new List<string>();
            _usedproxyServerListFromWeb = new List<string>();
            _userAgentListFilePath = userAgentListFilePath;
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
                    //TODO LOGGING
                }
            }
            this._currentRow = -1;
            this._totalRows = 0;
            this._threadsRunningCount = 0;
            loadUserAgentListFromFile();
            loadProxyServerListFromFile();
            string agent = getUserAgent();
            if (_loadProxyServerFromWeb)
            {
                fillProxyServerDirectlyFromWebFreeProxy(agent);
            }
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
                        SqlCommand  cmd = new SqlCommand("spUlica_GetNeprocesirane_Select", conn);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(tblNeprocesiraneUlice);
                    }
                    catch (Exception ex)
                    {
                        saveSessionLog(ex);
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
                    saveSessionLog("IMENIK HR --> tblNeprocesiraneUlice EMPTY!", false);
                    Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + "tblNeprocesiraneUlice ROW COUNT == 0");
                }
            }
            else
            {
                saveSessionLog("IMENIK HR --> _link EMPTY STRING!", false);
            }
        }

        private void snimiFirmu(int id_Ulica, string nazivFirma, string linkFirmaTelBroj, CookieCollection cookies, string proxyIP, string userAgent, int requestTimeOut )
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(System.Web.HttpUtility.UrlEncode(linkFirmaTelBroj).Replace("%2f", "/").Replace("%3a", ":"));
            request.Method = "GET";
            if (string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko";
            }
            else
            {
                request.UserAgent = userAgent;
            }

            if (!string.IsNullOrEmpty(proxyIP) )
            {
                WebProxy wp = new WebProxy(proxyIP);
                wp.BypassProxyOnLocal = false;
                request.Timeout = requestTimeOut;
                request.Proxy = wp;
                request.KeepAlive = false;
                request.Timeout = System.Threading.Timeout.Infinite;
                request.ProtocolVersion = HttpVersion.Version10;
                request.AllowWriteStreamBuffering = false;
            }

            CookieContainer cc = new CookieContainer();
            request.CookieContainer = cc;
            string responseFromServer = "";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            if (dataStream != null)
            {
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }
            dataStream.Close();
            dataStream.Dispose();
            response.Close();
            response.Dispose();
            if (!string.IsNullOrEmpty(responseFromServer))
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(responseFromServer);
                if (doc != null)
                {
                    int id_Firma = -1;
                    string firmaNaziv = nazivFirma;
                    string firmaNaselje = "";
                    string firmaPostanskiBroj = "";
                    string firmaUlica = "";
                    string firmaKucniBroj = "";
                    string firmaPunaAdresa = "";
                  
                    HtmlAgilityPack.HtmlNodeCollection htmlNodesFirmaAdresa = doc.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), ' adresa_detalj ')]");
                    if (htmlNodesFirmaAdresa != null && htmlNodesFirmaAdresa.Count > 0)
                    {
                        string[] firmaPodaci = htmlNodesFirmaAdresa[0].InnerText.Trim().Split(' ');
                        if (firmaPodaci != null && firmaPodaci.Length != 0)
                        {
                            try
                            {
                                firmaPunaAdresa = htmlNodesFirmaAdresa[0].InnerText.Trim();
                                firmaPostanskiBroj = firmaPodaci[0].ToString().Replace(",", "");
                                firmaNaselje = firmaPodaci[1].ToString().Replace(",", "");
                                
                                string zadnjiPodatak = firmaPodaci[firmaPodaci.Length - 1].ToString();

                                if (zadnjiPodatak.ToUpper() == "BB" || zadnjiPodatak.Any(c => char.IsDigit(c)))
                                {
                                    firmaKucniBroj = zadnjiPodatak;
                                }
                                for(int i = 2; i< firmaPodaci.Length-1; i++)
                                {
                                    firmaUlica += firmaPodaci[i].ToString() + " ";
                                }
                            }
                            catch (Exception ex)
                            {
                                saveSessionLog(ex);
                            }
                        }
                        else
                        {
                            saveSessionLog("IMENIK HR --> snimiFirmusnimiFirmu(int id_Ulica, string nazivFirma, string linkFirmaTelBroj, CookieCollection cookies) -->  string[] firmaPodaci = htmlNodesOsobaAdresa[0].InnerText.Trim().Split(' ') IS NULL OR COUNT == 0 --> " + "--> id_Ulica=" + id_Ulica.ToString() + ", nazivFirma=" + nazivFirma + "linkFirmaTelBroj=" + linkFirmaTelBroj, false);
                        }
                    }
                    else
                    {
                        saveSessionLog("IMENIK HR -->snimiFirmu(int id_Ulica, string nazivFirma, string linkFirmaTelBroj, CookieCollection cookies) --> htmlNodesFirmaAdresa IS NULL OR COUNT == 0 --> " + "--> id_Ulica=" + id_Ulica.ToString() + ", nazivFirma=" + nazivFirma + "linkFirmaTelBroj=" + linkFirmaTelBroj, false);
                    }

                    HtmlAgilityPack.HtmlNodeCollection htmlNodesTelBrojevi = doc.DocumentNode.SelectNodes("//td[contains(concat(' ', @class, ' '), ' data_tel ')]");
                    if (htmlNodesTelBrojevi != null && htmlNodesTelBrojevi.Count != 0)
                    {
                        using (SqlConnection conn = new SqlConnection(_connectionString))
                        {
                            try
                            {
                                conn.Open();
                                SqlCommand cmd = new SqlCommand("spFirma_SnimiFirmu_Insert", conn);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@firmaNaziv", firmaNaziv);
                                cmd.Parameters.AddWithValue("@firmaNaselje", firmaNaselje);
                                cmd.Parameters.AddWithValue("@firmaPostanskiBroj", firmaPostanskiBroj);
                                cmd.Parameters.AddWithValue("@firmaUlica", firmaUlica);
                                cmd.Parameters.AddWithValue("@firmaKucniBroj", firmaKucniBroj);
                                cmd.Parameters.AddWithValue("@firmaPunaAdresa", firmaPunaAdresa);
                                cmd.Parameters.AddWithValue("@id_Ulica", id_Ulica);
                                cmd.Parameters.AddWithValue("@id_WebScraperSession", this._id_WebScraperSession);
                                SqlParameter p = new SqlParameter("@id_Firma", SqlDbType.Int);
                                p.Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(p);
                                cmd.ExecuteNonQuery();
                                id_Firma = Convert.ToInt32(p.Value);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + exceptionToString(ex));
                            }
                            finally
                            {
                                if (conn.State == ConnectionState.Open)
                                {
                                    conn.Close();
                                }
                            }
                        }

                        if (id_Firma != -1)
                        {
                            for (int i = 0; i < htmlNodesTelBrojevi.Count; i++)
                            {
                                using (SqlConnection conn = new SqlConnection(_connectionString))
                                {
                                    try
                                    {
                                        conn.Open();
                                        SqlCommand cmd = new SqlCommand("spTelefon_SnimiTelefon_Insert", conn);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@id_Firma", id_Firma);
                                        cmd.Parameters.AddWithValue("@PredBroj", htmlNodesTelBrojevi[i].InnerText.Trim().Substring(0, htmlNodesTelBrojevi[i].InnerText.Trim().LastIndexOf(')')).Replace("(", ""));
                                        cmd.Parameters.AddWithValue("@Broj", htmlNodesTelBrojevi[i].InnerText.Trim().Substring(htmlNodesTelBrojevi[i].InnerText.Trim().LastIndexOf(')')).Replace(")", "").Trim().Replace(" ", ""));
                                        cmd.Parameters.AddWithValue("@id_WebScraperSession", this._id_WebScraperSession);
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + exceptionToString(ex));
                                    }
                                    finally
                                    {
                                        if (conn.State == ConnectionState.Open)
                                        {
                                            conn.Close();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            saveSessionLog("IMENIK HR --> snimiFirmu(int id_Ulica, string nazivFirma, string linkFirmaTelBroj, CookieCollection cookies) --> id_Firma == -1!!!! --> " + "--> id_Ulica=" + id_Ulica.ToString() + ", nazivFirma=" + nazivFirma + "linkFirmaTelBroj=" + linkFirmaTelBroj, false);
                        }
                    }
                    else
                    {
                       saveSessionLog("IMENIK HR --> snimiFirmu(int id_Ulica, string nazivFirma, string linkFirmaTelBroj, CookieCollection cookies) --> htmlNodesTelBrojevi IS NULL OR COUNT == 0 --> " + "--> id_Ulica=" + id_Ulica.ToString() + ", nazivFirma=" + nazivFirma + "linkFirmaTelBroj=" + linkFirmaTelBroj, false);
                    }
                }
                else
                {
                    saveSessionLog("IMENIK HR --> snimiFirmu(int id_Ulica, string nazivFirma, string linkFirmaTelBroj, CookieCollection cookies) --> doc.LoadHtml(responseFromServer) IS NULL --> id_Ulica=" + id_Ulica.ToString() + ", nazivFirma=" + nazivFirma + "linkFirmaTelBroj=" + linkFirmaTelBroj, false);
                }
            }
            else
            {
                saveSessionLog("IMENIK HR --> snimiFirmu(int id_Ulica, string nazivFirma, string linkFirmaTelBroj, CookieCollection cookies) --> responseFromServer IS NULL  --> id_Ulica=" + id_Ulica.ToString() + ", nazivFirma=" + nazivFirma + "linkFirmaTelBroj=" + linkFirmaTelBroj, false);
            }
        }

        private string GetResponseFromServerForOsoba(string linkOsobaTelBroj, CookieCollection cookies, string proxyIP, string userAgent, int requestTimeout )
        {
            string responseFromServer = "";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this._link.Replace("/imenik", "") + System.Web.HttpUtility.UrlEncode(linkOsobaTelBroj).Replace("%2f", "/"));
            request.Method = "GET";
            request.ReadWriteTimeout = requestTimeout + 5000; ;
            request.KeepAlive = false;

            if (string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko";
            }
            else
            {
                request.UserAgent = userAgent;
            }

            if (!string.IsNullOrEmpty(proxyIP))
            {
                WebProxy wp = new WebProxy(proxyIP);
                wp.BypassProxyOnLocal = false;
                request.Timeout = requestTimeout;
                request.Proxy = wp;
                request.Timeout = requestTimeout + 5000;//System.Threading.Timeout.Infinite;
                request.ProtocolVersion = HttpVersion.Version10;
                request.AllowWriteStreamBuffering = false;
            }

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
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var buffer = new byte[180000];
                using (Stream sm = response.GetResponseStream())
                {
                    int totalBytesRead = 0;
                    int bytesRead;
                    do
                    {
                        if (sm.CanRead)
                        {
                            bytesRead = sm.Read(buffer, totalBytesRead, 180000 - totalBytesRead);
                            totalBytesRead += bytesRead;
                        }
                        else
                        {
                            bytesRead = 0;
                        }
                    } while (bytesRead != 0 );
                }
                responseFromServer = Encoding.Default.GetString(buffer);

                try
                {
                    response.Close();
                    response.Dispose();
                }
                catch (Exception rspEX)
                { }
                finally
                {
                    response = null;
                    buffer = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> GetResponseFromServerForOsoba -->" + linkOsobaTelBroj + " --> EXCEPTION!");
                saveSessionLog(ex);
            }
            return responseFromServer;
        }

        private HtmlAgilityPack.HtmlDocument GetHTMLOsoba(string linkOsobaTelBroj, CookieCollection cookies, ref string proxyIP, ref string userAgent, int requestTimeout)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
            string responseFromServer = GetResponseFromServerForOsoba(linkOsobaTelBroj, cookies, proxyIP, userAgent, requestTimeout);
            if (!string.IsNullOrEmpty(responseFromServer))
            {
                while
                (
                         responseFromServer.Contains("Odmori malo, zaslužio si...")
                      || responseFromServer.Contains("A connection attempt failed because the connected party did not properly respond after a period of time") 
                      || responseFromServer.Contains("The operation was canceled.") 
                      || responseFromServer.Contains("The operation has timed out.") 
                      || responseFromServer.Contains("No connection could be made because the target machine actively refused it No connection could be made because the target machine actively refused it") 
                      || responseFromServer.Contains("An existing connection was forcibly closed by the remote host") 
                      || responseFromServer.Contains("The remote server returned an error: (403) Forbidden")
                 )
                {
                    proxyIP = getProxyServer();
                    userAgent = getUserAgent();
                    Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> GetHTMLOsoba -->" + linkOsobaTelBroj + " --> GETTING NEW PROXY!");
                    responseFromServer = GetResponseFromServerForOsoba(linkOsobaTelBroj, cookies, proxyIP, userAgent, requestTimeout);
                }
                doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(responseFromServer);
            }
            return doc;
        }

        private void snimiOsobu(int id_Ulica, string imePrezime, string linkOsobaTelBroj, CookieCollection cookies, ref string proxyIP, ref string userAgent, int requestTimeout)
        {
            if (id_Ulica != -1 && !string.IsNullOrEmpty(linkOsobaTelBroj) && !string.IsNullOrEmpty(imePrezime))
            {
                var doc = GetHTMLOsoba(linkOsobaTelBroj, cookies, ref proxyIP, ref userAgent, requestTimeout);
                if (doc != null)
                {
                    int id_Osoba = -1;
                    string osobaIme = "";
                    string osobaPrezime = "";
                    string osobaNaselje = "";
                    string osobaPostanskiBroj = "";
                    string osobaUlica = "";
                    string osobaKucniBroj = "";
                    string osobaPunaAdresa = "";
                    string[] oip = imePrezime.Split(' ');
                    if (oip != null && oip.Length >= 2)
                    {
                        osobaIme = oip[0].ToString();
                        osobaPrezime = oip[1].ToString();
                        if (oip.Length == 3)
                        {
                            osobaPrezime = " " + oip[2].ToString();
                        }

                        if (oip.Length == 4)
                        {
                            osobaPrezime = " " + oip[3].ToString();
                        }
                    }
                    HtmlAgilityPack.HtmlNodeCollection htmlNodesOsobaAdresa = doc.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), ' adresa_detalj ')]");
                    if (htmlNodesOsobaAdresa != null && htmlNodesOsobaAdresa.Count > 0)
                    {
                        string[] osobaPodaci = htmlNodesOsobaAdresa[0].InnerText.Trim().Split(',');
                        if (osobaPodaci != null && osobaPodaci.Length != 0)
                        {
                            try
                            {
                                //10410 Velika Gorica, Cvjetno naselje 12
                                osobaPunaAdresa = htmlNodesOsobaAdresa[0].InnerText.Trim();
                                string[] osobaPodaciPostanskiBrojGrad = osobaPodaci[0].Split(' ');
                                if (osobaPodaciPostanskiBrojGrad != null && osobaPodaciPostanskiBrojGrad.Length != 0)
                                {
                                    osobaPostanskiBroj = osobaPodaciPostanskiBrojGrad[0].ToString();
                                    for (int i = 1; i < osobaPodaciPostanskiBrojGrad.Length; i++)
                                    {
                                        osobaNaselje += osobaPodaciPostanskiBrojGrad[i].ToString() + " ";
                                    }
                                }

                                string[] osobaPodaciUlicaKucniBroj = osobaPodaci[1].Split(' ');
                                if (osobaPodaciUlicaKucniBroj != null && osobaPodaciUlicaKucniBroj.Length != 0)
                                {
                                    osobaKucniBroj = osobaPodaciUlicaKucniBroj[osobaPodaciUlicaKucniBroj.Length - 1].ToString();
                                    for (int i = 0; i < osobaPodaciUlicaKucniBroj.Length-1; i++)
                                    {
                                        osobaUlica += osobaPodaciUlicaKucniBroj[i].ToString() + " ";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                saveSessionLog(ex);
                            }
                        }
                        else
                        {
                            saveSessionLog("IMENIK HR --> SnimiOsobu(int id_Ulica, string imePrezime, string linkOsobaTelBroj, CookieCollection cookies ) -->  string[] osobaPodaci = htmlNodesOsobaAdresa[0].InnerText.Trim().Split(' ') IS NULL OR COUNT == 0 --> " + "--> id_Ulica=" + id_Ulica.ToString() + ", imePrezime=" + imePrezime + "linkOsobaTelBroj=" + linkOsobaTelBroj, false);
                        }
                    }
                    else
                    {
                        saveSessionLog("IMENIK HR --> SnimiOsobu(int id_Ulica, string imePrezime, string linkOsobaTelBroj, CookieCollection cookies ) --> htmlNodesOsobaAdresa IS NULL OR COUNT == 0 --> " + "--> id_Ulica=" + id_Ulica.ToString() + ", imePrezime=" + imePrezime + "linkOsobaTelBroj=" + linkOsobaTelBroj, false);
                    }

                    HtmlAgilityPack.HtmlNodeCollection htmlNodesTelBrojevi = doc.DocumentNode.SelectNodes("//td[contains(concat(' ', @class, ' '), ' data_tel ')]");
                    if (htmlNodesTelBrojevi != null && htmlNodesTelBrojevi.Count != 0)
                    {
                        Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + "IMENIK HR --> SnimiOsobu --> SAVING OSOBA /TELEFON TO DATABASE -->" + osobaIme + " " + osobaPrezime);
                        using (SqlConnection conn = new SqlConnection(_connectionString))
                        {
                            try
                            {
                                conn.Open();
                                SqlCommand cmd = new SqlCommand("spOsoba_SnimiOsobu_Insert", conn);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@osobaIme", osobaIme);
                                cmd.Parameters.AddWithValue("@osobaPrezime", osobaPrezime);
                                cmd.Parameters.AddWithValue("@osobaNaselje", osobaNaselje);
                                cmd.Parameters.AddWithValue("@osobaPostanskiBroj", osobaPostanskiBroj);
                                cmd.Parameters.AddWithValue("@osobaUlica", osobaUlica);
                                cmd.Parameters.AddWithValue("@osobaKucniBroj", osobaKucniBroj);
                                cmd.Parameters.AddWithValue("@osobaPunaAdresa", osobaPunaAdresa);
                                cmd.Parameters.AddWithValue("@id_Ulica", id_Ulica);
                                cmd.Parameters.AddWithValue("@id_WebScraperSession", this._id_WebScraperSession);
                                SqlParameter p = new SqlParameter("@id_Osoba", SqlDbType.Int);
                                p.Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(p);
                                cmd.ExecuteNonQuery();
                                id_Osoba = Convert.ToInt32(p.Value);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + exceptionToString(ex));
                            }
                            finally
                            {
                                if (conn.State == ConnectionState.Open)
                                {
                                    conn.Close();
                                }
                            }
                        }
                        if (id_Osoba != -1)
                        {
                            for (int i = 0; i < htmlNodesTelBrojevi.Count; i++)
                            {
                                using (SqlConnection conn = new SqlConnection(_connectionString))
                                {
                                    try
                                    {
                                        conn.Open();
                                        SqlCommand cmd = new SqlCommand("spTelefon_SnimiTelefon_Insert", conn);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@id_Osoba", id_Osoba);
                                        cmd.Parameters.AddWithValue("@PredBroj", htmlNodesTelBrojevi[i].InnerText.Trim().Substring(0, htmlNodesTelBrojevi[i].InnerText.Trim().LastIndexOf(')')).Replace("(", ""));
                                        cmd.Parameters.AddWithValue("@Broj", htmlNodesTelBrojevi[i].InnerText.Trim().Substring(htmlNodesTelBrojevi[i].InnerText.Trim().LastIndexOf(')')).Replace(")", "").Trim().Replace(" ", ""));
                                        cmd.Parameters.AddWithValue("@id_WebScraperSession", this._id_WebScraperSession);
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + exceptionToString(ex));
                                    }
                                    finally
                                    {
                                        if (conn.State == ConnectionState.Open)
                                        {
                                            conn.Close();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            saveSessionLog("IMENIK HR --> SnimiOsobu(int id_Ulica, string imePrezime, string linkOsobaTelBroj, CookieCollection cookies ) --> id_Osoba == -1!!!! --> " + "--> id_Ulica=" + id_Ulica.ToString() + ", imePrezime=" + imePrezime + "linkOsobaTelBroj=" + linkOsobaTelBroj, false);
                        }
                    }
                    else
                    {
                        saveSessionLog("IMENIK HR --> SnimiOsobu(int id_Ulica, string imePrezime, string linkOsobaTelBroj, CookieCollection cookies ) --> htmlNodesTelBrojevi IS NULL OR COUNT == 0 --> " + "--> id_Ulica=" + id_Ulica.ToString() + ", imePrezime=" + imePrezime + "linkOsobaTelBroj=" + linkOsobaTelBroj, false);
                    }
                }
                else
                {
                    saveSessionLog("IMENIK HR --> SnimiOsobu(int id_Ulica, string imePrezime, string linkOsobaTelBroj, CookieCollection cookies ) --> responseFromServer IS NULL --> " + "--> id_Ulica=" + id_Ulica.ToString() + ", imePrezime=" + imePrezime + "linkOsobaTelBroj=" + linkOsobaTelBroj, false);
                }
            }
        }

        private void snimiOsobeFirme(HtmlNodeCollection nodes, int id_Ulica, CookieCollection cookies, ref string proxyIP, ref string userAgent, int requestTimeout )
        {
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].ParentNode.ParentNode.InnerHtml.Contains("POSLOVNI"))
                    {
                        //snimiFirmu(id_Ulica, nodes[i].ChildNodes[1].Attributes[0].Value.ToString(), nodes[i].ChildNodes[1].Attributes[1].Value.ToString(), (CookieCollection)t.Item1, ref proxyIP, userAgent, this._timeBetweenHTTPRequests_MS);
                    }
                    else
                    {
                       snimiOsobu(id_Ulica, nodes[i].ChildNodes[1].Attributes[0].Value.ToString(), nodes[i].ChildNodes[1].Attributes[1].Value.ToString(), cookies, ref proxyIP, ref userAgent, this._timeBetweenHTTPRequests_MS);  
                    }
                }
            }
        }

        private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjesto(string link, CookieCollection cookies, ref string proxyIP, ref string userAgent, int requestTimeout, ref bool IsCaptcha, ref bool useGradInsteadMjesto, ref int rezultatPretrage, int currentResultPage)
        {
            Tuple<CookieCollection, HtmlNodeCollection> t = null;
            IsCaptcha = false;
            string log = "";
            try
            {
                HtmlNodeCollection nodes = new HtmlNodeCollection(null);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(link);
                request.Method = "GET";
                request.ReadWriteTimeout = requestTimeout + 5000;//200000;
                HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                request.CachePolicy = noCachePolicy;
                if (string.IsNullOrEmpty(userAgent))
                {
                    request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko";
                }
                else
                {
                    request.UserAgent = userAgent;
                }
                if (!string.IsNullOrEmpty(proxyIP))
                {
                    WebProxy wp = new WebProxy(proxyIP);
                    wp.BypassProxyOnLocal = true;
                    request.Timeout = requestTimeout + 5000;
                    request.Proxy = wp;
                    request.KeepAlive = false;
                    request.ProtocolVersion = HttpVersion.Version10;
                    request.AllowWriteStreamBuffering = false;
                }
                CookieContainer cc = new CookieContainer();
                request.CookieContainer = cc;
                if (cookies != null && cookies.Count != 0)
                {
                    for (int ck = 0; ck < cookies.Count; ck++)
                    {
                        Cookie c = new Cookie();
                        c.Secure = cookies[ck].Secure;
                        c.Port = cookies[ck].Port;
                        c.Path = cookies[ck].Path;
                        c.Name = cookies[ck].Name;
                        c.HttpOnly = cookies[ck].HttpOnly;
                        c.Expires = cookies[ck].Expires;
                        c.Domain = cookies[ck].Domain;
                        c.Value = cookies[ck].Value;
                        c.Discard = cookies[ck].Discard;
                        c.CommentUri = cookies[ck].CommentUri;
                        c.Comment = cookies[ck].Comment;
                        c.Expired = cookies[ck].Expired;
                        c.Version = cookies[ck].Version;
                        cc.Add(c);
                    }
                }
                string responseFromServer = "";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var buffer = new byte[180000];
                using (Stream sm = response.GetResponseStream())
                {
                    int totalBytesRead = 0;
                    int bytesRead;
                    do
                    {
                        if (sm.CanRead)
                        {
                            bytesRead = sm.Read(buffer, totalBytesRead, 180000 - totalBytesRead);
                            totalBytesRead += bytesRead;
                        }
                        else
                        {
                            bytesRead = 0;
                        }
                    } while (bytesRead != 0);
                    request.Abort();
                }
                responseFromServer = Encoding.Default.GetString(buffer);
                buffer = null;
                if (!string.IsNullOrEmpty(responseFromServer))
                {
                    if (responseFromServer.Contains("Odmori malo, zaslužio si..."))
                    {
                        IsCaptcha = true;
                        log = "IMENIK HR --> private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjesto(string link) --> responseFromServer CAPTCHA --> link" + link;
                    }
                    else if (responseFromServer.Contains("Nije pronađen niti jedan rezultat za upit") || responseFromServer.Contains("niti jedan rezultat za upit") || responseFromServer.Contains("niti jedan") )
                    {
                        log = "IMENIK HR --> private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjesto(string link) --> responseFromServer: Nije pronađen niti jedan rezultat za upit --> link" + link;
                        useGradInsteadMjesto = !useGradInsteadMjesto;
                        rezultatPretrage = -666;
                    }
                    else if (responseFromServer.Contains("PRONAĐENO PRIBLIŽNO") || responseFromServer.Contains("PRONAĐENO UKUPNO"))
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(responseFromServer);
                        nodes = doc.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), ' adresa ')]");
                        t = new Tuple<CookieCollection, HtmlNodeCollection>(response.Cookies, nodes);
                        if (currentResultPage == 1)
                        {
                            HtmlAgilityPack.HtmlNodeCollection temRezPretrage = doc.DocumentNode.SelectNodes("//td[@class='c_32']");
                            foreach (HtmlNode n in temRezPretrage)
                            {
                                if (n != null && n.InnerHtml.Contains("PRONA") && ( n.InnerHtml.Contains("PRIBLI") || n.InnerHtml.Contains("UKUPNO")) && n.InnerHtml.Contains("REZULTATA"))
                                {
                                    if (n.ChildNodes != null && n.ChildNodes.Count == 3 && !string.IsNullOrEmpty(n.ChildNodes[1].InnerHtml.ToString()))
                                    {
                                        rezultatPretrage = Convert.ToInt32(n.ChildNodes[1].InnerHtml.ToString());
                                        break;
                                    }
                                }
                            }
                        }
                        IsCaptcha = false;
                    }
                    else
                    {
                        log = "IMENIK HR --> private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjesto(string link) --> UNKNOWN responseFromServer: " + responseFromServer + " --> link" + link;
                    }
                }
                else
                {
                    log = "IMENIK HR --> private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjesto(string link) --> responseFromServer EMPTY STRING --> link" + link;
                }
                try
                {
                    response.Close();
                    response.Dispose();
                }
                catch (Exception rspEX)
                { }
                finally
                {
                    response = null;
                }

            }
            catch (Exception ex)
            {
                IsCaptcha = false;
                if 
                (
                       ex.Message.ToString().Contains("A connection attempt failed because the connected party did not properly respond after a period of time") 
                    || ex.Message.ToString().Contains("The operation was canceled.") 
                    || ex.Message.ToString().Contains("The operation has timed out.") 
                    || ex.Message.ToString().Contains("No connection could be made because the target machine actively refused it No connection could be made because the target machine actively refused it") 
                    || ex.Message.ToString().Contains("An existing connection was forcibly closed by the remote host") 
                    || ex.Message.ToString().Contains("The remote server returned an error: (403) Forbidden")
                    || ex.Message.ToString().Contains("An error occurred while sending the request. The server returned an invalid or unrecognized response")
                )
                {
                    log = "IMENIK HR --> private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjesto(string link) --> responseFromServer PROXY EXCEPTION --> link" + link + Environment.NewLine + ex.Message.ToString();
                    IsCaptcha = true;
                }
                else if (ex.Message.ToString() != "The remote server returned an error: (500) Internal Server Error.")
                {
                    log = "IMENIK HR --> private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjesto(string link) --> responseFromServer TARGET SERVER EXCEPTION --> link" + link + Environment.NewLine + ex.Message.ToString();
                    saveSessionLog(ex);
                }
                else
                {
                    log = "IMENIK HR --> private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjesto(string link) --> responseFromServer TARGET SERVER UNHANDLED EXCEPTION --> link" + link + Environment.NewLine + ex.Message.ToString();
                    IsCaptcha = true;
                }
            }
            if (!string.IsNullOrEmpty(log))
            {
                saveSessionLog(log, false);
            }
            return t;
        }

        private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjestoWithProxyServers(ref string proxyIP, ref string userAgent, ref int rezulatPretrage, string ulicaNaziv, string mjestoNaziv, string gradNaziv, int currentResultsPage)
        {
            Tuple<CookieCollection, HtmlNodeCollection> t = new Tuple<CookieCollection, HtmlNodeCollection>(null, null);
            bool useGradInsteadMjesto = false;
            bool isCaptcha = false;
            t = GetUsersForUlicaMjesto(this._link + "/trazi/" + currentResultsPage.ToString() + "/" + "ulica:" + System.Web.HttpUtility.UrlEncode(ulicaNaziv) + "%20mjesto:" + System.Web.HttpUtility.UrlEncode(mjestoNaziv) + ".html", null, ref proxyIP, ref userAgent, this._timeBetweenHTTPRequests_MS, ref isCaptcha, ref useGradInsteadMjesto, ref rezulatPretrage, currentResultsPage);
            if (rezulatPretrage != -666)
            {
                if (isCaptcha == false && useGradInsteadMjesto)
                {
                    t = GetUsersForUlicaMjesto(this._link + "/trazi/" + currentResultsPage.ToString() + "/" + "ulica:" + System.Web.HttpUtility.UrlEncode(ulicaNaziv) + "%20mjesto:" + System.Web.HttpUtility.UrlEncode(gradNaziv) + ".html", null, ref proxyIP, ref userAgent, this._timeBetweenHTTPRequests_MS, ref isCaptcha, ref useGradInsteadMjesto, ref rezulatPretrage, currentResultsPage);
                    while (isCaptcha == true && proxyIP != "")
                    {
                        proxyIP = getProxyServer();
                        userAgent = getUserAgent();
                        t = GetUsersForUlicaMjesto(this._link + "/trazi/" + currentResultsPage.ToString() + "/" + "ulica:" + System.Web.HttpUtility.UrlEncode(ulicaNaziv) + "%20mjesto:" + System.Web.HttpUtility.UrlEncode(gradNaziv) + ".html", null, ref proxyIP, ref userAgent, this._timeBetweenHTTPRequests_MS, ref isCaptcha, ref useGradInsteadMjesto, ref rezulatPretrage, currentResultsPage);
                    }
                }
                while (rezulatPretrage != -666 && isCaptcha == true && proxyIP != "")
                {
                    proxyIP = getProxyServer();
                    userAgent = getUserAgent();
                    if (!string.IsNullOrEmpty(proxyIP))
                    {
                        t = GetUsersForUlicaMjesto(this._link + "/trazi/" + currentResultsPage.ToString() + "/" + "ulica:" + System.Web.HttpUtility.UrlEncode(ulicaNaziv) + "%20mjesto:" + (useGradInsteadMjesto == true ? System.Web.HttpUtility.UrlEncode(gradNaziv) : System.Web.HttpUtility.UrlEncode(mjestoNaziv)) + ".html", null, ref proxyIP, ref userAgent, this._timeBetweenHTTPRequests_MS, ref isCaptcha, ref useGradInsteadMjesto, ref rezulatPretrage, currentResultsPage);
                        if (t == null && useGradInsteadMjesto == false)
                        {
                            t = GetUsersForUlicaMjesto(this._link + "/trazi/" + currentResultsPage.ToString() + "/" + "ulica:" + System.Web.HttpUtility.UrlEncode(ulicaNaziv) + "%20mjesto:" + System.Web.HttpUtility.UrlEncode(gradNaziv) + ".html", null, ref proxyIP, ref userAgent, this._timeBetweenHTTPRequests_MS, ref isCaptcha, ref useGradInsteadMjesto, ref rezulatPretrage, currentResultsPage);
                        }
                        while (isCaptcha == true && proxyIP != "")
                        {
                            proxyIP = getProxyServer();
                            userAgent = getUserAgent();
                            t = GetUsersForUlicaMjesto(this._link + "/trazi/" + currentResultsPage.ToString() + "/" + "ulica:" + System.Web.HttpUtility.UrlEncode(ulicaNaziv) + "%20mjesto:" + System.Web.HttpUtility.UrlEncode(gradNaziv) + ".html", null, ref proxyIP, ref userAgent, this._timeBetweenHTTPRequests_MS, ref isCaptcha, ref useGradInsteadMjesto, ref rezulatPretrage, currentResultsPage);
                        }
                    }
                    else
                    {
                        saveSessionLog("PROXY IP NULL!", false);
                    }
                }
            }
            return t;
        }

        private void processUlicaMjesto()
        {
            lock (_tblNeprocesiraneUlice)
            {
                _threadsRunningCount++;
            }

            string proxyIP = getProxyServer();
            string userAgent = getUserAgent();
            bool isUlicaProcessed = false;

            do
            {
                int id_Ulica = -1;
                string mjestoNaziv = "";
                string ulicaNaziv = "";
                string gradNaziv = "";

                lock (_tblNeprocesiraneUlice)
                {
                    if (_currentRow < _totalRows - 1)
                    {
                        if (isUlicaProcessed)
                        {
                            _currentRow++;
                        }
                        if (_currentRow == -1)
                        {
                            _currentRow++;
                        }
                        id_Ulica = Convert.ToInt32(_tblNeprocesiraneUlice.Rows[_currentRow]["id_Ulica"]);
                        mjestoNaziv = _tblNeprocesiraneUlice.Rows[_currentRow]["NaseljeNaziv"].ToString();
                        ulicaNaziv = _tblNeprocesiraneUlice.Rows[_currentRow]["UlicaNaziv"].ToString();
                        gradNaziv = _tblNeprocesiraneUlice.Rows[_currentRow]["GradNaziv"].ToString();
                    }
                }
                string log = (!string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.Name) ? System.Threading.Thread.CurrentThread.Name.ToString() : "t_nr_1") + " --> proxy: " + proxyIP + ", user agent:" + userAgent + " --> id_Ulica = " + id_Ulica.ToString() + ", mjesto: " + mjestoNaziv + "/" + gradNaziv + ", ulica: " + ulicaNaziv;
                Console.WriteLine("\r{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " --> " + log);
                if (id_Ulica != -1)
                {
                    int rezultatPretrage = 1;
                    Tuple<CookieCollection, HtmlNodeCollection> t = GetUsersForUlicaMjestoWithProxyServers(ref proxyIP, ref userAgent, ref rezultatPretrage, ulicaNaziv, mjestoNaziv, gradNaziv, 1);
                    if (rezultatPretrage != -1 && t != null)
                    {
                        if ((HtmlNodeCollection)t.Item2 != null)
                        {
                            snimiOsobeFirme((HtmlNodeCollection)t.Item2, id_Ulica, (CookieCollection)t.Item1, ref proxyIP, ref userAgent, this._timeBetweenHTTPRequests_MS);
                        }
                        rezultatPretrage = rezultatPretrage / 20;
                        for (int i = 2; i <= rezultatPretrage; i++)
                        {
                            t = GetUsersForUlicaMjestoWithProxyServers(ref proxyIP, ref userAgent, ref rezultatPretrage, ulicaNaziv, mjestoNaziv, gradNaziv, i);
                            if (t != null && t.Item2 != null && ((HtmlNodeCollection)t.Item2).Count != 0)
                            {
                                snimiOsobeFirme((HtmlNodeCollection)t.Item2, id_Ulica, (CookieCollection)t.Item1, ref proxyIP, ref userAgent, this._timeBetweenHTTPRequests_MS);
                            }
                            else
                            {
                                saveSessionLog((!string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.Name) ? System.Threading.Thread.CurrentThread.Name.ToString() : "t_nr_1") + "<--> IMENIKHR -->  Tuple<CookieCollection, HtmlNodeCollection> t = NULL -- > LINK -->" + this._link + "/trazi/" + i.ToString() + "/" + "ulica:" + ulicaNaziv + "%20mjesto:" + mjestoNaziv + ".html --> log: " + log, false);
                            }
                        }
                        isUlicaProcessed = true;
                    }
                    else if (rezultatPretrage == -666)
                    {
                        isUlicaProcessed = true;
                    }
                    if (isUlicaProcessed)
                    {
                        using (SqlConnection conn = new SqlConnection(_connectionString))
                        {
                            try
                            {
                                conn.Open();
                                SqlCommand cmd = new SqlCommand("spUlica_SetProcesirano_Update", conn);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@id_Ulica", id_Ulica);
                                cmd.Parameters.AddWithValue("@id_WebScraperSession", this._id_WebScraperSession);
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                string exc = exceptionToString(ex);
                                saveSessionLog((!string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.Name) ? System.Threading.Thread.CurrentThread.Name.ToString() : "t_nr_1") + "<--> IMENIKHR --> id_Ulica == -1 -->" + exc, true);
                            }
                            finally
                            {
                                if (conn.State == ConnectionState.Open)
                                {
                                    conn.Close();
                                }
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(_timeBetweenHTTPRequests_MS);
                }
            }
            while (_currentRow < _totalRows - 1);

            lock (_tblNeprocesiraneUlice)
            {
                _threadsRunningCount--;
            }
        }

        private void runScraper()
        {
            startSession();
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
            finishSession();
        }

    }
}
