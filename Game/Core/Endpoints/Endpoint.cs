using Game.Model;

namespace Game.Core.Endpoints
{
    public partial class Endpoint
    {
        //Naming and location
        public int x = 0;

        public int y = 0;

        private string IconType = "";

        public Person Owner;
        public EndpointMonitor Monitor = EndpointMonitor.NONE;
        public EndpointFirewall Firewall = EndpointFirewall.NONE;
        public EndpointHashing MemoryHashing = EndpointHashing.NONE;
        public EndpointState State = EndpointState.ONLINE;
        public EndpointType EndpointType = EndpointType.PERSONAL;

        public string Name { get; set; } = "*";
        public double ActiveTraceDificulty = 1;
        public int PassiveTraceDificulty = 1;
        public int TraceSpeed = 1;
        public bool isHidden { set; get; } = false;

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
    }
}