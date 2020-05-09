using System;

namespace Myra.Graphics2D.UI
{
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
		All = 0,
		Vertical = 1,
		Horizontal = 2
	}
}