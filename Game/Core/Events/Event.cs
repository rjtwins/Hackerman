using System;

namespace Core.Events
{
    public abstract class Event
    {
        public Guid Id { get; protected set; } //Event ID
        public string Name { get; protected set; } //Event Name
        public int Interval { get; protected set; } //Time until firing of event in time units
        public int StartTime { get; protected set; } //Time when to fire event in time units

        public Event()
        {
            this.Id = Guid.NewGuid();
        }

        public virtual void StartEvent()
        {
            LogEvent();
        }

        public virtual void LogEvent()
        {
            Logger.EventLog(Id.ToString(), Name);
        }

        public virtual void SetStartTime(int TimeInTicks)
        {
            this.StartTime = TimeInTicks;
            this.Interval = EventTicker.invokeCount - TimeInTicks;
        }

        public virtual void SetStartInterval(int IntervalInTicks)
        {
            this.Interval = IntervalInTicks;
            this.StartTime = EventTicker.invokeCount + IntervalInTicks;
        }
    }
}