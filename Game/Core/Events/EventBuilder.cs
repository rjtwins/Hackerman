using System;

namespace Game.Core.Events
{
    public class EventBuilder :
        IEventName,
        IEventInterval,
        IEventStartTime,
        IEventVoid,
        IEventAction,
        IEventFunction
    {
        private string _EventName;
        private double _EventInterval;
        private DateTime _EventStartTime;
        private Action _EventVoid;
        private Action<object[]> _EventAction;
        private Func<object[], object[]> _EventFunc;
        private object[] _Arguments;

        private EventBuilder(string EventName)
        {
            this._EventName = EventName;
        }

        public static IEventName BuildEvent(string EventName)
        {
            return new EventBuilder(EventName);
        }

        public IEventVoid EventVoid(Action VoidToRun)
        {
            this._EventVoid = VoidToRun;
            return this;
        }

        public IEventAction EventAction(Action<object[]> ActionToRun, object[] Arguments)
        {
            this._EventAction = ActionToRun;
            this._Arguments = Arguments;
            return this;
        }

        public IEventFunction EventFunction(Func<object[], object[]> FuncToRun, object[] Arguments)
        {
            this._EventFunc = FuncToRun;
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

        public void RegisterWithFunc()
        {
            new Event(this._EventName, this._EventInterval, _EventFunc, _Arguments);
        }

        public void RegisterWithVoid()
        {
            new Event(this._EventName, this._EventInterval, _EventVoid);
        }
    }

    public interface IEventName
    {
        public IEventInterval EventInterval(double IntervalInSecondes);

        public IEventStartTime EventStartTime(DateTime StartTime);
    }

    public interface IEventInterval
    {
        public IEventVoid EventVoid(Action VoidToRun);

        public IEventAction EventAction(Action<object[]> ActionToRun, object[] Arguments);

        public IEventFunction EventFunction(Func<object[], object[]> FuncToRun, object[] Arguments);
    }

    public interface IEventStartTime
    {
        public IEventVoid EventVoid(Action VoidToRun);

        public IEventAction EventAction(Action<object[]> ActionToRun, object[] Arguments);

        public IEventFunction EventFunction(Func<object[], object[]> FuncToRun, object[] Arguments);
    }

    public interface IEventVoid
    {
        public void RegisterWithVoid();
    }

    public interface IEventAction
    {
        public void RegisterWithAction();
    }

    public interface IEventFunction
    {
        public void RegisterWithFunc();
    }
}