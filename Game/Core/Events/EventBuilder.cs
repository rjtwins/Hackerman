using System;

namespace Game.Core.Events
{
    public class EventBuilder :
        IEventName,
        IEventInterval,
        IEventStartTime,
        IEventAction
    {
        private string _EventName;
        private double _EventInterval;
        private DateTime _EventStartTime;
        private Delegate _EventAction;
        private object[] _Arguments;

        private EventBuilder(string EventName)
        {
            this._EventName = EventName;
        }

        public static IEventName BuildEvent(string EventName)
        {
            return new EventBuilder(EventName);
        }

        public IEventAction EventAction(Action<object[]> ActionToRun, object[] Arguments)
        {
            this._EventAction = ActionToRun;
            this._Arguments = Arguments;
            return this;
        }

        public IEventInterval EventInterval(double IntervalInSecondes)
        {
            this._EventInterval = IntervalInSecondes;
            //this._EventStartTime = Global.GameTime.AddSeconds(IntervalInSecondes);
            return this;
        }

        public IEventStartTime EventStartTime(DateTime StartTime)
        {
            this._EventStartTime = StartTime;
            this._EventInterval = (StartTime - Global.GameTime).TotalSeconds;
            return this;
        }

        public void RegisterWithAction()
        {
            new Event(this._EventName, this._EventInterval, _EventAction, _Arguments);
        }
    }

    public interface IEventName
    {
        public IEventInterval EventInterval(double IntervalInSecondes);

        public IEventStartTime EventStartTime(DateTime StartTime);
    }

    public interface IEventInterval
    {
        public IEventAction EventAction(Action<object[]> ActionToRun, object[] Arguments);

    }

    public interface IEventStartTime
    {
        public IEventAction EventAction(Action<object[]> ActionToRun, object[] Arguments);

    }

    public interface IEventAction
    {
        public void RegisterWithAction();
    }
}