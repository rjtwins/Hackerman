using System.Collections.Generic;

namespace Game.Core.FileSystem
{
    public class SystemLog : Game.Core.FileSystem.Program
    {
        private List<string> systemLog = new List<string>();

        public SystemLog() : base("syslog.log", false)
        {

        }

        public List<string> GetSystemLog()
        {
            return systemLog;
        }

        private void SetSystemLog(List<string> value)
        {
            systemLog = value;
        }
    }
}