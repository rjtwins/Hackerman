namespace Game.Core.Endpoints
{
    public class LogItem
    {
        public Endpoint From;
        public Endpoint Too;
        public LogType LogType;
        public AccessLevel AccessLevel;
        public string userName;
    }
}