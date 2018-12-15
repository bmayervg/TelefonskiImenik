using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace TelefonskiImenik
{
    static class Program
    {
        public static string _connectionString;
        public static string _userAgentListFilePath;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            _connectionString = ConfigurationManager.AppSettings["connectionString"];
            _userAgentListFilePath = ConfigurationManager.AppSettings["userAgentList"];
            if (string.IsNullOrEmpty(_connectionString))
            {
                MessageBox.Show("connectionString NOT INITIALIZED -> app.config!");
                Application.Exit();
            }
            else if (string.IsNullOrEmpty(_userAgentListFilePath))
            {
                MessageBox.Show("userAgentList NOT INITIALIZED -> app.config!");
                Application.Exit();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
        }

    }
}
