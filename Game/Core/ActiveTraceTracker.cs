using Core.Events;
using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core
{
    class ActiveTraceTracker
    {
        public double TraceTimeInSecondes = 30d;
        public Endpoint CurrentEndpoint;
        private int CurrentEndpointIndex = 0;
        
        public void StartTrace()
        {
            this.CurrentEndpointIndex = Global.Bounce.BounceList.Count - 1;
            this.CurrentEndpoint = Global.Bounce.BounceList[CurrentEndpointIndex];
            Global.EventTicker.RegisterEvent(
                new Event("ACTIVE TRACE", TraceTimeInSecondes * CurrentEndpoint.ActiveTraceDificulty, this.TraceToNext)
                );
        }

        public void TraceToNext()
        {
            CurrentEndpointIndex -= 1;
            if(CurrentEndpointIndex == 0)
            {
                //we are at the origin so do something
                return;
            }
            if(Global.Bounce.BounceList.Count == 0)
            {
                //The bounce list became of length 0, most likely we disconnected and cleared the list.
                return;
            }
            if(Global.Bounce.BounceList[Global.Bounce.BounceList.Count - 1].ConnectedFrom == null)
            {
                //Connection was lost since last trace.
                return;
            }
            this.CurrentEndpoint = Global.Bounce.BounceList[CurrentEndpointIndex];
            if (CurrentEndpoint.IsLocalEndpoint)
            {
                //this should have been handled before but now we got here somehow.
                return;
            }
            Global.EventTicker.RegisterEvent(
            new Event("ACTIVE TRACE", TraceTimeInSecondes * CurrentEndpoint.ActiveTraceDificulty, this.TraceToNext)
            );
        }
    }
}
