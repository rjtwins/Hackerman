using Game.Core.Console.LocalPrograms;
using Game.Core.Endpoints;
using Game.Model;
using System;
using System.Collections.Generic;
using static Game.Core.Endpoints.Endpoint;

namespace Game.Core.Console
{
    public class LocalSystem
    {
        //In KiloBytes
        public long DiskMemory = 1048576;

        public long UsedDiskMemory = 0;
        public string MemoryDiskName = "A-SOFT K1 HDD";

        //In MHz
        public int StartingProcessorSpeed = 120;

        public int ProcessorSpeed = 1000;
        public string ProcessorName = "(53XXUQ0F) TORNADO-NNX 120VK";
        public string ProcessorNameShort = "TORNADO-NNX";

        //in Kbit/s
        public int ModumSpeed = 20;

        public string ModumName = "LINE BLAZER 99";

        //Software levels
        public SoftwareLevel TraceTracker { get; set; } = SoftwareLevel.LVL4;
        public SoftwareLevel DictionaryHacker { get; set; } = SoftwareLevel.LVL0;
        public SoftwareLevel HashLookup { get; set; } = SoftwareLevel.LVL0;
        public SoftwareLevel FirewallBypass { get; set; } = SoftwareLevel.LVL0;
        public SoftwareLevel FirewallDisable { get; set; } = SoftwareLevel.LVL0;
        public SoftwareLevel MonitorBypass { get; set; } = SoftwareLevel.LVL0;
        public SoftwareLevel MemoryScraper { get; set; } = SoftwareLevel.LVL0;
        public SoftwareLevel TrafficListner { get; set; } = SoftwareLevel.LVL0;
        public SoftwareLevel BounceRouter { get; set; } = SoftwareLevel.LVL0;
        public SoftwareLevel BotnetManager { get; set; } = SoftwareLevel.LVL0;
        public SoftwareLevel LogInterface { get; set; } = SoftwareLevel.LVL0;

        public Bounce Bouncer = new Bounce();
        private Dictionary<string, Endpoint[]> SavedBounceLists = new Dictionary<string, Endpoint[]>();
        private HashSet<Traffic> TrafficListnerUsernamePasswordList = new();
        private HashSet<Traffic> MemoryScraperUsernamePasswordList = new();

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
}