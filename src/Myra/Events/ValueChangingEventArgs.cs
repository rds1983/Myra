using Myra.Graphics2D.UI;

namespace Myra.Events
{
	public class ValueChangingEventArgs<T> : CancellableEventArgs
	{
		public T OldValue
		{
			get; private set;
		}

		public T NewValue
		{
			get; set;
		}

		public ValueChangingEventArgs(T oldValue, T newValue) : base(InputEventType.ValueChanged)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}