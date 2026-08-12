using Myra.Graphics2D.UI;

namespace Myra.Events
{
	/// <summary>
	/// Provides data for cancellable events with associated data.
	/// </summary>
	/// <typeparam name="T">The type of data associated with the event.</typeparam>
	public class CancellableEventArgs<T> : MyraEventArgs
	{
		/// <summary>
		/// Gets the data associated with the event.
		/// </summary>
		public T Data { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the event should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CancellableEventArgs{T}"/> class.
		/// </summary>
		/// <param name="data">The data to associate with the event.</param>
		/// <param name="inputEventType">The type of the input event.</param>
		public CancellableEventArgs(T data, InputEventType inputEventType) : base(inputEventType)
		{
			Data = data;
		}
	}
}