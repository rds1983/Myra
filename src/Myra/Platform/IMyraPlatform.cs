using FontStashSharp.Interfaces;
using Myra.Graphics2D.UI;
using System.Drawing;

namespace Myra.Platform
{
	/// <summary>
	/// Provides platform-specific functionality for the Myra UI framework.
	/// </summary>
	public interface IMyraPlatform
	{
		/// <summary>
		/// Gets the size of the game view.
		/// </summary>
		Point ViewSize { get; }

		/// <summary>
		/// Gets the renderer instance for rendering graphics.
		/// </summary>
		IMyraRenderer Renderer { get; }

		/// <summary>
		/// Gets information about the current mouse state.
		/// </summary>
		/// <returns>Mouse information including position and pressed button state.</returns>
		MouseInfo GetMouseInfo();

		/// <summary>
		/// Fills an array of keys pressed/released states.
		/// The index of the array represents value of <see cref="Keys"/>.
		/// Set value to true if a key is pressed, and to false if a key is released.
		/// </summary>
		/// <param name="keys">An array of key states to fill.</param>
		void SetKeysDown(bool[] keys);

		/// <summary>
		/// Sets the type of mouse cursor to display.
		/// </summary>
		/// <param name="mouseCursorType">The type of cursor to display.</param>
		void SetMouseCursorType(MouseCursorType mouseCursorType);

		/// <summary>
		/// Gets the current state of all touch points.
		/// </summary>
		/// <returns>A collection of touch points currently active.</returns>
		TouchCollection GetTouchState();
	}
}
