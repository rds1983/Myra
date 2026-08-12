using Myra.Graphics2D.UI;

namespace Myra.Events
{
	/// <summary>
	/// Provides data for value changed events.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public class ValueChangedEventArgs<T> : MyraEventArgs
	{
		/// <summary>
		/// Gets the previous value before the change.
		/// </summary>
		public T OldValue
		{
			get; private set;
		}

		/// <summary>
		/// Gets the new value after the change.
		/// </summary>
		public T NewValue
		{
			get; private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangedEventArgs{T}"/> class.
		/// </summary>
		/// <param name="oldValue">The previous value.</param>
		/// <param name="newValue">The new value.</param>
		public ValueChangedEventArgs(T oldValue, T newValue) : base(InputEventType.ValueChanged)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}