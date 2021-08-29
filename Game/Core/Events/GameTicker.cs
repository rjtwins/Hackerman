using Game;
using Game.Core.Events;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Events
{
    public class GameTicker
    {
        //Main ticker
        private Timer MainTimer;
        public int GameSpeed { private set; get; } = 1;
        public double TimeIntervalInSecondes = 0.1d;

        //Amount of time the ticker was called.
        public static int invokeCount { private set; get; }

        //Dictionaries for all events past present and future.
        public SortedList<DateTime, Guid> EventQueue = new SortedList<DateTime, Guid>();
        public Dictionary<Guid, Event> IDEventDict = new Dictionary<Guid, Event>();
        private Dictionary<int, double> GameSpeedDict = new Dictionary<int, double>();

        public GameTicker()
        {
            // timer callback has been reached.
            MainTimer = new Timer(Beat, this, Timeout.Infinite, 100);
            Global.GameTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            Global.GameTime = Global.GameTime.AddSeconds(1000000000d);

            GameSpeedDict[0] = 0;
            GameSpeedDict[1] = 0.1;
            GameSpeedDict[2] = 6;
            GameSpeedDict[3] = 60;
            GameSpeedDict[4] = 720;
        }

        public void ChangeSpeed(int level)
        {
            this.GameSpeed = level;
            TimeIntervalInSecondes = GameSpeedDict[level];
            //TimeIntervalInSecondes = (double)(15 * GameSpeed);
        }

        public void RegisterEvent(Event e)
        {
            if (e.StartTime < Global.GameTime)
            {
                throw new Exception("Event cannot be registered as its start time is before the current time.");
            }
            while (this.EventQueue.ContainsKey(e.StartTime))
            {
                e.SetStartTime(e.StartTime.AddSeconds(1));
            }
            this.EventQueue.Add(e.StartTime, e.Id);
            this.IDEventDict[e.Id] = e;
            //Debug.WriteLine("New Event Registered, Start time: " + e.StartTime + "Current Time: " + Global.GameTime);
        }

        public void StopTicker()
        {
            Global.GamePaused = true;
            this.GameSpeed = 0;
            TimeIntervalInSecondes = GameSpeedDict[0];
        }

        public void StartTicker()
        {
            if (TimeIntervalInSecondes == 0d)
            {
                TimeIntervalInSecondes = GameSpeedDict[1];
                this.GameSpeed = 1;
                Global.GamePaused = false;
                return;
            }
            MainTimer.Change(100, 100);
            Global.GamePaused = false;
        }

        public void Beat(Object stateInfo)
        {
            GameTicker.invokeCount += 1;
            Global.GameTime = Global.GameTime.AddSeconds(TimeIntervalInSecondes);
            if (Global.MainWindow != null)
            {
                Global.MainWindow.UpdateDateTime();
                HandleEvents();
            }
        }

        private void HandleEvents()
        {
            List<Event> EventsToHandle = new List<Event>();
            lock (this.EventQueue)
            {
                Event e;
                for (int i = 0; i < EventQueue.Count; i++)
                {
                    Guid EventId = EventQueue[EventQueue.Keys[i]];
                    if (!IDEventDict.TryGetValue(EventId, out e))
                    {
                        throw new Exception("Event: " + e.Name + ":" + e.Id + " is in the event queue but not in event id dict!");
                    }
                    if (e.StartTime > Global.GameTime)
                    {
                        break;
                    }
                    EventsToHandle.Add(e);
                    foreach (Event eventToHandle in EventsToHandle)
                    {
                        EventQueue.Remove(eventToHandle.StartTime);
                    }
                }
            }

            lock (EventsToHandle)
            {
                foreach(Event eventToHandle in EventsToHandle)
                {
                    TryStartEvent(eventToHandle);
                }
            }
        }

        /// <summary>
        /// Returns after x in game secondes
        /// </summary>
        /// <param name="secondes"></param>
        public void SleepSeconds(double secondes)
        {
            double secondsAddedPerSeconds = this.TimeIntervalInSecondes * 10;
            System.Threading.Thread.Sleep(Convert.ToInt32((secondes / secondsAddedPerSeconds) * 1000d));
        }

        private void TryStartEvent(Event eventToHandle)
        {
            eventToHandle.StartEvent();
        }
    }
}