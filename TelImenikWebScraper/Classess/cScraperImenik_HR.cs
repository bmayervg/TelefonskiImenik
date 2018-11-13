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

namespace TelImenikWebScraper.Classess
{
    public class cScraperImenik_HR
    {
        private int _id_WebScraperSession = -1;
        private int _timeBetweenHTTPRequests_MS = 2000;
        private string _link = "";
        private string _connectionString;
        private int _numberOfThreads = 1;

        private string _proxyServerListFilePath;
        private List<String> _proxyServerList;
        private List<String> _usedProxyServerList;

        private string _userAgentListFilePath;
        private List<String> _userAgentList;
        private List<String> _usedUserAgentList;

        private DataTable _tblNeprocesiraneUlice;
        private int _currentRow = -1;
        private int _totalRows = 0;
        private int _threadsRunningCount = 0;

        #region USER_AGENT

        private void loadUsqerAgentListFromFile()
        {
            Console.WriteLine("OBTAINING USER AGENT LIST FROM FILE");

            if (!string.IsNullOrEmpty(_userAgentListFilePath))
            {
                _userAgentList = System.IO.File.ReadAllLines(_userAgentListFilePath).ToList<String>();
            }
            Console.WriteLine("OBTAINING USER AGENT LIST FROM FILE --> DONE " );
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
            Console.WriteLine("OBTAINING PROXY SERVER LIST FROM FILE");
            if (!string.IsNullOrEmpty(_proxyServerListFilePath))
            {
                _proxyServerList = System.IO.File.ReadAllLines(_proxyServerListFilePath).ToList<String>();
                for (int i = 0; i < _proxyServerList.Count; i++)
                {
                    string ip = "";
                    int port = 0;
                    string[] serverData = _proxyServerList[i].ToString().Split(":");
                    if (serverData != null && serverData.Length == 2)
                    {
                        ip = serverData[0].ToString();
                        port = Convert.ToInt32( serverData[1]);
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
            Console.WriteLine("OBTAINING PROXY SERVER LIST FROM FILE-> DONE --> PROXY SERVER LIST COUNT" + _proxyServerList.Count.ToString() );
        }

        private string getProxyServer()
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
                        if (_proxyServerList != null && _proxyServerList.Count == 0 && _usedProxyServerList != null && _usedProxyServerList.Count != 0)
                        {
                            _proxyServerList.AddRange(_usedProxyServerList);
                            _usedProxyServerList.Clear();
                            proxyServer = _proxyServerList[0].ToString();
                            _proxyServerList.RemoveAt(0);
                        }
                    }
                }
            }
            return proxyServer;
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
                        Console.WriteLine(exceptionToString(ex));
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
                Console.WriteLine("IMENIKHR --> _connectionString NOT INITIALIZED!");
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
                        Console.WriteLine(exceptionToString(ex));
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
                Console.WriteLine("IMENIKHR --> _connectionString NOT INITIALIZED!");
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

        public cScraperImenik_HR(string connectionString, int numberOfThreads, string link, int timeBetweenHTTPRequests_MS, string proxyServerListFilePath, string userAgentListFilePath)
        {
            _proxyServerListFilePath = proxyServerListFilePath;
            _usedProxyServerList = new List<string>();
            _usedUserAgentList = new List<string>();
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
            loadProxyServerListFromFile();
            loadUsqerAgentListFromFile();
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
                    Console.WriteLine("tblNeprocesiraneUlice ROW COUNT == 0");
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
                                Console.WriteLine(exceptionToString(ex));
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
                                        Console.WriteLine(exceptionToString(ex));
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

        private void snimiOsobu(int id_Ulica, string imePrezime, string linkOsobaTelBroj, CookieCollection cookies, string proxyIP, string userAgent, int requestTimeout, ref bool IsCaptcha)
        {
            if (id_Ulica != -1 && !string.IsNullOrEmpty(linkOsobaTelBroj) && !string.IsNullOrEmpty(imePrezime))
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this._link.Replace("/imenik", "" ) + System.Web.HttpUtility.UrlEncode(linkOsobaTelBroj).Replace("%2f", "/"));
                request.Method = "GET";
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
                    request.KeepAlive = false;
                    request.Timeout = System.Threading.Timeout.Infinite;
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
                    if (responseFromServer.Contains("Odmori malo, zaslužio si..."))
                    {
                        IsCaptcha = true;
                        saveSessionLog("IMENIK HR --> SnimiOsobu(int id_Ulica, string imePrezime, string linkOsobaTelBroj, CookieCollection cookies ) --> responseFromServer IS CAPTCHA  --> REDIRECTING TO ANOTHER PROXY --> id_Ulica=" + id_Ulica.ToString() + ", imePrezime=" + imePrezime + "linkOsobaTelBroj=" + linkOsobaTelBroj, false);
                    }
                    else
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(responseFromServer);
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
                            if (oip != null && oip.Length == 2)
                            {
                                osobaIme = oip[0].ToString();
                                osobaPrezime = oip[1].ToString();
                            }
                            HtmlAgilityPack.HtmlNodeCollection htmlNodesOsobaAdresa = doc.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), ' adresa_detalj ')]");
                            if (htmlNodesOsobaAdresa != null && htmlNodesOsobaAdresa.Count > 0)
                            {
                                string[] osobaPodaci = htmlNodesOsobaAdresa[0].InnerText.Trim().Split(' ');
                                if (osobaPodaci != null && osobaPodaci.Length != 0)
                                {
                                    try
                                    {
                                        osobaPunaAdresa = htmlNodesOsobaAdresa[0].InnerText.Trim();
                                        osobaPostanskiBroj = osobaPodaci[0].ToString().Replace(",", "");
                                        osobaNaselje = osobaPodaci[1].ToString().Replace(",", "");
                                        osobaUlica = osobaPodaci[2].ToString().Replace(",", "");

                                        if (osobaPodaci.Length == 4)
                                        {
                                            osobaKucniBroj = osobaPodaci[3].ToString();
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
                                        Console.WriteLine(exceptionToString(ex));
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
                                                Console.WriteLine(exceptionToString(ex));
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
                else
                {
                    saveSessionLog("IMENIK HR --> SnimiOsobu(int id_Ulica, string imePrezime, string linkOsobaTelBroj, CookieCollection cookies ) --> doc.LoadHtml(responseFromServer) IS NULL --> " + "--> id_Ulica=" + id_Ulica.ToString() + ", imePrezime=" + imePrezime + "linkOsobaTelBroj=" + linkOsobaTelBroj, false);
                }
            }
        }

        private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjesto(string link, CookieCollection cookies, string proxyIP, string userAgent, int requestTimeout, ref bool IsCaptcha)
        {
            Tuple<CookieCollection, HtmlNodeCollection> t = null;
            IsCaptcha = false;
            try
            {
                HtmlNodeCollection nodes = new HtmlNodeCollection(null);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(link);
                request.Method = "GET";
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
                    request.Timeout = requestTimeout;
                    request.Proxy = wp;
                    request.KeepAlive = false;
                    request.Timeout = System.Threading.Timeout.Infinite;
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
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                if (!string.IsNullOrEmpty(responseFromServer) && !responseFromServer.Contains("Odmori malo, zaslužio si..."))
                {
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(responseFromServer);
                    nodes = doc.DocumentNode.SelectNodes("//div[contains(concat(' ', @class, ' '), ' adresa ')]");
                    t = new Tuple<CookieCollection, HtmlNodeCollection>(response.Cookies, nodes);
                    IsCaptcha = false;
                }
                else if (!string.IsNullOrEmpty(responseFromServer) && responseFromServer.Contains("Odmori malo, zaslužio si..."))
                {
                    IsCaptcha = true;
                    saveSessionLog("IMENIK HR --> private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjesto(string link) --> responseFromServer CAPTCHA --> link" + link, false);
                }
                else
                {
                    IsCaptcha = false;
                    saveSessionLog("IMENIK HR --> private Tuple<CookieCollection, HtmlNodeCollection> GetUsersForUlicaMjesto(string link) --> responseFromServer empty --> link" + link, false);
                }
                
                response.Close();
                response.Dispose();
                reader.Close();
                reader.Dispose();
                dataStream.Close();
                dataStream.Dispose();
            }
            catch (Exception ex)
            {
                IsCaptcha = false;
                if (ex.Message.ToString() != "The remote server returned an error: (500) Internal Server Error.")
                {
                    saveSessionLog(ex);
                }
                else
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }
            return t;
        }

        private void snimiOsobeFirme(HtmlNodeCollection nodes, int id_Ulica, CookieCollection cookies, string proxyIP, string userAgent, int requestTimeout, ref bool IsCaptcha)
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
                        if (!IsCaptcha)
                        {
                            snimiOsobu(id_Ulica, nodes[i].ChildNodes[1].Attributes[0].Value.ToString(), nodes[i].ChildNodes[1].Attributes[1].Value.ToString(), cookies, proxyIP, userAgent, this._timeBetweenHTTPRequests_MS, ref IsCaptcha);
                        }
                        else
                        {
                            i = nodes.Count - 1;
                        }
                    }
                }
            }
        }

        private void processUlicaMjesto()
        {
            lock (_tblNeprocesiraneUlice)
            {
                _threadsRunningCount++;
            }
            string proxyIP = getProxyServer();
            string userAgent = getUserAgent();
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
                string log = (!string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.Name ) ? System.Threading.Thread.CurrentThread.Name.ToString() : "t_nr_1" ) + "proxy: " + proxyIP + "user agent:" + " --> id_Ulica = " + id_Ulica.ToString() + ", mjesto: " + mjestoNaziv + ", ulica: " + ulicaNaziv;
                Console.WriteLine(log);
                bool isUlicaProcessed = false;
                if (id_Ulica != -1)
                {
                    int currentResultsPage = 1;   
                    if (!string.IsNullOrEmpty(this._link))
                    {
                        bool isCaptcha = false;
                        Tuple<CookieCollection, HtmlNodeCollection> t = GetUsersForUlicaMjesto(this._link + "/trazi/" + currentResultsPage.ToString() + "/" + "ulica:" + System.Web.HttpUtility.UrlEncode(ulicaNaziv) + "%20mjesto:" + System.Web.HttpUtility.UrlEncode(mjestoNaziv) + ".html", null, proxyIP, userAgent, this._timeBetweenHTTPRequests_MS, ref isCaptcha);
                        while (isCaptcha == true && proxyIP != "")
                        {
                            proxyIP = getProxyServer();
                            userAgent = getUserAgent();
                            t = GetUsersForUlicaMjesto(this._link + "/trazi/" + currentResultsPage.ToString() + "/" + "ulica:" + System.Web.HttpUtility.UrlEncode(ulicaNaziv) + "%20mjesto:" + System.Web.HttpUtility.UrlEncode(mjestoNaziv) + ".html", null, proxyIP, userAgent, this._timeBetweenHTTPRequests_MS, ref isCaptcha);
                        }
                        if (t != null)
                        {
                            HtmlNodeCollection nodes = (HtmlNodeCollection)t.Item2;
                            if (nodes != null && nodes.Count != 0)
                            {
                                while (nodes != null && nodes.Count != 0)
                                {
                                    snimiOsobeFirme(nodes, id_Ulica, (CookieCollection)t.Item1, proxyIP, userAgent, this._timeBetweenHTTPRequests_MS, ref isCaptcha);
                                    while (isCaptcha == true && proxyIP != "")
                                    {
                                        proxyIP = getProxyServer();
                                        userAgent = getUserAgent();
                                        currentResultsPage = 1;
                                        t = GetUsersForUlicaMjesto(this._link + "/trazi/" + currentResultsPage.ToString() + "/sve/sve/sve/vaznost/ulica:" + System.Web.HttpUtility.UrlEncode(ulicaNaziv) + "%20mjesto:" + System.Web.HttpUtility.UrlEncode(mjestoNaziv) + ".html", (CookieCollection)t.Item1, proxyIP, userAgent, this._timeBetweenHTTPRequests_MS, ref isCaptcha);
                                    }
                                    currentResultsPage++;
                                    t = GetUsersForUlicaMjesto(this._link + "/trazi/" + currentResultsPage.ToString() + "/" + "ulica:" + System.Web.HttpUtility.UrlEncode(ulicaNaziv) + "%20mjesto:" + System.Web.HttpUtility.UrlEncode(mjestoNaziv) + ".html", null, proxyIP, userAgent, this._timeBetweenHTTPRequests_MS, ref isCaptcha);
                                    while (isCaptcha == true && proxyIP != "")
                                    {
                                        proxyIP = getProxyServer();
                                        userAgent = getUserAgent();
                                        t = GetUsersForUlicaMjesto(this._link + "/trazi/" + currentResultsPage.ToString() + "/sve/sve/sve/vaznost/ulica:" + System.Web.HttpUtility.UrlEncode(ulicaNaziv) + "%20mjesto:" + System.Web.HttpUtility.UrlEncode(mjestoNaziv) + ".html", (CookieCollection)t.Item1, proxyIP, userAgent, this._timeBetweenHTTPRequests_MS, ref isCaptcha);
                                    }
                                    if (t != null && t.Item2 != null)
                                    {
                                        nodes = (HtmlNodeCollection)t.Item2;
                                    }
                                    else
                                    {
                                        nodes = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            saveSessionLog("IMENIKHR -->  Tuple<CookieCollection, HtmlNodeCollection> t = NULL -- > LINK -->" + this._link + "/trazi/" + currentResultsPage.ToString() + "/" + "ulica:" + ulicaNaziv + "%20mjesto:" + mjestoNaziv + ".html --> log: " + log, false);
                        }
                        isUlicaProcessed = true;
                    }
                    else
                    {
                        saveSessionLog("IMENIKHR --> id_Ulica == -1 -->" + log, false);
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
                                Console.WriteLine(exceptionToString(ex));
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
            //// progress bar
            //int nextPercentage = 5;
            //Console.WriteLine();
            //Console.WriteLine("|-------------------| Running " + _numberOfThreads.ToString() + " threads on " + _totalRows.ToString() + " items");
            //Console.Write("|");
            //while (_currentRow < _totalRows - 1 || _threadsRunningCount > 0)
            //{
            //    if (_currentRow != 0)
            //    {
            //        while (_currentRow * 100 / _currentRow > nextPercentage)
            //        {
            //            Console.Write(".");
            //            nextPercentage += 5;
            //        }
            //    }
            //    Thread.Sleep(1000);
            //}
            //while (nextPercentage < 100)
            //{
            //    Console.Write(".");
            //    nextPercentage += 5;
            //}
            //Console.WriteLine("|");
            finishSession();
        }

    }
}
