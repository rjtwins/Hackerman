using Game;
using System;

namespace Core.Events
{
    public class Event
    {
        public Guid Id { get; protected set; } //Event ID
        public string Name { get; protected set; } //Event Name
        public double Interval { get; protected set; } //Time until firing of event in time units
        public DateTime StartTime { get; protected set; } //Time when to fire event in time units

        public bool Canceled = false;

        private Func<object[], object[]> FunctionToRun;
        private Action<object[]> ActionToRunWithReturn;
        private Action ActionToRun;
        object[] MethodArguments;

        public Event(string v)
        {
            this.Id = Guid.NewGuid();
        }

        public Event(string name, double startInSecondes, Func<object[], object[]> methodToRun, object[] methodArguments)
        {
            this.Name = name;
            SetStartInterval(startInSecondes);
            this.FunctionToRun = methodToRun;
        }

        public Event(string name, double startInSecondes, Action<object[]> methodToRun, object[] methodArguments)
        {
            this.Name = name;
            SetStartInterval(startInSecondes);
            this.ActionToRunWithReturn = methodToRun;
        }

        public Event(string name, double startInSecondes, Action methodToRun)
        {
            this.Name = name;
            SetStartInterval(startInSecondes);
            this.ActionToRun = methodToRun;
        }

        //TODO: work on a if tree here!
        public virtual object[] StartEvent()
        {

            if (this.Canceled)
            {
                return new object[0];
            }
            LogEvent();
            return MethodToRun(MethodArguments);
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

        public virtual void SetStartInterval(double IntervalInSecondes)
        {
            this.Interval = IntervalInSecondes;
            this.StartTime = Global.GameTime.AddSeconds(IntervalInSecondes);
        }
    }
}