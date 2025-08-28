using Myra.Graphics2D.UI;

namespace Myra.Events
{
	public sealed class GenericEventArgs<T> : MyraEventArgs
	{
		public T Data { get; private set; }

		public GenericEventArgs(T value, InputEventType eventType) : base(eventType)
		{
			Data = value;
		}
	}
}