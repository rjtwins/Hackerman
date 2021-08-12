using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Endpoints
{
    public class Endpoint : EndpointBackend
    {
        //Naming and location
        public int x = 0;
        public int y = 0;
        private string IconType = "";
        public string name = "*";
        public bool isHidden { protected set; get; } = false;

        public Endpoint() : base()
        {
            //For testing:
            UsernamePasswordDict["rootroot"] = AccessLevel.ROOT;
            UsernamePasswordDict["guestguest"] = AccessLevel.USER;
            UsernamePasswordDict["adminadmin"] = AccessLevel.ADMIN;
        }
    }
}
