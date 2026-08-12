namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Specifies the horizontal alignment of an element.
	/// </summary>
	public enum HorizontalAlignment
	{
		/// <summary>Aligns the element to the left.</summary>
		Left,
		/// <summary>Centers the element horizontally.</summary>
		Center,
		/// <summary>Aligns the element to the right.</summary>
		Right,
		/// <summary>Stretches the element to fill available horizontal space.</summary>
		Stretch
	}

	/// <summary>
	/// Specifies the vertical alignment of an element.
	/// </summary>
	public enum VerticalAlignment
	{
		/// <summary>Aligns the element to the top.</summary>
		Top,
		/// <summary>Centers the element vertically.</summary>
		Center,
		/// <summary>Aligns the element to the bottom.</summary>
		Bottom,
		/// <summary>Stretches the element to fill available vertical space.</summary>
		Stretch
	}

	/// <summary>
	/// Specifies mouse button types.
	/// </summary>
	public enum MouseButtons
	{
		/// <summary>The left mouse button.</summary>
		Left,
		/// <summary>The middle mouse button.</summary>
		Middle,
		/// <summary>The right mouse button.</summary>
		Right
	}

	/// <summary>
	/// Specifies the orientation of an element (horizontal or vertical).
	/// </summary>
	public enum Orientation
	{
		/// <summary>Horizontal orientation.</summary>
		Horizontal,
		/// <summary>Vertical orientation.</summary>
		Vertical
	}

	/// <summary>
	/// Specifies the mouse cursor type to display.
	/// </summary>
	public enum MouseCursorType
	{
		/// <summary>Arrow cursor.</summary>
		Arrow,
		/// <summary>Text selection (I-beam) cursor.</summary>
		IBeam,
		/// <summary>Wait (hourglass) cursor.</summary>
		Wait,
		/// <summary>Crosshair cursor.</summary>
		Crosshair,
		/// <summary>Wait arrow (arrow with hourglass) cursor.</summary>
		WaitArrow,
		/// <summary>Northwest to southeast diagonal resize cursor.</summary>
		SizeNWSE,
		/// <summary>Northeast to southwest diagonal resize cursor.</summary>
		SizeNESW,
		/// <summary>Horizontal resize cursor.</summary>
		SizeWE,
		/// <summary>Vertical resize cursor.</summary>
		SizeNS,
		/// <summary>Move (all directions) cursor.</summary>
		SizeAll,
		/// <summary>Not allowed cursor.</summary>
		No,
		/// <summary>Hand (pointer) cursor.</summary>
		Hand,
	}
}