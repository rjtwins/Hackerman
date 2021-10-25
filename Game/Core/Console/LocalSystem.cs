using Game.Core.Console.LocalPrograms;
using Game.Core.Endpoints;
using Game.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static Game.Core.Endpoints.Endpoint;

namespace Game.Core.Console
{
    [JsonObject(MemberSerialization.OptOut)]
    public class LocalSystem
    {
        [JsonIgnore]
        private static LocalSystem instance;
        [JsonIgnore]
        public static LocalSystem Intance
        {
            get
            {
                if(instance == null)
                {
                    instance = new();
                }
                return instance;
            }
            private set
            {

            }
        }

        private LocalSystem()
        {

        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext streamingContext)
        {
            instance = this;
        }

        //In KiloBytes
        public long DiskMemory = 1048576;

        public long UsedDiskMemory = 0;
        public string MemoryDiskName = "Millennium K1 HDD";

        //In MHz
        public int StartingProcessorSpeed = 120;

        public int ProcessorSpeed = 120;
        public string ProcessorName = "(53XXUQ0F) TORNADO-NNX 120VK";
        public string ProcessorNameShort = "TORNADO-NNX 120VK";

        public string RAMName = "QuickStore";
        public int RAMCapacity = 64;

        //in Kbit/s
        public int ModumSpeed = 22*8;
        public string ModumName = "Comstar M99";

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

        [JsonIgnore]
        public Bounce Bouncer = new Bounce();

        [JsonProperty]
        private Dictionary<string, Guid[]> SavedBounceLists { get; set; } = new Dictionary<string, Guid[]>();

        [JsonProperty]
        private HashSet<Traffic> MemoryScraperUsernamePasswordList { get; set; } = new();

        public bool FirewallBypassActive { get; private set; } = false;
        public bool MonitorBypassActive { get; private set; } = false;

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
        }

        public void MemoryScraperAddEntry(Endpoint from, Endpoint too, string username, string password, EndpointHashing hashed = EndpointHashing.NONE, string hash = null)
        {
            Traffic traffic = new Traffic()
            {
                From = from,
                Too = too,
                Username = username,
                Password = password,
                Hashed = hashed,
                LoginHash = hash,
                TimeStamp = Global.GameTime.ToString()
            };
            Global.LocalEndpoint.AddMemoryScraperTraffic(traffic);
        }

        internal void SaveCurrentBounceListsAs(string commandBody)
        {
            Endpoint[] endpointBounceList = Global.Bounce.BounceList.ToArray();
            Guid[] guidBounceList = new Guid[endpointBounceList.Length];
            for (int i = 0; i < guidBounceList.Length; i++)
            {
                guidBounceList[i] = endpointBounceList[i].Id;
            }
            this.SavedBounceLists[commandBody] = guidBounceList;
        }

        internal Endpoint[] LoadBounceList(string commandBody)
        {
            if (SavedBounceLists.TryGetValue(commandBody, out Guid[] bounceList))
            {
                Endpoint[] endpointBounceList = new Endpoint[bounceList.Length];
                for (int i = 0; i < endpointBounceList.Length; i++)
                {
                    endpointBounceList[i] = Global.AllEndpointsDict[bounceList[i]];
                }
                return endpointBounceList;
            }
            throw new Exception(commandBody + " was not found.");
        }

        internal Dictionary<string, Endpoint[]> GetSavedBouncelists()
        {
            Dictionary<string, Endpoint[]> BounceListDictionary = new();
            foreach(KeyValuePair<string, Guid[]> pair in this.SavedBounceLists)
            {
                Endpoint[] endpointArray = new Endpoint[pair.Value.Length];
                for (int i = 0; i < endpointArray.Length; i++)
                {
                    endpointArray[i] = Global.AllEndpointsDict[pair.Value[i]];
                }
                BounceListDictionary[pair.Key] = endpointArray;
            }
            return BounceListDictionary;
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