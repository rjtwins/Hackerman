using Game.Core.Endpoints;
using Game.Core.FileSystem;

namespace Game.Core.Console.RemotePrograms
{
    public class MemoryScraper : Program
    {
        public MemoryScraper() : base("MemoryScraper", true)
        {
            this.IsMalicious = true;
        }

        public override string RunProgram(Endpoint ranOn)
        {
            string result = "LOGINS IN MEMORY:\n";
            foreach ((string, string) login in ranOn.LoginHistory)
            {
                result += "USER: ";
                result += login.Item1 + "\n";
                result += "PWRD: ";
                result += login.Item2 + "\n";
            }
            return result;
        }
    }
}