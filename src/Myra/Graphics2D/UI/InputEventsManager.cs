using System.Collections.Generic;

namespace Myra.Graphics2D.UI
{
	internal enum InputEventType
	{
		MouseLeft,
		MouseEntered,
		MouseMoved,
		MouseWheel,
		TouchLeft,
		TouchEntered,
		TouchMoved,
		TouchDown,
		TouchUp,
		TouchDoubleClick
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

		private static readonly Queue<InputEvent> _events = new Queue<InputEvent>();

		public static void Queue(IInputEventsProcessor processor, InputEventType type)
		{
			_events.Enqueue(new InputEvent(processor, type));
		}

		public static void ProcessEvents()
		{
			while(_events.Count > 0)
			{
				var ev = _events.Dequeue();

				ev.Processor.ProcessEvent(ev.Type);
			}
		}
	}
}
