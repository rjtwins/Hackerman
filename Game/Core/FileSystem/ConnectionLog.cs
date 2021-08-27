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
                string acceslevel = string.Empty;
                switch (item.AccessLevel)
                {
                    case AccessLevel.USER:
                        acceslevel = "USER";
                        break;

                    case AccessLevel.ADMIN:
                        acceslevel = "ADMIN";
                        break;

                    case AccessLevel.ROOT:
                        acceslevel = "ROOT";
                        break;

                    default:
                        break;
                }

                switch (item.LogType)
                {
                    case LogType.CONNECTION_ATTEMPT:
                        result += "LOGIN ATTEMPT:\t\t\t\t" + item.From.IPAddress;
                        break;

                    case LogType.CONNECTION_FAILED:
                        result += "FAILED AUTHENTICAION:\t\t\t" + item.From.IPAddress;
                        break;

                    case LogType.CONNECTION_SUCCES:
                        result += "CONNECTION SUCCESFULL:\t\t\t" + item.From.IPAddress + " WITH: " + acceslevel;
                        break;

                    case LogType.CONNECTION_ROUTED:
                        result += "ROUTED:\t\t\t\t" + item.From.IPAddress + " : " + item.Too.IPAddress;
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