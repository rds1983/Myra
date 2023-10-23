using FontStashSharp.Interfaces;
using Myra.Graphics2D.UI;
using System.Drawing;

namespace Myra.Platform
{
	public interface IMyraPlatform
	{
		Point ViewSize { get; }

		IMyraRenderer Renderer { get; }

		MouseInfo GetMouseInfo();

		/// <summary>
		/// Fills an array of keys pressed/released states.
		/// The index of the array represents value of <see cref="Keys"/>.
		/// Set value to true if a key is pressed, and to false if a key is released.
		/// </summary>
		/// <param name="keys"></param>
		void SetKeysDown(bool[] keys);
		void SetMouseCursorType(MouseCursorType mouseCursorType);

		TouchCollection GetTouchState();
	}
}
