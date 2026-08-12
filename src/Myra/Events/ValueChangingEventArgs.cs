using Myra.Graphics2D.UI;

namespace Myra.Events
{
	/// <summary>
	/// Provides data for value changing events that can be cancelled.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public class ValueChangingEventArgs<T> : CancellableEventArgs
	{
		/// <summary>
		/// Gets the previous value before the change.
		/// </summary>
		public T OldValue
		{
			get; private set;
		}

		/// <summary>
		/// Gets or sets the new value after the change. Can be modified to change the final value.
		/// </summary>
		public T NewValue
		{
			get; set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangingEventArgs{T}"/> class.
		/// </summary>
		/// <param name="oldValue">The previous value.</param>
		/// <param name="newValue">The new value.</param>
		public ValueChangingEventArgs(T oldValue, T newValue) : base(InputEventType.ValueChanged)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}