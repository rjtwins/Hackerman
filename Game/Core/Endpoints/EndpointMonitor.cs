using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Endpoints
{
    class EndpointMonitor
    {
        private Endpoint ParentEndpoint { get; set; }
        public List<Endpoint> SuspiciousEndpoints { private get; set; } = new();
        private int FailedLogins = 0;

        private bool monitorActive = true;

        public bool MonitorActive
        {
            get { return monitorActive; }
            set { monitorActive = value; }
        }

        public EndpointMonitor(Endpoint parentEndpoint)
        {
            this.ParentEndpoint = parentEndpoint;
            this.ParentEndpoint.OnFailedLogin += ParentEndpoint_OnFailedLogin;
            this.ParentEndpoint.OnLoggedIn += ParentEndpoint_OnLoggedIn;
            this.ParentEndpoint.OnDisconnected += ParentEndpoint_OnDisconnected;
        }

        private void ParentEndpoint_OnDisconnected(object sender, EndpointDisconnectedEventArgs e)
        {
            ResetLoginCount();
        }

        private void ParentEndpoint_OnLoggedIn(object sender, EndpointLoggedInEventArgs e)
        {
            ResetLoginCount();
        }

        private void ParentEndpoint_OnFailedLogin(object sender, EndpointLoginEventArgs e)
        {
            this.FailedLogins += 1;
            if(this.FailedLogins >= 4 && this.ParentEndpoint.MonitorActive)
            {
                StartActiveTrace();
            }
        }

        private void StartActiveTrace()
        {
            Global.ActiveTraceTracker.StartTrace(ParentEndpoint.TraceSpeed);
        }

        private void ResetLoginCount()
        {
            this.FailedLogins = 0;
        }

    }
}
