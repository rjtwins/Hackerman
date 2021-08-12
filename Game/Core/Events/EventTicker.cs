using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Events
{
    public class EventTicker
    {
        //Main ticker
        private Timer MainTimer;

        public int GameSpeed { private set; get; } = 1;

        //Amount of time the ticker was called.
        public static int invokeCount { private set; get; }

        //Dictionaries for all events past present and future.
        public SortedDictionary<int, List<Guid>> TimeIDListDict = new SortedDictionary<int, List<Guid>>();

        public Dictionary<Guid, Event> IDEventDict = new Dictionary<Guid, Event>();

        public EventTicker()
        {
            // Create an AutoResetEvent to signal the timeout threshold in the
            // timer callback has been reached.
            MainTimer = new Timer(Beat, this, Timeout.Infinite, 250);
        }

        public void ChangeSpeed(int level)
        {
            this.GameSpeed = level;
            int interval = 1000 / (level * 4);
            MainTimer.Change(interval, interval);
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
            MainTimer.Change(Timeout.Infinite, 250);
        }

        public void StartTicker()
        {
            MainTimer.Change(250, 250);
        }

        public void Beat(Object stateInfo)
        {
            //Debug.WriteLine("Tick number: " + invokeCount);
            //Debug.WriteLine("Time since game start: " + (Convert.ToDouble(invokeCount)) / 4 + "s");
            EventTicker.invokeCount += 1;
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