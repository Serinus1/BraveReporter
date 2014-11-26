using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BraveIntelReporter
{
    public class ReportLine
    {
        string IntelLine = string.Empty;
        string Status = string.Empty;

        public ReportLine(string intel, string status = "")
        {
            IntelLine = intel;
            Status = status;
        }
        public string ToJson()
        {
            StringBuilder sb = new StringBuilder("{");
            sb.Append("\"version\":");
            sb.Append(string.Format("\"{0}\",", Version));
            sb.Append("\"token\":");
            sb.Append(string.Format("\"{0}\",", Configuration.AuthToken));
            sb.Append("\"text\":");
            sb.Append(string.Format("\"{0}\"", IntelLine));
            if (Status != string.Empty)
            {
                sb.Append(",");
                sb.Append("\"status\":");
                sb.Append(string.Format("\"{0}\"", Status));
            }
            sb.Append("}");
            return sb.ToString();
        }

        public static string Version
        {
            get
            {
                return "1.1.0.7";
            }
        }
    }
}
