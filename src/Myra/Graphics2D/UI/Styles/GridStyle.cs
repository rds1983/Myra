#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using System.ComponentModel;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance and behavior of grid widgets.
	/// </summary>
	public class GridStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets a value indicating whether grid lines are displayed for debugging purposes.
		/// </summary>
		[DefaultValue(false)]
		public bool ShowGridLines { get; set; }

		/// <summary>
		/// Gets or sets the color of the grid lines when displayed.
		/// </summary>
		public Color GridLinesColor { get; set; }

		/// <summary>
		/// Gets or sets the spacing in pixels between grid columns.
		/// </summary>
		[DefaultValue(0)]
		public int ColumnSpacing { get; set; }

		/// <summary>
		/// Gets or sets the spacing in pixels between grid rows.
		/// </summary>
		[DefaultValue(0)]
		public int RowSpacing { get; set; }

		/// <summary>
		/// Gets or sets the brush used to draw the background of selected rows, columns, or cells.
		/// </summary>
		public IBrush SelectionBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used to draw the background of rows, columns, or cells being hovered over.
		/// </summary>
		public IBrush SelectionHoverBackground { get; set; }

		/// <summary>
		/// Gets or sets the selection mode for the grid (rows, columns, cells, or none).
		/// </summary>
		[DefaultValue(GridSelectionMode.None)]
		public GridSelectionMode GridSelectionMode { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the hover index can be null when the mouse is outside the grid.
		/// </summary>
		[DefaultValue(false)]
		public bool HoverIndexCanBeNull { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether nothing can be selected by clicking an already-selected item.
		/// </summary>
		[DefaultValue(false)]
		public bool CanSelectNothing { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GridStyle"/> class.
		/// </summary>
		public GridStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GridStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source grid style to copy from.</param>
		public GridStyle(GridStyle style) : base(style)
		{
			ShowGridLines = style.ShowGridLines;
			GridLinesColor = style.GridLinesColor;
			ColumnSpacing = style.ColumnSpacing;
			RowSpacing = style.RowSpacing;
			SelectionBackground = style.SelectionBackground;
			SelectionHoverBackground = style.SelectionHoverBackground;
			GridSelectionMode = style.GridSelectionMode;
			HoverIndexCanBeNull = style.HoverIndexCanBeNull;
			CanSelectNothing = style.CanSelectNothing;
		}

		/// <summary>
		/// Creates a deep copy of this grid style.
		/// </summary>
		/// <returns>A new GridStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new GridStyle(this);
		}
	}
}
