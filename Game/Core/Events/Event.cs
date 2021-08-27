using Core;
using System;

namespace Game.Core.Events
{
    public class Event
    {
        public Guid Id { get; protected set; } //Event ID
        public string Name { get; protected set; } //Event Name
        public double Interval { get; protected set; } //Time until firing of event in time units
        public DateTime StartTime { get; protected set; } //Time when to fire event in time units

        public bool Canceled = false;

        private Func<object[], object[]> FunctionToRun;
        private Action<object[]> ActionWithParameter;
        private Action ActionToRun;
        private object[] MethodArguments;

        private enum MethodType
        { VOID, ACTION, FUNC, NOTHING };

        private MethodType MType = MethodType.VOID;

        public Event(string v)
        {
            MType = MethodType.NOTHING;
            this.Id = Guid.NewGuid();
        }

        public Event(string name, double startInSecondes, Func<object[], object[]> methodToRun, object[] methodArguments)
        {
            this.Id = Guid.NewGuid();
            this.MType = MethodType.FUNC;
            this.Name = name;
            SetStartInterval(startInSecondes);
            this.FunctionToRun = methodToRun;
            this.MethodArguments = methodArguments;
            Register();
        }

        public Event(string name, double startInSecondes, Action<object[]> methodToRun, object[] methodArguments)
        {
            this.Id = Guid.NewGuid();
            this.MType = MethodType.ACTION;
            this.Name = name;
            SetStartInterval(startInSecondes);
            this.ActionWithParameter = methodToRun;
            this.MethodArguments = methodArguments;
            Register();
        }

        public Event(string name, double startInSecondes, Action methodToRun)
        {
            this.Id = Guid.NewGuid();
            this.MType = MethodType.VOID;
            this.Name = name;
            SetStartInterval(startInSecondes);
            this.ActionToRun = methodToRun;
            Register();
        }

        private void Register()
        {
            Global.EventTicker.RegisterEvent(this);
        }

        public virtual object[] StartEvent()
        {
            if (this.Canceled)
            {
                return new object[0];
            }
            LogEvent();
            switch (this.MType)
            {
                case MethodType.NOTHING:
                    return new object[0];

                case MethodType.VOID:
                    ActionToRun();
                    return new object[0];

                case MethodType.ACTION:
                    ActionWithParameter(MethodArguments);
                    return new object[0];

                case MethodType.FUNC:
                    return FunctionToRun(MethodArguments);

                default:
                    return new object[0];
            }
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