using Game.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Endpoints
{
    public class EndpointEvents
    {
        public Endpoint AttachedEndpoint = null;
        public int BaseRestartInterval = 0;
        public int BaseAdminCheckInterval = 0;

        //Base values are 2 days and 7 days
        public EndpointEvents(Endpoint attachedEndpoint, int restartInterval = 172800, int adminCheckInterval = 604800)
        {
            if (attachedEndpoint.IsLocalEndpoint)
            {
                return;
            }

            this.AttachedEndpoint = attachedEndpoint;
            this.BaseAdminCheckInterval = adminCheckInterval;
            this.BaseRestartInterval = restartInterval;

            EventBuilder.BuildEvent("EndpointRestart")
                .EventInterval(Global.Rand.Next(Math.Max(this.BaseRestartInterval - 7200, 3600), BaseRestartInterval + 7200))
                .EventVoid(AttachedEndpoint.AutoRestart)
                .RegisterWithVoid();

            EventBuilder.BuildEvent("EndpointAdminCheck")
                .EventInterval(Global.Rand.Next(Math.Max(this.BaseAdminCheckInterval - 7200, 3600), BaseAdminCheckInterval + 7200))
                .EventVoid(AttachedEndpoint.AdminSystemCheck)
                .RegisterWithVoid();
        }

        public void ScheduleNextRestart(int restartInterval = 172800)
        {
            EventBuilder.BuildEvent("EndpointRestart")
                .EventInterval(Global.Rand.Next(Math.Max(restartInterval - 7200, 3600), restartInterval + 7200))
                .EventVoid(AttachedEndpoint.AutoRestart)
                .RegisterWithVoid();
            this.AttachedEndpoint.NextRestartDate = Global.GameTime.AddSeconds(restartInterval);
        }

        public void ScheduleNextAdminCheck(int adminCheckInterval = 604800)
        {

            EventBuilder.BuildEvent("EndpointAdminCheck")
                .EventInterval(Global.Rand.Next(Math.Max(adminCheckInterval - 7200, 3600), adminCheckInterval + 7200))
                .EventVoid(AttachedEndpoint.AdminSystemCheck)
                .RegisterWithVoid();

            this.AttachedEndpoint.NextAdminCheckDate = Global.GameTime.AddSeconds(adminCheckInterval);

        }
    }
}
