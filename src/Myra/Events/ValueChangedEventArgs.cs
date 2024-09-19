using System;

namespace Myra.Events
{
	public class ValueChangedEventArgs<T> : EventArgs
	{
		public T OldValue
		{
			get; private set;
		}

		public T NewValue
		{
			get; private set;
		}

		public ValueChangedEventArgs(T oldValue, T newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
