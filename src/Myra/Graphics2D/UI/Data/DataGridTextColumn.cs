namespace Myra.Graphics2D.UI.Data
{
	/// <summary>
	/// A DataGrid column that displays text values, inheriting configuration from <see cref="DataGridColumnBase"/>.
	/// </summary>
	public class DataGridTextColumn : DataGridColumnBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridTextColumn"/> class with default values.
		/// </summary>
		public DataGridTextColumn()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridTextColumn"/> class with the specified property, header, and width.
		/// </summary>
		/// <param name="property">The name of the property on the data object to bind to.</param>
		/// <param name="header">The text displayed in the column header.</param>
		/// <param name="width">The width of the column in pixels.</param>
		public DataGridTextColumn(string property, string header, int width = 100) : base(property, header, width)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridTextColumn"/> class with the specified property and width,
		/// using the property name as the header text.
		/// </summary>
		/// <param name="property">The name of the property on the data object to bind to.</param>
		/// <param name="width">The width of the column in pixels.</param>
		public DataGridTextColumn(string property, int width = 100) : this(property, property, width)
		{
		}
	}
}
