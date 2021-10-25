using Game.Model;
using Newtonsoft.Json;
using System;

namespace Game.Core.Endpoints
{
    public partial class Endpoint
    {
        #region backing fields
        [JsonProperty]
        public int x = 0;
        [JsonProperty]
        public int y = 0;
        [JsonProperty]
        private string IconType = "";
        public SoftwareLevel Firewall = SoftwareLevel.LVL0;
        [JsonProperty]
        public EndpointHashing MemoryHashing = EndpointHashing.NONE;
        [JsonProperty]
        public EndpointState State = EndpointState.ONLINE;
        [JsonProperty]
        public EndpointType EndpointType = EndpointType.PERSONAL;
        [JsonProperty]
        private string name = "*";
        [JsonProperty]
        private double activeTraceDificulty = 1;
        [JsonProperty]
        private int passiveTraceDificulty = 1;
        [JsonProperty]
        private int traceSpeed = 1;
        [JsonProperty]
        private bool isHidden = false;
        [JsonProperty]
        public Guid owner;
        #endregion

        public Person Owner
        {
            get
            {
                return Global.AllPersonsDict[owner];
            }
            set
            {
                owner = value.Id;
            }
        }
        public string Name { get => name; set => name = value; }
        public double ActiveTraceDificulty { get => activeTraceDificulty; set => activeTraceDificulty = value; }
        public int PassiveTraceDificulty { get => passiveTraceDificulty; set => passiveTraceDificulty = value; }
        public int TraceSpeed { get => traceSpeed; set => traceSpeed = value; }
        public bool IsHidden { get => isHidden; set => isHidden = value; }

        public Endpoint(Person Person, EndpointType endpointType)
        {
            this.Id = Guid.NewGuid();
            Global.AllEndpointsDict[Id] = this;
            this.EndpointType = endpointType;
            this.Owner = Person;
            this.SetupEndpoint();

            //For testing:
            Person p1 = new Person();
            p1.Name = "root";
            p1.WorkPassword = "root";

            Person p2 = new Person();
            p2.Name = "guest";
            p2.WorkPassword = "guest";

            this.AddUser(p1, "root", AccessLevel.ROOT);
            this.AddUser(p2, "guest", AccessLevel.USER);
        }

        internal LogItem FindConnectedLog(Endpoint from)
        {
            foreach(LogItem item in this.SystemLog)
            {
                if(item.LogType != LogType.CONNECTION_SUCCES)
                {
                    continue;
                }
                if(item.From == from)
                {
                    return item;
                }
            }
            return null;
        }

        internal LogItem FindDisconnectedLog(Endpoint from)
        {
            foreach (LogItem item in this.SystemLog)
            {
                if (item.LogType != LogType.CONNECTION_DISCONNECTED)
                {
                    continue;
                }
                if (item.From == from)
                {
                    return item;
                }
            }
            return null;
        }
    }
}