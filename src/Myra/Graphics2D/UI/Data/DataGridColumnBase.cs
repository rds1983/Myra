namespace Myra.Graphics2D.UI.Data
{
	/// <summary>
	/// Defines the base configuration for a DataGrid column, including its header text, bound property name, and width.
	/// </summary>
	public abstract class DataGridColumnBase
	{
		/// <summary>
		/// Gets or sets the name of the property on the data item to bind this column to.
		/// </summary>
		public string Property { get; set; }

		/// <summary>
		/// Gets or sets the display text shown in the column header.
		/// </summary>
		public string Header { get; set; }

		/// <summary>
		/// Gets or sets the width of the column in pixels.
		/// </summary>
		public int Width { get; set; } = 100;

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridColumnBase"/> class with default values.
		/// </summary>
		public DataGridColumnBase()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridColumnBase"/> class with the specified property, header, and width.
		/// </summary>
		/// <param name="property">The name of the property on the data object to bind to.</param>
		/// <param name="header">The text displayed in the column header.</param>
		/// <param name="width">The width of the column in pixels.</param>
		public DataGridColumnBase(string property, string header, int width = 100)
		{
			Property = property;
			Header = header;
			Width = width;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridColumnBase"/> class with the specified property and width,
		/// using the property name as the header text.
		/// </summary>
		/// <param name="property">The name of the property on the data object to bind to.</param>
		/// <param name="width">The width of the column in pixels.</param>
		public DataGridColumnBase(string property, int width = 100) : this(property, null, width)
		{
		}

		/// <summary>
		/// Creates the widget used to render a cell value in the grid.
		/// </summary>
		/// <param name="value">The data value to display.</param>
		/// <returns>A <see cref="Widget"/> representing the cell, or <c>null</c> if the value should not be rendered.</returns>
		public abstract Widget CreateWidget(object value);
	}
}
