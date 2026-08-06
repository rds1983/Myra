using System;
using System.ComponentModel;

namespace Myra.Graphics2D.UI.Data
{
	/// <summary>
	/// Flags describing which grid capabilities a column type supports.
	/// </summary>
	[Flags]
	public enum DataGridColumnFlags
	{
		/// <summary>
		/// No capabilities.
		/// </summary>
		None = 0,

		/// <summary>
		/// The column supports per-column text filtering.
		/// </summary>
		CanFilter = 1 << 0,

		/// <summary>
		/// The column supports sorting by its values.
		/// </summary>
		CanSort = 1 << 1,

		/// <summary>
		/// The column supports both filtering and sorting.
		/// </summary>
		All = CanFilter | CanSort
	}

	/// <summary>
	/// Defines the base configuration for a DataGrid column, including its header text, bound property name, and width.
	/// </summary>
	public abstract class DataGridColumnBase
	{
		private string _filter;

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
		/// Gets the <see cref="DataGridColumnFlags"/> describing which capabilities this column type supports.
		/// </summary>
		public abstract DataGridColumnFlags Flags { get; }

		/// <summary>
		/// Gets or sets whether this column participates in sorting. Defaults to <c>true</c>.
		/// Ignored when the column does not have the <see cref="DataGridColumnFlags.CanSort"/> flag.
		/// </summary>
		[DefaultValue(true)]
		public bool HasSorting { get; set; } = true;

		/// <summary>
		/// Gets or sets whether this column shows a filter input in the filter row. Defaults to <c>true</c>.
		/// Ignored when the column does not have the <see cref="DataGridColumnFlags.CanFilter"/> flag.
		/// </summary>
		[DefaultValue(true)]
		public bool HasFilter { get; set; } = true;

		/// <summary>
		/// Gets or sets the horizontal alignment of the column header text. Defaults to <see cref="HorizontalAlignment.Left"/>.
		/// </summary>
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment HeaderHorizontalAlignment { get; set; } = HorizontalAlignment.Left;

		/// <summary>
		/// Gets or sets the horizontal alignment of the cell content in this column. Defaults to <see cref="HorizontalAlignment.Left"/>.
		/// </summary>
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment CellHorizontalAlignment { get; set; } = HorizontalAlignment.Left;

		/// <summary>
		/// Gets or sets the filter text applied to this column. Rows whose cell value does not contain
		/// this text are hidden. Setting this property on a column that does not support filtering throws an exception.
		/// </summary>
		public string Filter
		{
			get => _filter;

			set
			{
				if (!Flags.HasFlag(DataGridColumnFlags.CanFilter))
				{
					throw new Exception($"Column of type {GetType()} doesn't support filtering.");
				}

				_filter = value;
			}
		}

		/// <summary>
		/// Gets or sets the string comparison used when matching filter text against cell values for this column.
		/// When <c>null</c>, the DataGrid's <see cref="DataGrid.FilterStringComparison"/> is used instead.
		/// </summary>
		public StringComparison? FilterStringComparison { get; set; } = null;

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
