using System;
using System.Configuration;
using TelImenikWebScraper.Classess;

namespace TelImenikWebScraper
{
    class Program
    {
        static void Main(string[] args)
        {

            cScraperImenik_HR s = new cScraperImenik_HR(ConfigurationManager.AppSettings["connectionString"].ToString(), Convert.ToInt32(ConfigurationManager.AppSettings["imenikHRWSNumberOfThreads"]), ConfigurationManager.AppSettings["imenikHRLink"].ToString(), Convert.ToInt32(ConfigurationManager.AppSettings["imenikHRWSleepTimeBetweenThreads"]));
            s.Start();
            
        }
    }
}
