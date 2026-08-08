using Myra.Graphics2D.UI.Styles;
using System;

namespace Myra.Graphics2D.UI.Data
{
	/// <summary>
	/// A DataGrid column that displays boolean values as read-only check boxes.
	/// Supports sorting but not text filtering.
	/// </summary>
	public class DataGridCheckBoxColumn : DataGridColumnBase
	{
		/// <inheritdoc/>
		public override DataGridColumnFlags Flags => DataGridColumnFlags.CanSort;

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridCheckBoxColumn"/> class with the specified property, header, and width.
		/// </summary>
		/// <param name="property">The name of the property on the data object to bind to.</param>
		/// <param name="header">The text displayed in the column header.</param>
		/// <param name="width">The width of the column in pixels.</param>
		public DataGridCheckBoxColumn(string property, string header, int width = 100) : base(property, header, width)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridCheckBoxColumn"/> class with the specified property and width,
		/// using the property name as the header text.
		/// </summary>
		/// <param name="property">The name of the property on the data object to bind to.</param>
		/// <param name="width">The width of the column in pixels.</param>
		public DataGridCheckBoxColumn(string property, int width = 100) : base(property, width)
		{
		}

		/// <inheritdoc/>
		public override Widget CreateWidget(object value, DataGridStyle style)
		{
			if (!(value is bool))
			{
				throw new Exception("DataGridCheckBoxColumn can only be used with boolean values.");
			}

			if (style.CheckCellStyle == null)
			{
				throw new Exception("CheckCellStyle is null");
			}

			var check = new Image
			{
				IsPressed = (bool)value
			};
			check.ApplyImageStyle(style.CheckCellStyle);

			return check;
		}
	}
}
