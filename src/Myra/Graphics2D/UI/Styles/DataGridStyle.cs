namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Defines the visual style for a <see cref="Data.DataGrid"/> widget, including scroll bar appearance.
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
		/// Gets or sets the image displayed next to the header of the currently sorted column when sorted ascending.
		/// </summary>
		public IImage SortAscendingImage { get; set; }

		/// <summary>
		/// Gets or sets the image displayed next to the header of the currently sorted column when sorted descending.
		/// </summary>
		public IImage SortDescendingImage { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridStyle"/> class with default values.
		/// </summary>
		public DataGridStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source DataGrid style to copy from.</param>
		public DataGridStyle(DataGridStyle style) : base(style)
		{
			VerticalScrollBackground = style.VerticalScrollBackground;
			VerticalScrollKnob = style.VerticalScrollKnob;
			SortAscendingImage = style.SortAscendingImage;
			SortDescendingImage = style.SortDescendingImage;
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
