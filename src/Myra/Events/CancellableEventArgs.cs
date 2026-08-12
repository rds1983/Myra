using Myra.Graphics2D.UI;

namespace Myra.Events
{
	/// <summary>
	/// Provides data for events that can be cancelled.
	/// </summary>
	public class CancellableEventArgs : MyraEventArgs
	{
		/// <summary>
		/// Gets or sets a value indicating whether the event should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CancellableEventArgs"/> class.
		/// </summary>
		/// <param name="inputEventType">The type of the input event.</param>
		public CancellableEventArgs(InputEventType inputEventType) : base(inputEventType)
		{
		}
	}
}