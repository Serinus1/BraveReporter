using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BraveIntelReporter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        internal void GetConfig()
        {
            WebClient client = new WebClient();
            client.DownloadFile("http://serinus.us/eve/intelConfig.xml", "intelConfig.xml");

        }
        internal List<File> GetIntelLogFiles()
        {
            // Get files with correct name and greatest timestamp <= now.
            var files = new DirectoryInfo(this.Path)
                    .GetFiles(this.Name + "_*.txt", SearchOption.TopDirectoryOnly);
        }
    }
}
