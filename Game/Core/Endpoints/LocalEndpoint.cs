using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Endpoints
{
    public class LocalEndpoint : Endpoint
    {
        //this is the players endpoint
        public LocalEndpoint() : base()
        {
            IsLocalEndpoint = true;
        }
    }
}
