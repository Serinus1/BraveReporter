using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace BraveIntelReporter
{
    public partial class frmMain : Form
    {
        long reported = 0;
        long failed = 0;
        public List<string> RoomsToMonitor;
        public List<FileInfo> FilesToMonitor;
        public int MonitorFrequency = 500;
        public Dictionary<FileInfo, string> LastLinesReported = new Dictionary<FileInfo, string>();
        public Uri ReportServer;
        private static readonly string defaultLogDirectory = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EVE", "logs", "Chatlogs");


        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            GetConfig();
            GetIntelLogFiles();
            timer.Interval = MonitorFrequency;
            timer.Start();
        }

        internal void GetConfig()
        {
            WebClient client = new WebClient();
            client.DownloadFile("http://serinus.us/eve/intelGlobalConfig.xml", "intelGlobalConfig.xml");
            RoomsToMonitor = new List<string>();
            XmlDocument configFile = new XmlDocument();
            configFile.Load("intelGlobalConfig.xml");
            foreach (XmlNode node in configFile.SelectNodes("BraveReporterSettings/chatrooms/chatroom"))
                if (node.Attributes["type"].InnerText == "intel") RoomsToMonitor.Add(node.InnerText);
            ReportServer = new Uri(configFile.SelectSingleNode("BraveReporterSettings/IntelServer").InnerText);
            ReportServer = new Uri(configFile.SelectSingleNode("BraveReporterSettings/MonitorFrequency").InnerText);
        }

        internal void GetIntelLogFiles()
        {
            FilesToMonitor = new List<FileInfo>();
            // Get files with correct name and greatest timestamp <= now.
            foreach (string roomname in RoomsToMonitor)
            {
                FileInfo[] files = new DirectoryInfo(defaultLogDirectory)
                        .GetFiles(roomname + "_*.txt", SearchOption.TopDirectoryOnly);
                files = files.OrderByDescending(f => f.LastWriteTime).ToArray();
                if (files.Count() > 0) FilesToMonitor.Add(files[0]);  
            }
            
            // Synchronize List and Dictionary without losing existing LastLinesReported strings
            foreach (FileInfo logfile in FilesToMonitor) // Add any new ones
                if (!LastLinesReported.ContainsKey(logfile)) LastLinesReported.Add(logfile, string.Empty);
            // And delete any old ones
            List<FileInfo> removethese = LastLinesReported.Keys.Where(key => !FilesToMonitor.Contains(key)).ToList();
            foreach (FileInfo fi in removethese) LastLinesReported.Remove(fi);

            // If a new file is created, recheck to make sure we have the most up to date log files.
            FileSystemWatcher watcher = new FileSystemWatcher(defaultLogDirectory);
            watcher.NotifyFilter = NotifyFilters.CreationTime;
            watcher.Created += new FileSystemEventHandler(FileCreated);
            watcher.EnableRaisingEvents = true;
            
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            GetIntelLogFiles();
        }

        private void ReportIntel(string lastline)
        {
            try
            {
                // Also send the message to Kiu Nakamura's Brave Server
                Encoding myEncoding = System.Text.ASCIIEncoding.UTF8;
                string postMessage = lastline;
                    //string.Format("[ {0} ]{1}\n", timestamp.ToString("yyyy.MM.dd HH:mm:ss"), message);

                WebClient client = new WebClient();
                byte[] KiuResponse = client.UploadData(new Uri("http://eve.501gu.de/BIntel/intel"), "PUT", myEncoding.GetBytes(postMessage));
                Debug.Write("Kiu << " + postMessage);
                Debug.Write("Kiu >> " + myEncoding.GetString(KiuResponse));
                reported++;
            }
            catch (Exception ex)
            {
                failed++;
                Debug.Write(string.Format("Exception: {0}", ex.Message));
            }

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lblReported.Text = reported.ToString();
            lblFailed.Text = failed.ToString();
            foreach (FileInfo logfile in FilesToMonitor)
            {
                bool lastlinefound = false;

                FileStream logFileStream = new FileStream(logfile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader logFileReader = new StreamReader(logFileStream);

                while (!logFileReader.EndOfStream)
                {
                    string line = logFileReader.ReadLine();
                    if (!line.StartsWith("[ 20")) continue;
                    double timeFromNow = (DateTime.Parse(line.Substring(2, 19)) - DateTime.UtcNow).TotalMinutes;
                    if (Math.Abs(timeFromNow) > 5) continue; // Don't report intel that hasn't happened in the last 5 minutes.
                    if (line == LastLinesReported[logfile] || LastLinesReported[logfile] == string.Empty)
                        lastlinefound = true;
                    if (lastlinefound && line != LastLinesReported[logfile]) // we've past the last line reported
                    {
                        LastLinesReported[logfile] = line;
                        ReportIntel(line + "\n");
                        txtIntel.Text = txtIntel.Text + line + "\r\n";
                    }
                }

                // Clean up
                logFileReader.Close();
                logFileStream.Close();
                
            }
        }
    }
}
