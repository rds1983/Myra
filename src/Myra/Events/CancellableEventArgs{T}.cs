using Myra.Graphics2D.UI;

namespace Myra.Events
{
	public class CancellableEventArgs<T> : MyraEventArgs
	{
		public T Data { get; private set; }
		public bool Cancel { get; set; }

		public CancellableEventArgs(T data, InputEventType inputEventType) : base(inputEventType)
		{
			Data = data;
		}
	}
}