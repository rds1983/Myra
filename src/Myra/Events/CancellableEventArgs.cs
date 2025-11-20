using Myra.Graphics2D.UI;

namespace Myra.Events
{
	public class CancellableEventArgs : MyraEventArgs
	{
		public bool Cancel { get; set; }

		public CancellableEventArgs(InputEventType inputEventType) : base(inputEventType)
		{
		}
	}
}