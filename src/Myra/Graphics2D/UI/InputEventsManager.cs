using Myra.Events;
using System.Collections.Generic;
using System.Linq;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Specifies the type of input event that occurred.
	/// </summary>
	public enum InputEventType
	{
		/// <summary>No input event type.</summary>
		None = -1,
		/// <summary>The mouse left a widget.</summary>
		MouseLeft,
		/// <summary>The mouse entered a widget.</summary>
		MouseEntered,
		/// <summary>The mouse moved over a widget.</summary>
		MouseMoved,
		/// <summary>The mouse wheel was scrolled.</summary>
		MouseWheel,
		/// <summary>A touch point left a widget.</summary>
		TouchLeft,
		/// <summary>A touch point entered a widget.</summary>
		TouchEntered,
		/// <summary>A touch point moved over a widget.</summary>
		TouchMoved,
		/// <summary>A touch point pressed down.</summary>
		TouchDown,
		/// <summary>A touch point was released.</summary>
		TouchUp,
		/// <summary>A touch point double-clicked.</summary>
		TouchDoubleClick,
		/// <summary>A keyboard key was released.</summary>
		KeyUp,
		/// <summary>A keyboard key was pressed.</summary>
		KeyDown,
		/// <summary>A widget is closing.</summary>
		Closing,
		/// <summary>A widget is losing keyboard focus.</summary>
		KeyboardFocusLosing,
		/// <summary>A context menu is closing.</summary>
		ContextMenuClosing,
		/// <summary>A character was input.</summary>
		CharInput,
		/// <summary>Text was deleted.</summary>
		TextDeleted,
		/// <summary>The cursor position changed.</summary>
		CursorPositionChanged,
		/// <summary>Text was changed by the user.</summary>
		TextChangedByUser,
		/// <summary>Text was changed.</summary>
		TextChanged,
		/// <summary>The selected index changed.</summary>
		SelectedIndexChanged,
		/// <summary>The hover index changed.</summary>
		HoverIndexChanged,
		/// <summary>A proportion value changed.</summary>
		ProportionChanged,
		/// <summary>A widget's enabled state changed.</summary>
		EnabledChanged,
		/// <summary>Keyboard focus changed.</summary>
		KeyboardFocusChanged,
		/// <summary>The widget's arrangement was updated.</summary>
		ArrangeUpdated,
		/// <summary>A widget's placed state changed.</summary>
		PlacedChanged,
		/// <summary>A widget's visible state changed.</summary>
		VisibleChanged,
		/// <summary>A widget's location changed.</summary>
		LocationChanged,
		/// <summary>A widget's size changed.</summary>
		SizeChanged,
		/// <summary>The selection changed.</summary>
		SelectionChanged,
		/// <summary>A value changed.</summary>
		ValueChanged,
		/// <summary>A widget's pressed state changed.</summary>
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

		private static readonly Queue<InputEvent> _eventsQueue = new Queue<InputEvent>();

		private static readonly Stack<InputEvent> _eventsStack = new Stack<InputEvent>();

		public static void Queue(IInputEventsProcessor processor, InputEventType type)
		{
			var ev = new InputEvent(processor, type);

			switch (MyraEnvironment.EventHandlingModel)
			{
				case EventHandlingStrategy.EventCapturing:
					QueueEventsQueue(ev);
					break;
				case EventHandlingStrategy.EventBubbling:
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
				case EventHandlingStrategy.EventCapturing:
					ProcessEventsQueue();
					break;
				case EventHandlingStrategy.EventBubbling:
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

		private static IEnumerable<InputEvent> GetInputEvents()
		{
			if (MyraEnvironment.EventHandlingModel == EventHandlingStrategy.EventBubbling)
			{
				return _eventsStack;
			}

			return _eventsQueue;
		}

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
				case EventHandlingStrategy.EventCapturing:
					_eventsQueue.Clear();
					foreach (var @event in events)
					{
						_eventsQueue.Enqueue(@event);
					}
					break;
				case EventHandlingStrategy.EventBubbling:
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
