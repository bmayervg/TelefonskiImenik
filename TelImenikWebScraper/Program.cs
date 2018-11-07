using System;
using System.Configuration;
using TelImenikWebScraper.Classess;

namespace TelImenikWebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            cScraperImenik_HR s = new cScraperImenik_HR(ConfigurationManager.AppSettings["connectionString"].ToString(), 1, "http://www.imenik.hr/imenik", 3000);
            s.Start();
            Console.ReadLine();
        }
    }
}
