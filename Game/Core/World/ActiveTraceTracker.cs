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
            this.TotalTraceTime = 0;
            this.TraceTimeLeft = 0;

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
            
            if(((int)Global.LocalSystem.TraceTracker) > 0)
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
            if(((int)Global.LocalSystem.TraceTracker) < 4)
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
            if(((int)Global.LocalSystem.TraceTracker) > 2)
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

            this.TraceTimeLeft -= TraceTimeInSecondes * CurrentEndpoint.ActiveTraceDificulty;
            UpdateTimerUI();


            if (((int)Global.LocalSystem.TraceTracker) > 1)
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
                .EventVoid(this.TraceToNext)
                .RegisterWithVoid();
        }
    }
}