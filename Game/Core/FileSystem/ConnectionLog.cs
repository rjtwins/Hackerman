﻿using Game.Core.Endpoints;
using System.Collections.Generic;

namespace Game.Core.FileSystem
{
    public class ConnectionLog : Program
    {
        public ConnectionLog() : base("syslog.log", false)
        {
        }

        public override string ToString()
        {
            string result = "CONNECTION LOG:\n";
            foreach (LogItem item in this.Folder.ParentFileSystem.ParrentEndpoint.ConnectionLog)
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
                        result += "LOGIN ATTEMPT: " + item.From.IPAddress;
                        break;
                    case LogType.CONNECTION_FAILED:
                        result += "FAILED AUTHENTICAION: " + item.From.IPAddress;
                        break;
                    case LogType.CONNECTION_SUCCES:
                        result += "CONNECTION SUCCESFULL: " + item.From.IPAddress + "LOGGED IN AS: " + acceslevel;
                        break;
                    case LogType.CONNECTION_ROUTED:
                        result += "ROUTED: " + item.From.IPAddress + " : " + item.Too.IPAddress;
                        break;
                    default:
                        break;
                }
                result += "\n";
            }
            return result;
        }
    }
}