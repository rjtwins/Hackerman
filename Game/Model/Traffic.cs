using Game.Core.Endpoints;
using System;
using static Game.Core.Endpoints.Endpoint;

namespace Game.Model
{
    public class Traffic
    {
        public Endpoint From;
        public Endpoint Too;
        public string Username;
        public string Password;
        public string LoginHash;
        public EndpointHashing Hashed;
        public string TimeStamp;
        public int Version;
        public bool Cracked = false;

        public override string ToString()
        {
            string result = string.Empty;
            String fromIP = "127.0.0.1";
            if (this.From != null)
            {
                fromIP = this.From.IPAddress;
            }
            string login = this.LoginHash;
            result += "Traffic:\n";
            result += "ON: IP: " + this.Too.IPAddress + " \tHOST: " + this.Too.Name + "\n";
            if (((int)this.Hashed) > ((int)Endpoint.EndpointHashing.NONE) && !Cracked)
            {
                result += "LOGIN HASH: " + login + "\n";
            }
            else
            {
                result += "USER: " + this.Username + "\nPWRD: " + this.Password;
            }
            if (this.Version > 1)
            {
                result += "FROM: IP: " + fromIP + "\n";
            }
            return result;
        }

        public bool Crack(int crackingLevel)
        {
            if (this.Hashed == EndpointHashing.NONE)
            {
                return false;
            }

            if (crackingLevel >= ((int)Hashed))
            {
                this.Cracked = true;
                return true;
            }
            return false;
        }
    }
}