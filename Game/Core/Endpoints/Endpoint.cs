using System;
using static Game.UTILS;

namespace Game.Core.Endpoints
{
    public partial class Endpoint
    {
        //Naming and location
        public int x = 0;
        public int y = 0;

        private string IconType = "";

        public Owner Owner;
        public EndpointMonitor Monitor = Endpoint.EndpointMonitor.NONE;
        public EndpointFirewall Firewall = Endpoint.EndpointFirewall.NONE;

        public EndpointState State = EndpointState.ONLINE;
        public EndpointType EndpointType;
        public string name = "*";
        public double ActiveTraceDificulty = 1;
        public int PassiveTraceDificulty = 1;
        public int TraceSpeed = 1;
        public bool isHidden { protected set; get; } = false;
        public bool SoftConnection { get; internal set; }

        public Endpoint(Owner owner, EndpointType endpointType)
        {
            this.EndpointType = endpointType;
            this.Owner = owner;
            this.SetupEndpoint();

            //For testing:
            Person p1 = new Person();
            p1.Name = "root";

            Person p2 = new Person();
            p2.Name = "guest";

            Person p3 = new Person();
            p3.Name = "admin";

            this.AddUser(p1, "root", AccessLevel.ROOT);
            this.AddUser(p2, "guest", AccessLevel.USER);
            this.AddUser(p3, "admin", AccessLevel.ADMIN);
        }
    }
}