using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class Logger
    {
        public static List<string> OutputLogs { private set; get; } = new List<string>();

        public static void GenericLog(string s)
        {
            Logger.OutputLogs.Add(s);
            Debug.WriteLine(s);
        }

        public static string PrintLogs()
        {
            return string.Join("\n", OutputLogs);
        }

        public static void ErrorLog(string type, string message)
        {
            GenericLog("ERROR\n--" + type + "\n--Message: " + message);
        }

        public static void EventLog(string id, string name)
        {
            GenericLog("EVENT\n--ID: " + id + "\n--Name: " + name);
        }
    }
}
