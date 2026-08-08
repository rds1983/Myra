using System.ComponentModel;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Defines the visual style of DataGrid header cells, including the header label appearance
	/// and the images used for the sort indicators.
	/// </summary>
	public class DataGridHeaderStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style applied to the header text labels.
		/// </summary>
		public LabelStyle LabelStyle { get; set; }

		/// <summary>
		/// Gets or sets the spacing between the header text and the sort indicator image.
		/// </summary>
		[DefaultValue(0)]
		public int SortImageTextSpacing { get; set; }

		/// <summary>
		/// Gets or sets the image displayed next to the header when the column is sorted ascending.
		/// </summary>
		public IImage SortAscendingImage { get; set; }

		/// <summary>
		/// Gets or sets the image displayed next to the header when the column is sorted descending.
		/// </summary>
		public IImage SortDescendingImage { get; set; }

		/// <summary>
		/// Gets or sets the padding applied around the header cell content.
		/// </summary>
		public Thickness ContentPadding { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridHeaderStyle"/> class.
		/// </summary>
		public DataGridHeaderStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridHeaderStyle"/> class as a deep copy of the specified style.
		/// </summary>
		/// <param name="style">The style to copy.</param>
		public DataGridHeaderStyle(DataGridHeaderStyle style) : base(style)
		{
			LabelStyle = style.LabelStyle != null ? new LabelStyle(style.LabelStyle) : null;
			SortImageTextSpacing = style.SortImageTextSpacing;
			SortAscendingImage = style.SortAscendingImage;
			SortDescendingImage = style.SortDescendingImage;
			ContentPadding = style.ContentPadding;
		}

		/// <summary>
		/// Creates a deep copy of this DataGrid header style.
		/// </summary>
		/// <returns>A new <see cref="DataGridHeaderStyle"/> instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new DataGridHeaderStyle(this);
		}
	}
}
