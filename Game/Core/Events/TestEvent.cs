namespace Core.Events
{
    public class TestEvent : Event
    {
        public TestEvent() : base()
        {
        }

        public override object[] StartEvent()
        {
            base.Name = "TestEvent";
            return base.StartEvent();
        }
    }
}