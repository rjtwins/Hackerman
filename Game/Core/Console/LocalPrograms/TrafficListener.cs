using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Console.LocalPrograms
{
    class TrafficListener
    {
        private static readonly TrafficListener instance = new TrafficListener();

        public static TrafficListener Instance
        {
            get
            {
                return instance;
            }
            private set
            {

            }
        }

        private Endpoint AttachedToo;

        public string StartupFromCommand()
        {
            if(Global.RemoteSystem == null)
            {
                return "";
            }
        }

        public void AttachTooEndpoint(Endpoint target)
        {
            this.AttachedToo = target;
            target.OnLogin += Target_OnLogin;
        }

        private void Target_OnLogin(object sender, EndpointLoginEventArgs e)
        {
            //Show user traffic was intercepted
        }

        public void Stop()
        {
            AttachedToo.OnLogin -= Target_OnLogin;
            this.AttachedToo = null;
        }
    }
}
