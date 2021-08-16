using Core.Events;
using Game.Core.Endpoints;
using Game.Core.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core
{
    public class ActiveTraceTracker
    {
        public double TraceTimeInSecondes = 300d;
        public Endpoint CurrentEndpoint;
        private int CurrentEndpointIndex = 0;
        private bool Stop = false;
        private bool Tracing = false;
        
        public void StartTrace()
        {
            if (this.Tracing)
            {
                throw new Exception("Trying to start a trace when one is allreayd underway.");
            }
            this.CurrentEndpointIndex = Global.Bounce.BounceList.Count - 1;
            this.CurrentEndpoint = Global.Bounce.BounceList[CurrentEndpointIndex];
            EventBuilder
                .BuildEvent("ACTIVE TRACE")
                .EventInterval(TraceTimeInSecondes * CurrentEndpoint.ActiveTraceDificulty)
                .EventVoid(this.TraceToNext)
                .RegisterWithVoid();
            Debug.WriteLine("Starting Active Trace at: " + this.CurrentEndpoint.IPAddress);
            this.Tracing = true;
        }

        public void StopTrace()
        {
            this.Stop = true;
            this.Tracing = false;
            this.CurrentEndpoint = null;
            this.CurrentEndpointIndex = 0;
        }

        public void TraceToNext()
        {
            if (this.Stop)
            {
                this.Stop = false;
                Debug.WriteLine("Active Trace: Trace stopped via StopTrace()");
                return;
            }
            CurrentEndpointIndex -= 1;
            if(CurrentEndpointIndex == 0)
            {
                //we are at the origin so do something
                Debug.WriteLine("Active Trace: CurrentEndpointIdex == 0, found local endpoint");
                return;
            }
            if(Global.Bounce.BounceList.Count == 0)
            {
                //The bounce list became of length 0, most likely we disconnected and cleared the list.
                Debug.WriteLine("Active Trace: Bouncelist is empty");

                return;
            }
            if(Global.Bounce.BounceList[Global.Bounce.BounceList.Count - 1].ConnectedFrom == null)
            {
                //Connection was lost since last trace.
                Debug.WriteLine("Active Trace: connection between local end final was lost stopping trace");
                return;
            }
            this.CurrentEndpoint = Global.Bounce.BounceList[CurrentEndpointIndex];
            if (CurrentEndpoint.IsLocalEndpoint)
            {
                //this should have been handled before but now we got here somehow.
                Debug.WriteLine("Active Trace: Current is local endpoint, we should never get here.");
                return;
            }
            Debug.WriteLine("Active Trace: Tracing to next: " + CurrentEndpoint.IPAddress);
            EventBuilder
                .BuildEvent("ACTIVE TRACE")
                .EventInterval(TraceTimeInSecondes * CurrentEndpoint.ActiveTraceDificulty)
                .EventVoid(this.TraceToNext)
                .RegisterWithVoid();
        }
    }
}
