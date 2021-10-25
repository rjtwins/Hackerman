using Game.Core.Events;
using Newtonsoft.Json;
using System;

namespace Game.Core.Endpoints
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EndpointEvents
    {
        #region backing fields
        [JsonProperty]
        private Guid attachedEndpoint;

        [JsonIgnore]
        public Endpoint AttachedEndpoint
        {
            get
            {
                return Global.AllEndpointsDict[attachedEndpoint];
            }
            set
            {
                attachedEndpoint = value.Id;
            }
        }

        [JsonProperty]
        public int BaseRestartInterval = 0;
        [JsonProperty]
        public int BaseAdminCheckInterval = 0;
        #endregion

        [JsonConstructor]
        public EndpointEvents()
        {

        }

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
                .EventInterval(Global.Rand.Next(Math.Max(this.BaseRestartInterval - 43200, 3600), BaseRestartInterval + 43200))
                .EventAction(EndpointInteraction.AutoRestartEndpoint, new object[] { attachedEndpoint.Id })
                .RegisterWithAction();

            EventBuilder.BuildEvent("EndpointAdminCheck")
                .EventInterval(Global.Rand.Next(Math.Max(this.BaseAdminCheckInterval - 43200, 3600), BaseAdminCheckInterval + 43200))
                .EventAction(EndpointInteraction.AdminCheckEndpoint, new object[] { attachedEndpoint.Id })
                .RegisterWithAction();
        }

        public void ScheduleNextRestart(int restartInterval = 172800)
        {
            EventBuilder.BuildEvent("EndpointRestart")
                .EventInterval(Global.Rand.Next(Math.Max(restartInterval - 7200, 3600), restartInterval + 7200))
                .EventAction(EndpointInteraction.AutoRestartEndpoint, new object[] { AttachedEndpoint.Id })
                .RegisterWithAction();
            this.AttachedEndpoint.NextRestartDate = Global.GameTime.AddSeconds(restartInterval);
        }

        public void ScheduleNextAdminCheck(int adminCheckInterval = 604800)
        {
            EventBuilder.BuildEvent("EndpointAdminCheck")
                .EventInterval(Global.Rand.Next(Math.Max(adminCheckInterval - 7200, 3600), adminCheckInterval + 7200))
                .EventAction(EndpointInteraction.AdminCheckEndpoint, new object[] { AttachedEndpoint.Id })
                .RegisterWithAction();
            this.AttachedEndpoint.NextAdminCheckDate = Global.GameTime.AddSeconds(adminCheckInterval);
        }
    }
}