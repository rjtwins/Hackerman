using Game.Core.Console;
using Game.Core.Endpoints;
using Game.Core.Events;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Game.Core.World
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ActiveTraceTracker
    { 
        private static ActiveTraceTracker instance;
        public static ActiveTraceTracker Instance
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

        public double TotalTraceTime { get => totalTraceTime; set => totalTraceTime = value; }
        public double TraceTimeLeft { get => traceTimeLeft; set => traceTimeLeft = value; }
        public double TraceBeepFrequency { get => traceBeepFrequency; set => traceBeepFrequency = value; }
        public Endpoint CurrentEndpoint { get => Global.AllEndpointsDict[currentEndpoint]; set => currentEndpoint = value.Id; }
        public int CurrentEndpointIndex { get => currentEndpointIndex; set => currentEndpointIndex = value; }
        public int Speed { get => speed; set => speed = value; }
        public bool Stop { get => stop; set => stop = value; }
        public bool Tracing { get => tracing; set => tracing = value; }

        public static readonly double TraceTimeInSecondes = 5;
        private UTILS.FrequencyBooper FrequencyBooper;

        [JsonProperty]
        private double totalTraceTime = 5;
        [JsonProperty]
        private double traceTimeLeft = 5;
        [JsonProperty]
        private double traceBeepFrequency = int.MaxValue;
        [JsonProperty]
        private Guid currentEndpoint;
        [JsonProperty]
        private int currentEndpointIndex = 0;
        [JsonProperty]
        private int speed = 1;
        [JsonProperty]
        private bool stop = true;
        [JsonProperty]
        private bool tracing = false;

        [JsonConstructor]
        public ActiveTraceTracker()
        {
            Global.RemoteConsole.CommandParser.OnDisconnected += CommandParser_OnDisconnected;
        }

        [OnDeserialized]
        public void Ondeserialized(StreamingContext streamingContext)
        {
            ActiveTraceTracker.instance = this;
        }

        private void CommandParser_OnDisconnected(object sender, Console.RemoteConsoleDisconnectedEventArgs e)
        {
            if (this.Tracing)
                StopTrace();
        }

        public void StartTrace(int speed)
        {
            this.Stop = false;
            this.TotalTraceTime = 0;
            this.TraceTimeLeft = 0;
            this.Speed = speed;
            
            if (this.Tracing)
            {
                return;
            }
            this.Tracing = true;
            this.CurrentEndpointIndex = Global.Bounce.BounceList.Count - 1;
            this.CurrentEndpoint = Global.Bounce.BounceList[CurrentEndpointIndex];

            for (int i = 0; i < Global.Bounce.BounceList.Count-1; i++)
            {
                Endpoint e = Global.Bounce.BounceList[i];
                TotalTraceTime += TraceTimeInSecondes * e.ActiveTraceDificulty;
                TraceTimeLeft = TotalTraceTime;
            }

            EventBuilder
                .BuildEvent("ACTIVE TRACE")
                .EventInterval(TraceTimeInSecondes * CurrentEndpoint.ActiveTraceDificulty)
                .EventAction(ActiveTraceTracker.StaticTraceToNext, new object[] { })
                .RegisterWithAction();

            FrequencyBooper = new UTILS.FrequencyBooper(1);
            
            if(((int)LocalSystem.Intance.TraceTracker) > 0)
            {
                StartTraceTimer();
            }
        }

        private void StartTraceTimer()
        {
            FrequencyBooper.Start();
            Task.Factory.StartNew(() =>
            {
                while (TotalTraceTime > 0 && !this.Stop && this.Tracing)
                {
                    Beat();
                }
                FrequencyBooper.Stop();
            });
        }

        private void Beat()
        {
            TraceTimeLeft -= 1;
            CheckEmergencyDisconnect();
            SetBooperFreqency();
            Global.EventTicker.SleepSeconds(1);
        }

        private void CheckEmergencyDisconnect()
        {
            if(((int)LocalSystem.Intance.TraceTracker) < 4)
            {
                return;
            }
            if ((TraceTimeLeft - 1) <= 0 ||
                CurrentEndpointIndex == 1)
            {
                Global.App.Dispatcher.Invoke(() => { Global.RemoteConsole.CommandParser.ExitDisconnect(); });
                Global.App.Dispatcher.Invoke(() => { Global.RemoteConsole.AddOutput("Trace Tracker emergency disconnect."); });
            }
        }

        private void UpdateTimerUI()
        {
            if(((int)LocalSystem.Intance.TraceTracker) > 2)
            {
                Global.EndPointMap.UpdateTraceTimer(Convert.ToInt32(this.TraceTimeLeft));
            }
        }

        private void SetBooperFreqency()
        {
            TraceBeepFrequency = Math.Min(Math.Max((30 / TraceTimeLeft), 0.25), 40);
            FrequencyBooper.Frequency = TraceBeepFrequency;
        }

        public void StopTrace()
        {
            this.Stop = true;
            this.Tracing = false;
            this.CurrentEndpoint = null;
            this.CurrentEndpointIndex = 0;
        }

        public static void StaticTraceToNext(params Object[] args)
        {
            ActiveTraceTracker.instance.TraceToNext();
        }

        public void TraceToNext()
        {
            if (this.Stop)
            {
                FrequencyBooper.Stop();
                this.Stop = false;
                return;
            }
            CurrentEndpointIndex -= 1;
            if (CurrentEndpointIndex == -1)
            {
                //TODO Do something
                //we are at the origin so do something
                Global.App.Dispatcher.Invoke(() => { Global.RemoteConsole.CommandParser.ExitDisconnect(); });
                Global.App.Dispatcher.Invoke(() => { Global.RemoteConsole.AddOutput("Connection terminated by remote."); });
                EventBuilder.BuildEvent("ActiveTraceCaught")
                    .EventInterval(60d)
                    .EventAction(Consequences.StaticActiveTraceCaught, new object[] { })
                    .RegisterWithAction();
                FrequencyBooper.Stop();
                return;
            }
            if (Global.Bounce.BounceList.Count == 0)
            {
                FrequencyBooper.Stop();
                //The bounce list became of length 0, most likely we disconnected and cleared the list.
                return;
            }
            this.CurrentEndpoint = Global.Bounce.BounceList[CurrentEndpointIndex];

            this.TraceTimeLeft -= TraceTimeInSecondes * CurrentEndpoint.ActiveTraceDificulty;
            UpdateTimerUI();


            if (((int)LocalSystem.Intance.TraceTracker) > 1)
            {
                Global.EndPointMap.PingEndpoint(CurrentEndpoint);
            }

            if (CurrentEndpoint.IsLocalEndpoint)
            {
                FrequencyBooper.Stop();
                //this should have been handled before but now we got here somehow.
                return;
            }
            EventBuilder
                .BuildEvent("ACTIVE TRACE")
                .EventInterval(TraceTimeInSecondes * CurrentEndpoint.ActiveTraceDificulty)
                .EventAction(ActiveTraceTracker.StaticTraceToNext, new object[] { })
                .RegisterWithAction();
        }
    }
}