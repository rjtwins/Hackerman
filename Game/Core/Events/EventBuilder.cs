using Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Events
{
    public class EventBuilder : 
        IEventName ,
        IEventInterval ,
        IEventStartTime ,
        IEventVoid ,
        IEventAction,
        IEventFunction
    {

        string _EventName;
        double _EventInterval;
        DateTime _EventStartTime;
        Action _EventVoid;
        Action<object[]> _EventAction;
        Func<object[], object[]> _EventFunc;
        object[] _Arguments;

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

        public Event RegisterWithAction()
        {
            return new Event(this._EventName, this._EventInterval, _EventAction, _Arguments);
        }

        public Event RegisterWithFunc()
        {
            return new Event(this._EventName, this._EventInterval, _EventFunc, _Arguments);
        }

        public Event RegisterWithVoid()
        {
            return new Event(this._EventName, this._EventInterval, _EventVoid);
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
        public Event RegisterWithVoid();
    }
    public interface IEventAction
    {
        public Event RegisterWithAction();
    }
    public interface IEventFunction
    {
        public Event RegisterWithFunc();
    }
}
