using Game.Core.Endpoints;
using Game.Core.Events;
using System;
using System.Threading.Tasks;

namespace Game.Core.World
{
    public class ActiveTraceTracker
    {
        public double TraceTimeInSecondes = 5;
        public double TotalTraceTime = 5;
        public double TraceTimeLeft = 5;
        public double TraceBeepFrequency = int.MaxValue;

        private UTILS.FrequencyBooper FrequencyBooper;

        public Endpoint CurrentEndpoint;
        private int CurrentEndpointIndex = 0;
        private int Speed = 1;
        private bool Stop = false;
        private bool Tracing = false;

        public ActiveTraceTracker()
        {
            Global.RemoteConsole.CommandParser.OnDisconnected += CommandParser_OnDisconnected;
        }

        private void CommandParser_OnDisconnected(object sender, Console.RemoteConsoleDisconnectedEventArgs e)
        {
            if (this.Tracing)
                StopTrace();
        }

        public void StartTrace(int speed)
        {
            this.Speed = speed;
            this.Stop = false;
            if (this.Tracing)
            {
                throw new Exception("Trying to start a trace when one is allreayd underway.");
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
                .EventVoid(this.TraceToNext)
                .RegisterWithVoid();

            FrequencyBooper = new UTILS.FrequencyBooper(1);
            StartTraceTimer();
        }

        private void StartTraceTimer()
        {
            FrequencyBooper.Start();
            Task.Factory.StartNew(() => 
            {
                while(TotalTraceTime > 0)
                {
                    TraceTimeLeft -= 1;
                    TraceBeepFrequency = Math.Max((30 / TraceTimeLeft), 0.05);
                    FrequencyBooper.Frequency = TraceBeepFrequency;
                    Global.EventTicker.SleepSeconds(1);
                }
                FrequencyBooper.Stop();

            });
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
                EventBuilder.BuildEvent("ActiveTraceCaught")
                    .EventInterval(60d)
                    .EventAction(Consequences.Instance.ActiveTraceCaught, new object[] { })
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
            Global.EndPointMap.PingEndpoint(CurrentEndpoint);
            if (CurrentEndpoint.IsLocalEndpoint)
            {
                FrequencyBooper.Stop();
                //this should have been handled before but now we got here somehow.
                return;
            }
            EventBuilder
                .BuildEvent("ACTIVE TRACE")
                .EventInterval(TraceTimeInSecondes * CurrentEndpoint.ActiveTraceDificulty)
                .EventVoid(this.TraceToNext)
                .RegisterWithVoid();
        }
    }
}