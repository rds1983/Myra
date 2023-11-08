using System.Collections.Generic;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Numerics;
using System.Drawing;
using Myra.Platform;
#endif

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

		public static void QueueMouseLeft(IInputEventsProcessor processor)
		{
			_events.Enqueue(new InputEvent(processor, InputEventType.MouseLeft));
		}

		public static void QueueMouseEntered(IInputEventsProcessor processor)
		{
			_events.Enqueue(new InputEvent(processor, InputEventType.MouseEntered));
		}

		public static void QueueMouseMoved(IInputEventsProcessor processor)
		{
			_events.Enqueue(new InputEvent(processor, InputEventType.MouseMoved));
		}

		public static void QueueMouseWheel(IInputEventsProcessor processor)
		{
			_events.Enqueue(new InputEvent(processor, InputEventType.MouseWheel));
		}

		public static void QueueTouchLeft(IInputEventsProcessor processor)
		{
			_events.Enqueue(new InputEvent(processor, InputEventType.TouchLeft));
		}

		public static void QueueTouchEntered(IInputEventsProcessor processor)
		{
			_events.Enqueue(new InputEvent(processor, InputEventType.TouchEntered));
		}

		public static void QueueTouchMoved(IInputEventsProcessor processor)
		{
			_events.Enqueue(new InputEvent(processor, InputEventType.TouchMoved));
		}

		public static void QueueTouchDown(IInputEventsProcessor processor)
		{
			_events.Enqueue(new InputEvent(processor, InputEventType.TouchDown));
		}

		public static void QueueTouchUp(IInputEventsProcessor processor)
		{
			_events.Enqueue(new InputEvent(processor, InputEventType.TouchUp));
		}

		public static void QueueTouchDoubleClick(IInputEventsProcessor processor)
		{
			_events.Enqueue(new InputEvent(processor, InputEventType.TouchDoubleClick));
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
