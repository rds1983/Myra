namespace Myra
{
	partial class MyraEnvironment
	{
		/// <summary>
		/// Gets or sets a value indicating whether to draw debug frames around all widgets.
		/// </summary>
		public static bool DrawWidgetsFrames { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to draw a debug frame around the keyboard-focused widget.
		/// </summary>
		public static bool DrawKeyboardFocusedWidgetFrame { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to draw a debug frame around the widget under the mouse cursor.
		/// </summary>
		public static bool DrawMouseHoveredWidgetFrame { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to draw debug frames around text glyphs.
		/// </summary>
		public static bool DrawTextGlyphsFrames { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether clipping is disabled (useful for debugging).
		/// </summary>
		public static bool DisableClipping { get; set; }
	}
}
