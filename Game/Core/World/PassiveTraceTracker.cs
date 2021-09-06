using Game.Core.Endpoints;
using Game.Core.Events;
using System;
using System.Diagnostics;

namespace Game.Core.World
{

    //TODO rework passive trace
    public class PassiveTraceTracker
    {
        public double TraceTimeInSecondes = 300d;
        private bool Stop = false;
        private bool Tracing = false;

        public void StartTrace(Endpoint StartPoint, Endpoint Direction, DateTime TimeStamp)
        {
            if (this.Tracing)
            {
                throw new Exception("Trying to start a trace when one is already underway.");
            }
            EventBuilder
                .BuildEvent("PASSIVE TRACE")
                .EventInterval(TraceTimeInSecondes * Direction.PassiveTraceDificulty)
                .EventAction(TraceToNext, new object[] { StartPoint, Direction, TimeStamp })
                .RegisterWithAction();

            Debug.WriteLine("Starting Passive Trace at: " + Direction.IPAddress);
            this.Tracing = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Arguments">
        /// To contain:
        /// An Endpoint as the first entry.
        /// A DateTime as the seccond entry.
        /// </param>
        private void TraceToNext(object[] Arguments)
        {
            if (this.Stop)
            {
                return;
            }
            //Null checks
            if (Arguments == null)
            {
                return;
            }
            if (Arguments.Length != 3)
            {
                return;
            }
            if (Arguments[0] == null || Arguments[1] == null || Arguments[2] == null)
            {
                return;
            }

            //Arugment unpacking
            Endpoint Origin = Arguments[0] as Endpoint;
            Endpoint Inspected = Arguments[1] as Endpoint;
            DateTime TimeStamp = (DateTime)Arguments[2];
            double TimeDelta = 300d;

            Debug.WriteLine("Passive Trace Inspecting: " + Inspected.IPAddress);

            if (Inspected.Id == Global.LocalEndpoint.Id)
            {
                //TODO: DO SOMETHING
                //We are now at the player machine do something bad to it.
                Debug.WriteLine("Passive Trace Found Origin: " + Inspected.IPAddress);
                this.Stop = true;
            }

            foreach (LogItem item in Inspected.SystemLog)
            {
                //Filter all logs that are not connection routed logs
                if (item.LogType != LogType.CONNECTION_ROUTED)
                {
                    continue;
                }
                //Filter all logs that are not pointing to the previous trace
                if (item.Too.Id != Origin.Id)
                {
                    continue;
                }
                //Filter all logs that fall outside the 5 minutes(ingame time) time window.
                if ((item.TimeStamp - TimeStamp).TotalSeconds > TimeDelta)
                {
                    continue;
                }

                Debug.WriteLine("Passive Trace Inspecting next link: " + item.From.IPAddress);
                EventBuilder
                    .BuildEvent("PASSIVE TRACE")
                    .EventInterval(TraceTimeInSecondes * Inspected.PassiveTraceDificulty)
                    .EventAction(TraceToNext, new object[] { Inspected, item.From, TimeStamp })
                    .RegisterWithAction();
            }
        }
    }
}