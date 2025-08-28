using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Myra.Graphics2D.UI
{
	public enum InputEventType
    {
        None = -1,
        MouseLeft,
		MouseEntered,
		MouseMoved,
		MouseWheel,
		TouchLeft,
		TouchEntered,
		TouchMoved,
		TouchDown,
		TouchUp,
		TouchDoubleClick,
		KeyUp,
		KeyDown,
        Closing,
        KeyboardFocusLosing,
        ContextMenuClosing,
        CharInput,
        TextDeleted,
        CursorPositionChanged,
        TextChangedByUser,
        TextChanged,
        SelectedIndexChanged,
        HoverIndexChanged,
        ProportionChanged,
        EnabledChanged,
        KeyboardFocusChanged,
        ArrangeUpdated,
        PlacedChanged,
        VisibleChanged,
        LocationChanged,
        SizeChanged,
        SelectionChanged,
        ValueChanged,
        PressedChanged
    }

	internal interface IInputEventsProcessor
	{
		void ProcessEvent(InputEventType eventType);
	}

	internal static class InputEventsManager
	{
		private struct InputEvent
		{
			public IInputEventsProcessor Processor;
			public InputEventType Type;

			public InputEvent(IInputEventsProcessor processor, InputEventType type)
			{
				Processor = processor;
				Type = type;
			}
		}

		private static readonly Queue<InputEvent> _eventsQueue = new();

		private static readonly Stack<InputEvent> _eventsStack = new();

        public static void Queue(IInputEventsProcessor processor, InputEventType type)
		{
			var ev = new InputEvent(processor, type);

            switch (MyraEnvironment.EventHandlingModel)
            {
                case Events.EventHandlingStrategy.EventCapturing:
                    QueueEventsQueue(ev);
                    break;
                case Events.EventHandlingStrategy.EventBubbling:
                    QueueEventsStack(ev);
                    break;
                default:
                    break;
            }
        }

		private static void QueueEventsQueue(InputEvent ev)
        {
            _eventsQueue.Enqueue(ev);
        }

        private static void QueueEventsStack(InputEvent ev)
        {
			_eventsStack.Push(ev);
        }

        public static void ProcessEvents()
		{
			switch (MyraEnvironment.EventHandlingModel)
			{
				case Events.EventHandlingStrategy.EventCapturing:
					ProcessEventsQueue();
                    break;
				case Events.EventHandlingStrategy.EventBubbling:
                    ProcessEventsStack();
                    break;
				default:
					break;
			}
		}

		private static void ProcessEventsQueue()
        {
            while (_eventsQueue.Count > 0)
            {
                var ev = _eventsQueue.Dequeue();
                ProcessEventInternal(ev);
            }
        }

        private static void ProcessEventsStack()
        {
            while (_eventsStack.Count > 0)
            {
                var ev = _eventsStack.Pop();
				ProcessEventInternal(ev);
            }
        }

		private static void ProcessEventInternal(InputEvent ev)
		{
			ev.Processor.ProcessEvent(ev.Type);

        }

        private static IEnumerable<InputEvent> GetInputEvents() => MyraEnvironment.EventHandlingModel switch
        {
            Events.EventHandlingStrategy.EventBubbling => _eventsStack,
            _ => _eventsQueue,
        };

        public static void StopPropagation(InputEventType eventType)
		{
			var events = GetInputEvents()
				.Where(x => x.Type != eventType)
				.ToArray();

			SetInputEvents(events);

        }

		private static void SetInputEvents(IEnumerable<InputEvent> events)
		{
			switch (MyraEnvironment.EventHandlingModel)
			{
				case Events.EventHandlingStrategy.EventCapturing:
                    _eventsQueue.Clear();
					foreach (var @event in events)
					{
						_eventsQueue.Enqueue(@event);
					}
					break;
				case Events.EventHandlingStrategy.EventBubbling:
                    _eventsStack.Clear();
                    foreach (var @event in events)
                    {
                        _eventsStack.Push(@event);
                    }
                    break;
				default:
					break;
			}
		}
    }
}
