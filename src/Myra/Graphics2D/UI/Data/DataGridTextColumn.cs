using System.Globalization;

namespace Myra.Graphics2D.UI.Data
{
	/// <summary>
	/// A DataGrid column that displays text values, inheriting configuration from <see cref="DataGridColumnBase"/>.
	/// </summary>
	public class DataGridTextColumn : DataGridColumnBase
	{
		/// <summary>
		/// Gets or sets the format string applied to the cell value before display (e.g. <c>"{0:C2}"</c>).
		/// When <c>null</c> or empty, the value's <c>ToString</c> result is used directly.
		/// </summary>
		public string Format { get; set; }

		/// <inheritdoc/>
		public override bool HasFilter => true;

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
		/// <param name="format">An optional format string applied to the value (e.g. <c>"{0:C2}"</c>).</param>
		public DataGridTextColumn(string property, string header, int width = 100, string format = null) : base(property, header, width)
		{
			Format = format;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridTextColumn"/> class with the specified property and width,
		/// using the property name as the header text.
		/// </summary>
		/// <param name="property">The name of the property on the data object to bind to.</param>
		/// <param name="width">The width of the column in pixels.</param>
		/// <param name="format">An optional format string applied to the value (e.g. <c>"{0:C2}"</c>).</param>
		public DataGridTextColumn(string property, int width = 100, string format = null) : this(property, property, width, format)
		{
		}

		/// <inheritdoc/>
		public override Widget CreateWidget(object value)
		{
			if (value == null)
			{
				return null;
			}

			string strValue;
			if (string.IsNullOrEmpty(Format))
			{
				strValue = value.ToString();
			}
			else
			{
				strValue = string.Format(Format, value);
			}

			return new Label
			{
				Text = strValue
			};
		}
	}
}
