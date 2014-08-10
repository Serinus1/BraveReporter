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
        public List<FileInfo> FilesToMonitor;
        public DateTime LastTimeReported = DateTime.Now;
        public Dictionary<FileInfo, string> LastLinesReported = new Dictionary<FileInfo, string>();
        public bool EveIsRunning = false;


        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Configuration.GetConfig();
            if (Configuration.FirstRun)
            {
                new frmSettings().ShowDialog();
            }
            GetIntelLogFiles();
            timer.Interval = Configuration.MonitorFrequency;
            timer.Start();
        }



        internal void GetIntelLogFiles()
        {
            FilesToMonitor = new List<FileInfo>();
            // Get files with correct name and greatest timestamp <= now.
            foreach (string roomname in Configuration.RoomsToMonitor)
            {
                FileInfo[] files = new DirectoryInfo(Configuration.LogDirectory)
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
            FileSystemWatcher watcher = new FileSystemWatcher(Configuration.LogDirectory);
            watcher.NotifyFilter = NotifyFilters.CreationTime;
            watcher.Created += new FileSystemEventHandler(FileCreated);
            watcher.EnableRaisingEvents = true;
            lblMonitoringFiles.Text = string.Empty;
            foreach (FileInfo fi in FilesToMonitor) lblMonitoringFiles.Text += fi.Name + "\r\n";
            
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            GetIntelLogFiles();
        }

        private void ReportIntel(string lastline, string status = "")
        {
            Encoding myEncoding = System.Text.UTF8Encoding.UTF8;
            WebClient client = new WebClient();
            try
            {
                if (lastline.Contains("EVE System > Channel MOTD:")) return;
                lastline = lastline.Replace('"', '\'');
                string postMessage = new ReportLine(lastline, status).ToJson();

                byte[] KiuResponse = client.UploadData(Configuration.ReportServer, "PUT", myEncoding.GetBytes(postMessage));
                Debug.Write("Kiu << " + postMessage);
                Debug.Write("\nKiu >> " + myEncoding.GetString(KiuResponse));

                if (myEncoding.GetString(KiuResponse) == "OK\n") reported++;
            }
            catch (Exception ex)
            {
                failed++;
                if (ex.Message == "The remote server returned an error: (401) Unauthorized.")
                    txtIntel.AppendText("Authorization Token Invalid.  Try refreshing your auth token in settings.\r\n");
                else if (ex.Message == "The remote server returned an error: (426) 426.")
                    txtIntel.AppendText("Client version not supported.  Please close and restart application to update. (May require two restarts.)\r\n");
                else
                    txtIntel.AppendText(string.Format("Intel Server Error: {0}\r\n", ex.Message));
                Debug.Write(string.Format("Exception: {0}", ex.Message));
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // Check EVE Program Status
            var processes = Process.GetProcesses().Where(p => p.ProcessName.ToLower() == "exefile").ToList();
            if (!EveIsRunning && processes.Count == 0) return; // Eve still isn't running and we know it.
            if (processes.Count == 0 && EveIsRunning) // We think EVE is running, but it's not.
            {
                // EVE is not running
                timer.Interval = 300000;
                EveIsRunning = false;
                ReportIntel(string.Empty, "stop");
                txtIntel.AppendText("EVE is not running.  Will recheck for EVE process on 5 minute intervals. \r\n");
                return;
            }
            if (!EveIsRunning && processes.Count > 0) // We didn't think EVE was running, but it is.
            {
                timer.Interval = Configuration.MonitorFrequency;
                txtIntel.AppendText("EVE started.\r\n");
                ReportIntel(string.Empty, "start");
                EveIsRunning = true;
                GetIntelLogFiles();
            }

            if ((DateTime.UtcNow - LastTimeReported).TotalMinutes > 5)
            {
                GetIntelLogFiles();
                ReportIntel(string.Empty, "running");
                LastTimeReported = DateTime.UtcNow;
            }
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
                    if (line.Length > 0) line = line.Remove(0, 1);
                    if (!line.StartsWith("[ 20")) continue;
                    if ((DateTime.UtcNow - LastTimeReported).TotalMinutes > 2) LastLinesReported[logfile] = string.Empty;
                    double timeFromNow = (DateTime.Parse(line.Substring(2, 19)) - DateTime.UtcNow).TotalMinutes;
                    if (Math.Abs(timeFromNow) > 2) continue; // Don't report intel that hasn't happened in the last 2 minutes.
                    if (line == LastLinesReported[logfile] || LastLinesReported[logfile] == string.Empty)
                        lastlinefound = true;
                    if (lastlinefound && line != LastLinesReported[logfile]) // we've past the last line reported
                    {
                        LastLinesReported[logfile] = line;
                        LastTimeReported = DateTime.Parse(line.Substring(2, 19));
                        ReportIntel(line);
                        txtIntel.AppendText("[" + line.Substring(13) + "\r\n");
                    }
                }
                
                // Clean up
                logFileReader.Close();
                logFileStream.Close();
                
            }
        }

        private void frmMain_ResizeEnd(object sender, EventArgs e)
        {

        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipText = "Minimized to system tray (still reporting).";
                notifyIcon1.ShowBalloonTip(500);
                this.ShowInTaskbar = false;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                notifyIcon1.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = false;
        }

        private void label4_Click(object sender, EventArgs e)
        {
            
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmAbout().ShowDialog();
        }

        private void mnuViewMap_Click(object sender, EventArgs e)
        {
            if (Configuration.MapURL != string.Empty)
            {
                ProcessStartInfo sInfo = new ProcessStartInfo(Configuration.MapURL);
                Process.Start(sInfo);
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            ReportIntel(string.Empty, "stop");
        }

        private void mnuSettings_Click(object sender, EventArgs e)
        {
            new frmSettings().ShowDialog();
        }


    }
}
