using Game.Core.Endpoints;
using System.Collections.Generic;

namespace Game.Core.FileSystem
{
    public class ConnectionLog : Program
    {
        public List<LogItem> Log;

        public ConnectionLog() : base("syslog.log", false)
        {
            Log = new List<LogItem>();
        }

        public override string ToString()
        {
            string result = "CONNECTION LOG:\n";
            foreach (LogItem item in this.Log)
            {
                string acceslevel = item.AccessLevel.ToString();
                string ipAddress = "127.0.0.1";

                if (item.From != null)
                {
                    ipAddress = item.From.IPAddress;
                }

                switch (item.LogType)
                {
                    case LogType.CONNECTION_ATTEMPT:
                        result += "LOGIN ATTEMPT:\t\t\t\t" + ipAddress;
                        break;

                    case LogType.CONNECTION_FAILED:
                        result += "FAILED AUTHENTICAION:\t\t\t" + ipAddress;
                        break;

                    case LogType.CONNECTION_SUCCES:
                        result += "CONNECTION SUCCESFULL:\t\t\t" + ipAddress + " WITH: " + acceslevel;
                        break;

                    case LogType.CONNECTION_ROUTED:
                        result += "ROUTED:\t\t\t\t" + ipAddress + " : " + item.Too.IPAddress;
                        break;

                    default:
                        break;
                }
                result += "\t\t" + item.TimeStamp.ToString() + "\n";
            }
            return result;
        }
    }
}