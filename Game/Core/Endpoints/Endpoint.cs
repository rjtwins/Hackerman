using Game.Core.FileSystem;
using System;

namespace Game.Core.Endpoints
{
    public class Endpoint : EndpointBackend
    {
        //Naming and location
        public int x = 0;
        public int y = 0;
        private string IconType = "";

        public EndpointType EndpointType;
        public string name = "*";
        public double ActiveTraceDificulty = 1;
        public int PassiveTraceDificulty = 1;
        public bool isHidden { protected set; get; } = false;

        public Endpoint() : base()
        {
            //For testing:
            this.AddUser("root", "root", AccessLevel.ROOT);
            this.AddUser("guest", "guest", AccessLevel.USER);
            this.AddUser("admin", "admin", AccessLevel.ADMIN);

            for (int i = 0; i < Global.RandomUsersPerEndpoint; i++)
            {
                this.AddUser(UTILS.PickRandomName(), UTILS.PickRandomPassword(), AccessLevel.USER);
            }
        }
    }
}