namespace Myra.Graphics2D.UI
{
	public class InputContext
	{
		public bool MouseOrTouchHandled { get; set; }
		public Widget MouseWheelWidget { get; set; }

		public void Reset()
		{
			MouseOrTouchHandled = false;
			MouseWheelWidget = null;
		}
	}
}
