using Game.Core.Endpoints;
using Game.Core.Events;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Game.Core.World
{

    //TODO rework passive trace
    [JsonObject(MemberSerialization.OptIn)]
    public class PassiveTraceTracker
    {

        private static PassiveTraceTracker instance;
        public static PassiveTraceTracker Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new();
                }
                return instance;
            }
            private set
            {

            }
        }
        public bool Stop { get => stop; set => stop = value; }
        public bool Tracing { get => tracing; set => tracing = value; }

        private static readonly double TraceTimeInSecondes = 300d;
        private static readonly double FastTraceTimeInsecondes = 10d;

        [JsonProperty]
        private bool stop = false;
        [JsonProperty]
        private bool tracing = false;

        public void StartTrace(Endpoint StartPoint, Endpoint Direction, DateTime TimeStamp, bool FromDisconnedted)
        {
            if (this.Tracing)
            {
                throw new Exception("Trying to start a trace when one is already underway.");
            }
            EventBuilder
                .BuildEvent("PASSIVE TRACE")
                .EventInterval(TraceTimeInSecondes * Direction.PassiveTraceDificulty)
                .EventAction(PassiveTraceTracker.StaticTraceToNext, new object[] { StartPoint, Direction, TimeStamp , FromDisconnedted})
                .RegisterWithAction();

            Debug.WriteLine("Starting Passive Trace at: " + Direction.IPAddress);
            this.Tracing = true;
        }

        public static void StaticTraceToNext(object[] args)
        {
            PassiveTraceTracker.instance.TraceToNext(args);
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
            if (Arguments.Length != 4)
            {
                return;
            }
            if (Arguments[0] == null || Arguments[1] == null || Arguments[2] == null || Arguments[3] == null)
            {
                return;
            }

            //Arugment unpacking
            Endpoint Origin = Arguments[0] as Endpoint;
            Endpoint Inspected = Arguments[1] as Endpoint;
            DateTime TimeStamp = (DateTime)Arguments[2];
            bool FromDisconnected = (bool)Arguments[3];
            double TimeDelta = 300d;

            if (Inspected.Id == Global.LocalEndpoint.Id)
            {
                //TODO: DO SOMETHING
                //We are now at the player machine do something bad to it.
                Debug.WriteLine("Passive Trace Found Origin: " + Inspected.IPAddress);
                this.Stop = true;
                return;
            }

            Debug.WriteLine("Passive Trace Inspecting: " + Inspected.IPAddress);

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
                //If we are comming form a disconnected and therefore don't know the time stamp of routing we should just look trough all routing logs.
                if ((item.TimeStamp - TimeStamp).TotalSeconds > TimeDelta && !FromDisconnected)
                {
                    continue;
                }
                //Don't look trough all routing logs just the one since the disconnected time stamp we have.
                if (FromDisconnected)
                {
                    if (item.TimeStamp < TimeStamp)
                    {
                        MakeTraceEvent(Inspected, item.From, TimeStamp);
                        return;
                    }
                }
                MakeTraceEvent(Inspected, item.From, TimeStamp);
            }
        }

        private void MakeTraceEvent(Endpoint Origin, Endpoint Next, DateTime TimeStamp)
        {
            EventBuilder
                .BuildEvent("PASSIVE TRACE")
                .EventInterval(TraceTimeInSecondes * Next.PassiveTraceDificulty)
                .EventAction(PassiveTraceTracker.StaticTraceToNext, new object[] { Origin, Next, TimeStamp, false })
                .RegisterWithAction();
        }
    }
}