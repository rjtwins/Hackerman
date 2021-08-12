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