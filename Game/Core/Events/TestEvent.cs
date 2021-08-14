namespace Core.Events
{
    public class TestEvent : Event
    {
        public TestEvent() : base("TEST EVENT")
        {
        }

        public override object[] StartEvent()
        {
            base.Name = "TestEvent";
            return base.StartEvent();
        }
    }
}