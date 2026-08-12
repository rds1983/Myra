using Myra.Graphics2D.UI;

namespace Myra.Events
{
	/// <summary>
	/// Generic event arguments for passing arbitrary data with an event.
	/// </summary>
	/// <typeparam name="T">The type of data to pass with the event.</typeparam>
	public sealed class GenericEventArgs<T> : MyraEventArgs
	{
		/// <summary>
		/// Gets the data associated with the event.
		/// </summary>
		public T Data { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericEventArgs{T}"/> class.
		/// </summary>
		/// <param name="value">The data to associate with the event.</param>
		/// <param name="eventType">The type of the input event.</param>
		public GenericEventArgs(T value, InputEventType eventType) : base(eventType)
		{
			Data = value;
		}
	}
}