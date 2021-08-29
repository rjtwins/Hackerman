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

        public Person Owner;
        public EndpointMonitor Monitor = Endpoint.EndpointMonitor.NONE;
        public EndpointFirewall Firewall = Endpoint.EndpointFirewall.NONE;

        public EndpointState State = EndpointState.ONLINE;
        public EndpointType EndpointType;
        public string name = "*";
        public double ActiveTraceDificulty = 1;
        public int PassiveTraceDificulty = 1;
        public int TraceSpeed = 1;
        public bool isHidden { protected set; get; } = false;
        public DateTime NextAdminCheckDate { get; internal set; }
        public DateTime NextRestartDate { get; internal set; }

        public Endpoint(Person Person, EndpointType endpointType)
        {
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


        internal string PrintSchedule()
        {
            string result = "SCHEDULE:\n"
                + this.NextRestartDate.ToString() + "Scheduled automatic restart.\n"
                + this.NextAdminCheckDate.ToString() + "Scheduled administrative maintenance.\n";
            return result;
        }
    }
}