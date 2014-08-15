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
        /// <summary>
        /// How often to check for changes in the log file in milliseconds.  Can be set in IntelReporterLocalSettings.xml. 
        /// </summary>
        public static int MonitorFrequency = 500;
        /// <summary>
        /// Frequency to check the server for global configuration updates in minutes.
        /// </summary>
        public static int ConfigCheckFrequency = 30;
        /// <summary>
        /// URL of the intel map.
        /// </summary>
        public static string MapURL = string.Empty; 
        /// <summary>
        /// Server to relay intel to.
        /// </summary>
        public static Uri ReportServer;
        /// <summary>
        /// Default directory of eve chat logs
        /// </summary>
        public static readonly string DefaultLogDirectory = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EVE", "logs", "Chatlogs");
        /// <summary>
        /// Directory of eve chat logs
        /// </summary>
        public static string LogDirectory = DefaultLogDirectory;
        /// <summary>
        /// Room names to search for in the chat directory
        /// </summary>
        public static List<string> RoomsToMonitor;
        /// <summary>
        /// Retrieved from the server side webpage by the user.  Token that confirms identity with core.
        /// </summary>
        public static string AuthToken = string.Empty;
        /// <summary>
        /// Run on windows startup
        /// </summary>
        public static bool RunOnStartup = false;
        /// <summary>
        /// Windows startup registry key
        /// </summary>
        public static RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        /// <summary>
        /// Determines if this is the first time the user has run the application
        /// </summary>
        public static bool FirstRun = true;
        /// <summary>
        /// Folder to save application configuration files.
        /// </summary>
        public static string MyFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EVE", "BraveIntelReporter");

        public static bool GetConfig(out string report)
        {
            if (!Directory.Exists(MyFolder)) Directory.CreateDirectory(MyFolder);
            report = string.Empty;
            string localreport = string.Empty;
            string globalreport = string.Empty;
            GetLocalConfig(out localreport);
            bool globalsuccessful = GetGlobalConfig(out globalreport);
            report = localreport + "\r\n" + globalreport;
            return globalsuccessful;
        }

        private static void GetLocalConfig(out string report)
        {
            
            if (File.Exists(System.IO.Path.Combine(MyFolder, "IntelReporterLocalSettings.xml")))
            {
                FirstRun = false;
                XmlDocument configFile = new XmlDocument();
                configFile.Load(System.IO.Path.Combine(MyFolder, "IntelReporterLocalSettings.xml"));
                if (configFile.SelectSingleNode("BraveReporterSettings/LogDirectory") != null)
                    LogDirectory = configFile.SelectSingleNode("BraveReporterSettings/LogDirectory").InnerText;
                if (configFile.SelectSingleNode("BraveReporterSettings/AuthToken") != null)
                    AuthToken = configFile.SelectSingleNode("BraveReporterSettings/AuthToken").InnerText;
                if (rkApp.GetValue("BraveIntelReporter") != null)
                    RunOnStartup = true;
                report = "Loaded local settings.";
            }
            report = "Local settings not found. Using defaults.";
        }
        internal static bool GetGlobalConfig(out string report)
        {
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile("http://serinus.us/eve/intelGlobalConfig.xml", System.IO.Path.Combine(MyFolder, "intelGlobalConfig.xml"));
                report = "Global config updated.";
            }
            catch (Exception ex)
            {
                if (!File.Exists(System.IO.Path.Combine(MyFolder, "intelGlobalConfig.xml")))
                {
                    report = "Could not load global config file.";
                    return false;
                }
                else report = "Failed to retrieve global config.  Using last known settings.";
            }
            RoomsToMonitor = new List<string>();
            XmlDocument configFile = new XmlDocument();
            configFile.Load(System.IO.Path.Combine(MyFolder, "intelGlobalConfig.xml"));
            foreach (XmlNode node in configFile.SelectNodes("BraveReporterSettings/chatrooms/chatroom"))
                if (node.Attributes["type"].InnerText == "intel") RoomsToMonitor.Add(node.InnerText);
            ReportServer = new Uri(configFile.SelectSingleNode("BraveReporterSettings/IntelServer").InnerText);
            MonitorFrequency = int.Parse(configFile.SelectSingleNode("BraveReporterSettings/MonitorFrequency").InnerText);
            if (configFile.SelectSingleNode("BraveReporterSettings/MapLink") != null)
                MapURL = configFile.SelectSingleNode("BraveReporterSettings/MapLink").InnerText;
            return true;
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

            localSettings.Save(System.IO.Path.Combine(MyFolder, "IntelReporterLocalSettings.xml"));

        }
    }


}
