using System;

namespace Myra.Graphics2D.UI
{
	public enum TextAlign
	{
		Left,
		Center,
		Right
	}

	public enum HorizontalAlignment
	{
		Left,
		Center,
		Right,
		Stretch
	}

	public enum VerticalAlignment
	{
		Top,
		Center,
		Bottom,
		Stretch
	}

	public enum MouseButtons
	{
		Left,
		Middle,
		Right
	}

	public enum Orientation
	{
		Horizontal,
		Vertical
	}

	public enum SelectionMode
	{
		/// <summary>
		/// Only one item can be selected
		/// </summary>
		Single,

		/// <summary>
		/// Multiple items can be selected
		/// </summary>
		Multiple
	}

	[Flags]
	public enum DragDirection
	{
		None = 0,
		Vertical = 1,
		Horizontal = 2,
		Both = Vertical | Horizontal
	}

	public enum ImageResizeMode
	{
		/// <summary>
		/// Simply Stretch
		/// </summary>
		Stretch,

		/// <summary>
		/// Keep Aspect Ratio
		/// </summary>
		KeepAspectRatio
	}
}