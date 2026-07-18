namespace Myra.Graphics2D.UI.Styles
{
	public class DataGridStyle : GridStyle
	{
		public IImage VerticalScrollBackground { get; set; }

		public IImage VerticalScrollKnob { get; set; }

		public DataGridStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScrollViewerStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source scroll viewer style to copy from.</param>
		public DataGridStyle(DataGridStyle style) : base(style)
		{
			VerticalScrollBackground = style.VerticalScrollBackground;
			VerticalScrollKnob = style.VerticalScrollKnob;
		}

		/// <summary>
		/// Creates a deep copy of this scroll viewer style.
		/// </summary>
		/// <returns>A new ScrollViewerStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new DataGridStyle(this);
		}
	}
}
