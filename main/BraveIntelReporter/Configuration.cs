using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace BraveIntelReporter
{
    public static class Configuration
    {
        public static int MonitorFrequency = 500;
        public static string MapURL = string.Empty; 
        public static Uri ReportServer;
        public static readonly string DefaultLogDirectory = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EVE", "logs", "Chatlogs");
        public static string LogDirectory = DefaultLogDirectory;
        public static List<string> RoomsToMonitor;
        public static string AuthToken = string.Empty;
        public static bool RunOnStartup = false;
        public static RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public static void GetConfig()
        {
            GetGlobalConfig();
            GetLocalConfig();
        }

        private static void GetLocalConfig()
        {
            if (File.Exists("IntelReporterLocalSettings.xml"))
            {
                XmlDocument configFile = new XmlDocument();
                configFile.Load("IntelReporterLocalSettings.xml");
                if (configFile.SelectSingleNode("BraveReporterSettings/LogDirectory") != null)
                    LogDirectory = configFile.SelectSingleNode("BraveReporterSettings/LogDirectory").InnerText;
                if (configFile.SelectSingleNode("BraveReporterSettings/AuthToken") != null)
                    AuthToken = configFile.SelectSingleNode("BraveReporterSettings/AuthToken").InnerText;
                if (rkApp.GetValue("BraveIntelReporter") != null)
                    RunOnStartup = true;
            }
        }
        private static void GetGlobalConfig()
        {
            WebClient client = new WebClient();
            client.DownloadFile("http://serinus.us/eve/intelGlobalConfig.xml", "intelGlobalConfig.xml");
            RoomsToMonitor = new List<string>();
            XmlDocument configFile = new XmlDocument();
            configFile.Load("intelGlobalConfig.xml");
            foreach (XmlNode node in configFile.SelectNodes("BraveReporterSettings/chatrooms/chatroom"))
                if (node.Attributes["type"].InnerText == "intel") RoomsToMonitor.Add(node.InnerText);
            ReportServer = new Uri(configFile.SelectSingleNode("BraveReporterSettings/IntelServer").InnerText);
            MonitorFrequency = int.Parse(configFile.SelectSingleNode("BraveReporterSettings/MonitorFrequency").InnerText);
            if (configFile.SelectSingleNode("BraveReporterSettings/MapLink") != null)
                MapURL = configFile.SelectSingleNode("BraveReporterSettings/MapLink").InnerText;
        }

        internal static void Save(bool runOnStartup)
        {
            if (runOnStartup != Configuration.RunOnStartup)
            {
                if (runOnStartup) 
                    rkApp.SetValue("BraveIntelReporter", Application.ExecutablePath.ToString());
                else 
                    rkApp.DeleteValue("BraveIntelReporter", false);
                Configuration.RunOnStartup = runOnStartup;
            }
            XmlDocument localSettings = new XmlDocument();
            XmlNode rootNode = localSettings.CreateElement("BraveReporterSettings");
            localSettings.AppendChild(rootNode);

            XmlNode logdir = localSettings.CreateElement("LogDirectory");
            logdir.InnerText = Configuration.LogDirectory;
            rootNode.AppendChild(logdir);

            XmlNode authtoken = localSettings.CreateElement("AuthToken");
            authtoken.InnerText = Configuration.AuthToken;
            rootNode.AppendChild(authtoken);

            localSettings.Save("IntelReporterLocalSettings.xml");

        }
    }


}
