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
using System.Timers;
using System.Windows.Forms;
using System.Xml;

namespace BraveIntelReporter
{
    public partial class frmMain : Form
    {
        long reported = 0;
        long failed = 0;

        private DateTime LastGlobalConfigCheck = DateTime.Now;

        private STATE state = STATE.INIT;
        private System.Timers.Timer timerEve;
        private System.Timers.Timer timerFileDiscover;
        private System.Timers.Timer timerFileReader;
        private bool eveRunningLast = false;
        private Dictionary<String, FileInfo> roomToFile = new Dictionary<String, FileInfo>();
        private Dictionary<String, String> roomToLastLine = new Dictionary<String, String>();

        enum STATE
        {
            INIT, START, RUNNING, STOP
        };

        private Boolean isEveRunning()
        {
            return (Process.GetProcesses().Where(p => p.ProcessName.ToLower() == "exefile").ToList().Count() != 0);
        }

        private void updateLatestIntelFiles()
        {
            foreach (String roomName in Configuration.RoomsToMonitor)
            {
                Debug.WriteLine("KIU Checking for : " + roomName);

                FileInfo[] files = new DirectoryInfo(Configuration.LogDirectory)
                        .GetFiles(roomName + "_*.txt", SearchOption.TopDirectoryOnly);
                FileInfo fi = files.OrderByDescending(f => f.LastWriteTime).First();
                if (fi != null)
                {
                    roomToFile[roomName] = fi;
                    Debug.WriteLine("KIU Found: " + fi);
                }
            }
        }

        private void setState(STATE nState)
        {
            if (state == nState)
            {
                return;
            }

            state = nState;
            Debug.WriteLine("KIU STATE: " + nState);

            if (STATE.START == nState)
            {
                timerFileDiscover.Start();
                updateLatestIntelFiles();

                timerFileReader.Start();
            }
            if (STATE.STOP == nState)
            {
                timerFileDiscover.Stop();
                timerFileReader.Stop();
            }
        }

        private void init()
        {
            timerEve = new System.Timers.Timer();
            timerEve.Elapsed += new ElapsedEventHandler(execEveTimer);
            timerEve.Interval = 10000;
            timerEve.Start();
           
            timerFileDiscover = new System.Timers.Timer();
            timerFileDiscover.Elapsed += new ElapsedEventHandler(execFileDiscoverTimer);
            timerFileDiscover.Interval = 60000;

            timerFileReader = new System.Timers.Timer();
            timerFileReader.Elapsed += new ElapsedEventHandler(execFileReaderTimer);
            timerFileReader.Interval = 1000;
        }

        private void execEveTimer(object sender, EventArgs e)
        {
            Boolean eveRunning = isEveRunning();
            if (eveRunning == eveRunningLast)
            {
                return;
            }
            eveRunningLast = eveRunning;
            if (eveRunning)
            {
                setState(STATE.START);
            }
            else
            {
                setState(STATE.STOP);
            }
        }

        private void execFileDiscoverTimer(object sender, EventArgs e)
        {
            updateLatestIntelFiles();
        }

        private void execFileReaderTimer(object sender, EventArgs e)
        {
            FileStream logFileStream;
            StreamReader logFileReader;

            String line;
            String lastLine;
            Boolean lastLineFound;

            foreach (String roomName in Configuration.RoomsToMonitor)
            {
                FileInfo logfile = null;
                roomToFile.TryGetValue(roomName, out logfile);
                if (logfile == null)
                {
                    Debug.WriteLine("KIU Skipping room: " + roomName);
                    continue;
                }
                logFileStream = new FileStream(logfile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                logFileReader = new StreamReader(logFileStream);
                                
                line = null;                
                lastLineFound = false;
                roomToLastLine.TryGetValue(roomName, out lastLine);

                Debug.WriteLine("KIU Checking: " + logfile + " -- Last: " + lastLine);

                while (!logFileReader.EndOfStream)
                {
                    line = logFileReader.ReadLine();
                    if (line.Trim().Length == 0)
                    {
                        continue;
                    }

                    line = line.Remove(0, 1);

                    if (!lastLineFound)
                    {                        
                        if (line == lastLine)
                        {
                            lastLineFound = true;
                        }
                        continue;
                    }
                    
                    roomToLastLine[roomName] = line;
                    appendText(line + "\r\n"); 
                    ReportIntel(line);                    
                }

                if (!lastLineFound)
                {
                    roomToLastLine[roomName] = line;
                }

                // Clean up
                logFileReader.Close();
                logFileStream.Close();

            }
        }

        private void appendText(String line)
        {
            Debug.WriteLine("KIU append: " + line);
            this.txtIntel.Invoke(new MethodInvoker(() => this.txtIntel.AppendText(line)));
        }
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

            string report = string.Empty;
            bool haveglobalsettings = false;
            while (!haveglobalsettings)
            {
                haveglobalsettings = Configuration.GetConfig(out report);
                if (!haveglobalsettings)
                {
                    txtIntel.AppendText(report);
                    txtIntel.AppendText("Waiting 30 seconds and retrying.");
                    for (int i = 1; i < 300; i++) // A lazy way of waiting 30 seconds but keeping the UI responsive without multithreading. 
                    {
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }

            if (Configuration.FirstRun)
            {
                new frmSettings().ShowDialog();
            }
            init();
            timer.Interval = Configuration.MonitorFrequency;
            timer.Start();
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

                // byte[] KiuResponse = client.UploadData(Configuration.ReportServer, "PUT", myEncoding.GetBytes(postMessage));
                byte[] KiuResponse = new byte[2];
              //  Debug.Write("Kiu << " + postMessage);
               // Debug.Write("\nKiu >> " + myEncoding.GetString(KiuResponse));

                if (myEncoding.GetString(KiuResponse) == "OK\n") reported++;
            }
            catch (Exception ex)
            {
                failed++;
                if (ex.Message == "The remote server returned an error: (401) Unauthorized.")
                    appendText("Authorization Token Invalid.  Try refreshing your auth token in settings.\r\n");
                else if (ex.Message == "The remote server returned an error: (426) 426.")
                    appendText("Client version not supported.  Please close and restart application to update. (May require two restarts.)\r\n");
                else
                    appendText(string.Format("Intel Server Error: {0}\r\n", ex.Message));
                Debug.Write(string.Format("Exception: {0}", ex.Message));
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
