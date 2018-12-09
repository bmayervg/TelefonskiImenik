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

namespace TelefonskiImenik.Forms
{
    public partial class frmImenikPretraga : Form
    {
        public frmImenikPretraga()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            HtmlNodeCollection nodes = new HtmlNodeCollection(null);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://www.hakom.hr/default.aspx?id=8391");
            request.Method = "POST";
            request.ReadWriteTimeout = 5000;//200000;

            request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64; Trident / 7.0; rv: 11.0) like Gecko";
            var postData = "brojTel:385916121237";
            postData += "&_validacija:crna";
            postData += "&_valid:crna";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "text/html; charset=utf-8";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            //if (!string.IsNullOrEmpty(proxyIP))
            //{
            //    WebProxy wp = new WebProxy(proxyIP);
            //    wp.BypassProxyOnLocal = true;
            //    request.Timeout = requestTimeout + 5000;
            //    request.Proxy = wp;
            //    request.KeepAlive = false;
            //    request.ProtocolVersion = HttpVersion.Version10;
            //    request.AllowWriteStreamBuffering = false;
            //}
            //CookieContainer cc = new CookieContainer();
            //request.CookieContainer = cc;
            //if (cookies != null && cookies.Count != 0)
            //{
            //    for (int ck = 0; ck < cookies.Count; ck++)
            //    {
            //        Cookie c = new Cookie();
            //        c.Secure = cookies[ck].Secure;
            //        c.Port = cookies[ck].Port;
            //        c.Path = cookies[ck].Path;
            //        c.Name = cookies[ck].Name;
            //        c.HttpOnly = cookies[ck].HttpOnly;
            //        c.Expires = cookies[ck].Expires;
            //        c.Domain = cookies[ck].Domain;
            //        c.Value = cookies[ck].Value;
            //        c.Discard = cookies[ck].Discard;
            //        c.CommentUri = cookies[ck].CommentUri;
            //        c.Comment = cookies[ck].Comment;
            //        c.Expired = cookies[ck].Expired;
            //        c.Version = cookies[ck].Version;
            //        cc.Add(c);
            //    }
            //}
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

            if (!string.IsNullOrEmpty(responseFromServer))
            {

            }
        }
    }
}
