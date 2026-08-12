namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of scroll viewer widgets.
	/// </summary>
	public class ScrollViewerStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the image used for the horizontal scrollbar background.
		/// </summary>
		public IImage HorizontalScrollBackground { get; set; }

		/// <summary>
		/// Gets or sets the image used for the horizontal scrollbar thumb (draggable knob).
		/// </summary>
		public IImage HorizontalScrollKnob { get; set; }

		/// <summary>
		/// Gets or sets the image used for the vertical scrollbar background.
		/// </summary>
		public IImage VerticalScrollBackground { get; set; }

		/// <summary>
		/// Gets or sets the image used for the vertical scrollbar thumb (draggable knob).
		/// </summary>
		public IImage VerticalScrollKnob { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ScrollViewerStyle"/> class.
		/// </summary>
		public ScrollViewerStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScrollViewerStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source scroll viewer style to copy from.</param>
		public ScrollViewerStyle(ScrollViewerStyle style) : base(style)
		{
			HorizontalScrollBackground = style.HorizontalScrollBackground;
			HorizontalScrollKnob = style.HorizontalScrollKnob;
			VerticalScrollBackground = style.VerticalScrollBackground;
			VerticalScrollKnob = style.VerticalScrollKnob;
		}

		/// <summary>
		/// Creates a deep copy of this scroll viewer style.
		/// </summary>
		/// <returns>A new ScrollViewerStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new ScrollViewerStyle(this);
		}
	}
}