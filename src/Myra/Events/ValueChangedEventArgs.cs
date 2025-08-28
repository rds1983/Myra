using Myra.Graphics2D.UI;

namespace Myra.Events
{
	public class ValueChangedEventArgs<T> : MyraEventArgs
	{
		public T OldValue
		{
			get; private set;
		}

		public T NewValue
		{
			get; private set;
		}

		public ValueChangedEventArgs(T oldValue, T newValue) : base(InputEventType.ValueChanged)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}