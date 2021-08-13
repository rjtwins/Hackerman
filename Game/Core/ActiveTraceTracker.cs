using Core.Events;
using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core
{
    class ActiveTraceTracker
    {
        public double TraceTimeInSecondes = 30d;
        public Endpoint CurrentEndpoint;
        
        public void StartTrace()
        {
            this.CurrentEndpoint = Global.Bounce.BounceList[Global.Bounce.BounceList.Count - 1];
            Global.EventTicker.RegisterEvent(
                new Event("TRACE", TraceTimeInSecondes * CurrentEndpoint.ActiveTraceDificulty, this.TraceToNext, new object[0])
                );
        }

        public object[] TraceToNext(object [] arguments)
        {
            return new object[0];
        }
    }
}
