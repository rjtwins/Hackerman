using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Endpoints
{
    //Used to make outside calls to endpoint instances.
    public static class EndpointInteraction
    {
        private static Endpoint GetEndpointByID(Guid id)
        {
            return Global.AllEndpointsDict[id];
        }

        public static void AutoRestartEndpoint(params object[] args)
        {
            Guid id = (Guid)args[0];
            GetEndpointByID(id).AutoRestart();
        }

        public static void AdminCheckEndpoint(params object[] args)
        {
            Guid id = (Guid)args[0];
            GetEndpointByID(id).AdminSystemCheck();
        }
    }
}
