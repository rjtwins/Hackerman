using Game.Core.Console.LocalPrograms;
using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using static Game.Core.Endpoints.Endpoint;

namespace Game.Core.Console
{
    public class LocalSystem
    {
        //TODO make components to fix placeholders for speed exact.

        //In KiloBytes
        public long DiskMemory = 1048576;
        public long UsedDiskMemory = 0;
        public string MemoryDiskName = "A-SOFT K1 HDD";
        //In MHz
        public int ProcessorSpeed = 120;
        public string ProcessorName = "(53XXUQ0F) TORNADO-NNX 120VK";
        public string ProcessorNameShort = "TORNADO-NNX";
        //in Kbit/s
        public int ModumSpeed = 20;
        public string ModumName = "LINE BLAZER 99";

        public Bouncer Bouncer = new Bouncer();
        private Dictionary<string, Endpoint[]> SavedBounceLists = new Dictionary<string, Endpoint[]>();
        private HashSet<Traffic> TrafficListnerUsernamePasswordList = new();
        private HashSet<Traffic> MemoryScraperUsernamePasswordList = new();

        public EndpointFirewall FirewallBypass = EndpointFirewall.NONE;
        public EndpointMonitor MonitorBypass = EndpointMonitor.NONE;
        public bool FirewallBypassActive { get; private set; }
        public bool MonitorBypassActive { get; private set; }

        //private Dictionary<string, Program> SavedFiles = new Dictionary<string, Program>();

        public void TrafficListnerAddEntry(Endpoint from, Endpoint too, string username, string password, int version, EndpointHashing hashed = EndpointHashing.NONE, string hash = null)
        {
            Traffic traffic = new Traffic()
            {
                From = from,
                Too = too,
                Username = username,
                Password = password,
                Hashed = hashed,
                LoginHash = hash,
                TimeStamp = Global.GameTime.ToString(),
                Version = version
            };

            Global.LocalEndpoint.AddListnerTraffic(traffic);
            //this.TrafficListnerUsernamePasswordList.Add(traffic);
        }

        public void MemoryScraperAddEntry(Endpoint from, Endpoint too, string username, string password, EndpointHashing hashed = EndpointHashing.NONE, string hash = null)
        {
            this.MemoryScraperUsernamePasswordList.Add(new Traffic()
            {
                From = from,
                Too = too,
                Username = username,
                Password = password,
                Hashed = hashed,
                LoginHash = hash,
                TimeStamp = Global.GameTime.ToString()
            });
        }

        internal void SaveCurrentBounceListsAs(string commandBody)
        {
            this.SavedBounceLists[commandBody] = Global.Bounce.BounceList.ToArray();
        }

        internal Endpoint[] LoadBounceList(string commandBody)
        {
            if (SavedBounceLists.TryGetValue(commandBody, out Endpoint[] bounceList))
            {
                return bounceList;
            }
            throw new Exception(commandBody + " was not found.");
        }

        internal Dictionary<string, Endpoint[]> GetSavedBouncelists()
        {
            return this.SavedBounceLists;
        }

        internal bool RemoveBounceList(string commandBody)
        {
            if (!this.SavedBounceLists.ContainsKey(commandBody))
            {
                return false;
            }
            SavedBounceLists.Remove(commandBody);
            return true;
        }

        public int GetModumSpeed()
        {
            return ModumSpeed;
        }

        public int GetProcessorSpeed()
        {
            return this.ProcessorSpeed;
        }

        public long GetDiskMemory()
        {
            return this.DiskMemory;
        }

        public long GetDiskFreeMemory()
        {
            return this.DiskMemory - this.UsedDiskMemory;
        }

        public long GetDiskUsedMemory()
        {
            return this.UsedDiskMemory;
        }
    }

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
            result += "ON: IP: " + this.Too.IPAddress + " \tHOST: " + this.Too.name + "\n";
            if (((int)this.Hashed) > ((int)Endpoint.EndpointHashing.NONE) &&!Cracked)
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
            if(this.Hashed == EndpointHashing.NONE)
            {
                return false;
            }

            if(crackingLevel >= ((int)Hashed))
            {
                this.Cracked = true;
                return true;
            }
            return false;
        }
    }
}