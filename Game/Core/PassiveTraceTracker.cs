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
    class PassiveTraceTracker
    {
        public double TraceTimeInSecondes = 300d;
        public Endpoint StartEndPoint;
        public Endpoint CurrentEndpoint;
        private bool Stop = false;
        private bool Tracing = false;

        public void StartTrace(Endpoint StartPoint, Endpoint Direction)
        {
            if (this.Tracing)
            {
                throw new Exception("Trying to start a trace when one is already underway.");
            }
            this.CurrentEndpoint = Direction;
            EventBuilder
                .BuildEvent("PASSIVE TRACE")
                .EventInterval(TraceTimeInSecondes * CurrentEndpoint.ActiveTraceDificulty)
                .EventVoid(this.TraceToNext)
                .RegisterWithVoid();
            Debug.WriteLine("Starting Passive Trace at: " + this.CurrentEndpoint.IPAddress);
            this.Tracing = true;
        }

        private void TraceToNext()
        {

        }
    }
}
