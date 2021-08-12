using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Events
{
    public class TestEvent : Event
    {
        public TestEvent() : base()
        { 
        }
        public override void StartEvent()
        {
            base.Name = "TestEvent";
            base.StartEvent();
        }
    }
}
