namespace Myra.Graphics2D.UI.Data
{
	/// <summary>
	/// Defines the base configuration for a DataGrid column, including its header text, bound property name, and width.
	/// </summary>
	public class DataGridColumnBase
	{
		/// <summary>
		/// Gets or sets the display text shown in the column header.
		/// </summary>
		public string Header { get; set; }

		/// <summary>
		/// Gets or sets the name of the property on the data item to bind this column to.
		/// </summary>
		public string Property { get; set; }

		/// <summary>
		/// Gets or sets the width of the column in pixels.
		/// </summary>
		public int Width { get; set; } = 100;
	}
}
