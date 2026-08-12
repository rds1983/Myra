namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of separator widgets.
	/// </summary>
	public class SeparatorStyle : ImageStyle
	{
		/// <summary>
		/// Gets or sets the thickness of the separator line in pixels.
		/// </summary>
		public int Thickness { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SeparatorStyle"/> class.
		/// </summary>
		public SeparatorStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SeparatorStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source separator style to copy from.</param>
		public SeparatorStyle(SeparatorStyle style) : base(style)
		{
			Thickness = style.Thickness;
		}

		/// <summary>
		/// Creates a deep copy of this separator style.
		/// </summary>
		/// <returns>A new SeparatorStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new SeparatorStyle(this);
		}
	}
}
