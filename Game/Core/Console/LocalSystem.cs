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
        //In MHz
        public int ProcessorSpeed = 150;
        //in Kbit/s
        public int ModumSpeed = 20;

        public Bouncer Bouncer = new Bouncer();
        private Dictionary<string, Endpoint[]> SavedBounceLists = new Dictionary<string, Endpoint[]>();
        private HashSet<Traffic> TrafficListnerUsernamePasswordList = new();
        private HashSet<Traffic> MemoryScraperUsernamePasswordList = new();

        public EndpointFirewall FirewallBypass = EndpointFirewall.NONE;
        public EndpointMonitor MonitorBypass = EndpointMonitor.NONE;
        public bool FirewallBypassActive { get; private set; }
        public bool MonitorBypassActive { get; private set; }

        //private Dictionary<string, Program> SavedFiles = new Dictionary<string, Program>();

        public void TrafficListnerAddEntry(Endpoint from, Endpoint too, string username, string password, EndpointHashing hashed = EndpointHashing.NONE, string hash = null)
        {
            this.TrafficListnerUsernamePasswordList.Add(new Traffic() 
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
    }
}