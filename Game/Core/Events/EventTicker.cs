using Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Core.Events
{
    public class EventTicker
    {
        //Main ticker
        private Timer MainTimer;

        public int GameSpeed { private set; get; } = 1;
        public double TimeIntervalInSecondes = 6d;

        //Amount of time the ticker was called.
        public static int invokeCount { private set; get; }

        //Dictionaries for all events past present and future.
        public SortedDictionary<int, List<Guid>> TimeIDListDict = new SortedDictionary<int, List<Guid>>();

        public Dictionary<Guid, Event> IDEventDict = new Dictionary<Guid, Event>();

        private Dictionary<int, double> GameSpeedDict = new Dictionary<int, double>();

        public EventTicker()
        {
            // timer callback has been reached.
            MainTimer = new Timer(Beat, this, Timeout.Infinite, 250);
            Global.GameTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            Global.GameTime = Global.GameTime.AddSeconds(1000000000d);

            GameSpeedDict[0] = 0;
            GameSpeedDict[1] = 6;
            GameSpeedDict[2] = 45;
            GameSpeedDict[3] = 232;
            GameSpeedDict[4] = 632;
            GameSpeedDict[5] = 1832;

        }

        public void IncreaseGameSpeed()
        {
            ChangeSpeed(Math.Min(this.GameSpeed + 1, 5));
        }

        public void DecreaseGameSpeed()
        {
            ChangeSpeed(Math.Max(this.GameSpeed - 1, 1));
        }

        public void ChangeSpeed(int level)
        {
            this.GameSpeed = level;
            TimeIntervalInSecondes = GameSpeedDict[level];
            //TimeIntervalInSecondes = (double)(15 * GameSpeed);
        }

        public void RegisterEvent(Event e)
        {
            if (e.StartTime < EventTicker.invokeCount)
            {
                throw new Exception("Event cannot be registered as its start time is before the current time.");
            }
            if (!TimeIDListDict.ContainsKey(e.StartTime))
            {
                TimeIDListDict[e.StartTime] = new List<Guid>();
            }
            if (TimeIDListDict[e.StartTime] == null)
            {
                TimeIDListDict[e.StartTime] = new List<Guid>();
            }
            this.TimeIDListDict[e.StartTime].Add(e.Id);
            this.IDEventDict[e.Id] = e;
        }

        public void StopTicker()
        {
            Global.GamePaused = true;
            TimeIntervalInSecondes = GameSpeedDict[0];
        }

        public void StartTicker()
        {
            if(TimeIntervalInSecondes == 0d)
            {
                TimeIntervalInSecondes = GameSpeedDict[1];
                this.GameSpeed = 1;
                Global.GamePaused = false;
                return;
            }
            MainTimer.Change(500, 100);
            Global.GamePaused = false;
        }

        public void Beat(Object stateInfo)
        {
            EventTicker.invokeCount += 1;
            Global.GameTime = Global.GameTime.AddSeconds(TimeIntervalInSecondes);
            Global.MainWindow.UpdateDateTime();
            HandleEvents();
        }

        private void HandleEvents()
        {
            if (!TimeIDListDict.ContainsKey(invokeCount))
            {
                //No events to do return;
                return;
            }
            List<Guid> EventIDsToHandle = TimeIDListDict[invokeCount];

            foreach (Guid id in EventIDsToHandle)
            {
                TryStartEvent(id);
            }
        }

        private void TryStartEvent(Guid id)
        {
            Event e = null;
            try
            {
                e = IDEventDict[id];
            }
            catch (KeyNotFoundException ex)
            {
                Logger.ErrorLog("EventKey not found", "KeyName: " + id.ToString());
                return;
            }
            e.StartEvent();
        }
    }
}