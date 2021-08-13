using Game;
using System;

namespace Core.Events
{
    public abstract class Event
    {
        public Guid Id { get; protected set; } //Event ID
        public string Name { get; protected set; } //Event Name
        public double Interval { get; protected set; } //Time until firing of event in time units
        public DateTime StartTime { get; protected set; } //Time when to fire event in time units

        public bool Canceled = false;

        public Event()
        {
            this.Id = Guid.NewGuid();
        }

        public virtual void StartEvent()
        {
            if (this.Canceled)
            {
                return;
            }
            LogEvent();
        }

        public virtual void LogEvent()
        {
            Logger.EventLog(Id.ToString(), Name);
        }

        public virtual void AddSecondes(double secondes)
        {
            this.StartTime = this.StartTime.AddSeconds(secondes);
        }

        public virtual void CancelEvent()
        {
            this.Canceled = true;
        }
        public virtual void UnCancelEvent()
        {
            this.Canceled = false;
        }

        public virtual void SetStartTime(DateTime dateTime)
        {
            this.StartTime = dateTime;
            this.Interval = (Global.GameTime - dateTime).TotalSeconds;
        }

        public virtual void SetStartInterval(int IntervalInSecondes)
        {
            this.Interval = IntervalInSecondes;
            this.StartTime = Global.GameTime.AddSeconds(IntervalInSecondes);
        }
    }
}