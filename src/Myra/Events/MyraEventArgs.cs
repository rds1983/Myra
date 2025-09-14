using Myra.Graphics2D.UI;

namespace Myra.Events
{
    public class MyraEventArgs
    {
        public static readonly MyraEventArgs Empty = new MyraEventArgs(InputEventType.None);

        public InputEventType EventType { get; private set; }

        public MyraEventArgs(InputEventType inputEventType)
        {
            EventType = inputEventType;
        }
        public void StopPropagation()
        {
            InputEventsManager.StopPropagation(EventType);
        }
    }
}
