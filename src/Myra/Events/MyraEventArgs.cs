using Myra.Graphics2D.UI;

namespace Myra.Events
{
	/// <summary>
	/// Provides data for Myra UI events.
	/// </summary>
	public class MyraEventArgs
	{
		/// <summary>
		/// Gets an empty event args instance with no event type.
		/// </summary>
		public static readonly MyraEventArgs Empty = new MyraEventArgs(InputEventType.None);

		/// <summary>
		/// Gets the type of the input event.
		/// </summary>
		public InputEventType EventType { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MyraEventArgs"/> class with the specified event type.
		/// </summary>
		/// <param name="inputEventType">The type of the input event.</param>
		public MyraEventArgs(InputEventType inputEventType)
		{
			EventType = inputEventType;
		}

		/// <summary>
		/// Stops the propagation of the current event, preventing it from reaching other widgets in the propagation chain.
		/// </summary>
		public void StopPropagation()
		{
			InputEventsManager.StopPropagation(EventType);
		}
	}
}
