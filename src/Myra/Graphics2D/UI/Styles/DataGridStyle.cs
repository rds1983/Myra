namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Defines the visual style for a <see cref="Data.DataGrid"/> widget, including the header,
	/// filter, and cell styles, as well as scrollbar appearance.
	/// </summary>
	public class DataGridStyle : GridStyle
	{
		/// <summary>
		/// Gets or sets the image used for the vertical scrollbar track background.
		/// </summary>
		public IImage VerticalScrollBackground { get; set; }

		/// <summary>
		/// Gets or sets the image used for the vertical scrollbar thumb (knob).
		/// </summary>
		public IImage VerticalScrollKnob { get; set; }

		/// <summary>
		/// Gets or sets the style applied to the header cells.
		/// </summary>
		public DataGridHeaderStyle HeaderStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to the filter row text inputs.
		/// </summary>
		public TextBoxStyle FilterStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to text cell content.
		/// </summary>
		public LabelStyle TextCellStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to check box cell content.
		/// </summary>
		public ImageStyle CheckCellStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to image cell content.
		/// </summary>
		public WidgetStyle ImageCellStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridStyle"/> class.
		/// </summary>
		public DataGridStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridStyle"/> class as a deep copy of the specified style.
		/// </summary>
		/// <param name="style">The style to copy.</param>
		public DataGridStyle(DataGridStyle style) : base(style)
		{
			VerticalScrollBackground = style.VerticalScrollBackground;
			VerticalScrollKnob = style.VerticalScrollKnob;
			HeaderStyle = style.HeaderStyle != null ? new DataGridHeaderStyle(style.HeaderStyle) : null;
			FilterStyle = style.FilterStyle != null ? new TextBoxStyle(style.FilterStyle) : null;
			TextCellStyle = style.TextCellStyle != null ? new LabelStyle(style.TextCellStyle) : null;
			CheckCellStyle = style.CheckCellStyle != null ? new ImageStyle(style.CheckCellStyle) : null;
			ImageCellStyle = style.ImageCellStyle != null ? new WidgetStyle(style.ImageCellStyle) : null;
		}

		/// <summary>
		/// Creates a deep copy of this DataGrid style.
		/// </summary>
		/// <returns>A new <see cref="DataGridStyle"/> instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new DataGridStyle(this);
		}
	}
}
