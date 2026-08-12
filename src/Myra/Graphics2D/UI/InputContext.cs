namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Provides context information for mouse and touch input handling in the UI.
	/// </summary>
	public class InputContext
	{
		/// <summary>
		/// Gets or sets a value indicating whether the current mouse or touch input has been handled.
		/// </summary>
		public bool MouseOrTouchHandled { get; set; }

		/// <summary>
		/// Gets or sets the widget that will receive mouse wheel events.
		/// </summary>
		public Widget MouseWheelWidget { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the parent widget contains the mouse cursor.
		/// </summary>
		public bool ParentContainsMouse { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the parent widget contains a touch point.
		/// </summary>
		public bool ParentContainsTouch { get; set; }

		/// <summary>
		/// Resets the input context to its default state.
		/// </summary>
		public void Reset()
		{
			MouseOrTouchHandled = false;
			MouseWheelWidget = null;
			ParentContainsMouse = true;
			ParentContainsTouch = true;
		}
	}
}
