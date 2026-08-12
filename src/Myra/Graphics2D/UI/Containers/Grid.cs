using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.MML;
using Myra.Events;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System.Collections;



#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Specifies the selection mode for a grid widget.
	/// </summary>
	public enum GridSelectionMode
	{
		/// <summary>No selection is allowed.</summary>
		None,
		/// <summary>Entire rows can be selected.</summary>
		Row,
		/// <summary>Entire columns can be selected.</summary>
		Column,
		/// <summary>Individual cells can be selected.</summary>
		Cell
	}

	/// <summary>
	/// A grid widget that displays content in a table layout with support for rows, columns, spanning, and selection.
	/// </summary>
	public class Grid : Container
	{
		/// <summary>
		/// Attached property that specifies the column index of a child widget within the grid.
		/// </summary>
		public static readonly AttachedPropertyInfo<int> ColumnProperty =
			AttachedPropertiesRegistry.Create(typeof(Grid), "Column", 0,
				AttachedPropertyOption.AffectsArrange,
				new Attribute[] { new RangeAttribute(0) });

		/// <summary>
		/// Attached property that specifies the row index of a child widget within the grid.
		/// </summary>
		public static readonly AttachedPropertyInfo<int> RowProperty =
			AttachedPropertiesRegistry.Create(typeof(Grid), "Row", 0,
				AttachedPropertyOption.AffectsArrange,
				new Attribute[] { new RangeAttribute(0) });

		/// <summary>
		/// Attached property that specifies the number of columns a child widget spans.
		/// </summary>
		public static readonly AttachedPropertyInfo<int> ColumnSpanProperty =
			AttachedPropertiesRegistry.Create(typeof(Grid), "ColumnSpan", 1,
				AttachedPropertyOption.AffectsArrange,
				new Attribute[] { new RangeAttribute(1) });

		/// <summary>
		/// Attached property that specifies the number of rows a child widget spans.
		/// </summary>
		public static readonly AttachedPropertyInfo<int> RowSpanProperty =
			AttachedPropertiesRegistry.Create(typeof(Grid), "RowSpan", 1,
				AttachedPropertyOption.AffectsArrange,
				new Attribute[] { new RangeAttribute(1) });

		private readonly GridLayout _layout = new GridLayout();

		private int? _hoverRowIndex = null;
		private int? _hoverColumnIndex = null;
		private int? _selectedRowIndex = null;
		private int? _selectedColumnIndex = null;

		/// <summary>
		/// Gets or sets a value indicating whether grid lines are displayed for debugging purposes.
		/// </summary>
		[Category("Debug")]
		[DefaultValue(false)]
		public bool ShowGridLines { get; set; }

		/// <summary>
		/// Gets or sets the color of the grid lines when displayed.
		/// </summary>
		[Category("Debug")]
		[DefaultValue("White")]
		public Color GridLinesColor { get; set; }

		/// <summary>
		/// Gets or sets the spacing in pixels between grid columns.
		/// </summary>
		[Category("Grid")]
		[DefaultValue(0)]
		public int ColumnSpacing
		{
			get => _layout.ColumnSpacing;
			set
			{
				if (value == _layout.ColumnSpacing)
				{
					return;
				}

				_layout.ColumnSpacing = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the spacing in pixels between grid rows.
		/// </summary>
		[Category("Grid")]
		[DefaultValue(0)]
		public int RowSpacing
		{
			get { return _layout.RowSpacing; }
			set
			{
				if (value == _layout.RowSpacing)
				{
					return;
				}

				_layout.RowSpacing = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the default proportion used for columns when not explicitly specified.
		/// </summary>
		[Browsable(false)]
		public Proportion DefaultColumnProportion
		{
			get => _layout.DefaultColumnProportion;
			set => _layout.DefaultColumnProportion = value;
		}

		/// <summary>
		/// Gets or sets the default proportion used for rows when not explicitly specified.
		/// </summary>
		[Browsable(false)]
		public Proportion DefaultRowProportion
		{
			get => _layout.DefaultRowProportion;
			set => _layout.DefaultRowProportion = value;
		}


		/// <summary>
		/// Gets the collection of column proportions that define how columns divide available space.
		/// </summary>
		[Browsable(false)]
		public ObservableCollection<Proportion> ColumnsProportions => _layout.ColumnsProportions;

		/// <summary>
		/// Gets the collection of row proportions that define how rows divide available space.
		/// </summary>
		[Browsable(false)]
		public ObservableCollection<Proportion> RowsProportions => _layout.RowsProportions;


		/// <summary>
		/// Gets or sets the brush used to draw the background of selected rows, columns, or cells.
		/// </summary>
		[Category("Appearance")]
		public IBrush SelectionBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used to draw the background of rows, columns, or cells being hovered over.
		/// </summary>
		[Category("Appearance")]
		public IBrush SelectionHoverBackground { get; set; }

		/// <summary>
		/// Gets or sets the selection mode for the grid (rows, columns, cells, or none).
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(GridSelectionMode.None)]
		public GridSelectionMode GridSelectionMode { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the hover index can be null when the mouse is outside the grid.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		public bool HoverIndexCanBeNull { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether nothing can be selected by clicking an already-selected item.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool CanSelectNothing { get; set; }

		/// <summary>
		/// Gets the X coordinates of the vertical grid lines.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public List<int> GridLinesX => _layout.GridLinesX;

		/// <summary>
		/// Gets the Y coordinates of the horizontal grid lines.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public List<int> GridLinesY => _layout.GridLinesY;

		/// <summary>
		/// Gets the widths of each column in pixels.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public List<int> ColWidths => _layout.ColWidths;

		/// <summary>
		/// Gets the heights of each row in pixels.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public List<int> RowHeights => _layout.RowHeights;

		/// <summary>
		/// Gets the X coordinates of each cell's left edge.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public List<int> CellLocationsX => _layout.CellLocationsX;

		/// <summary>
		/// Gets the Y coordinates of each cell's top edge.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public List<int> CellLocationsY => _layout.CellLocationsY;

		/// <summary>
		/// Gets or sets the index of the row currently being hovered over, or null if no row is hovered.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int? HoverRowIndex
		{
			get
			{
				return _hoverRowIndex;
			}

			set
			{
				if (value == _hoverRowIndex)
				{
					return;
				}

				_hoverRowIndex = value;

				var ev = HoverIndexChanged;
				if (ev != null)
				{
					ev(this, new MyraEventArgs(InputEventType.HoverIndexChanged));
				}
			}
		}

		/// <summary>
		/// Gets or sets the index of the column currently being hovered over, or null if no column is hovered.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int? HoverColumnIndex
		{
			get
			{
				return _hoverColumnIndex;
			}

			set
			{
				if (value == _hoverColumnIndex)
				{
					return;
				}

				_hoverColumnIndex = value;

				var ev = HoverIndexChanged;
				if (ev != null)
				{
					ev(this, new MyraEventArgs(InputEventType.HoverIndexChanged));
				}
			}
		}

		/// <summary>
		/// Gets or sets the index of the currently selected row, or null if no row is selected.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int? SelectedRowIndex
		{
			get { return _selectedRowIndex; }

			set
			{
				if (value == _selectedRowIndex)
				{
					return;
				}

				_selectedRowIndex = value;

				var ev = SelectedIndexChanged;
				if (ev != null)
				{
					ev(this, new MyraEventArgs(InputEventType.SelectedIndexChanged));
				}
			}
		}

		/// <summary>
		/// Gets or sets the index of the currently selected column, or null if no column is selected.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int? SelectedColumnIndex
		{
			get { return _selectedColumnIndex; }

			set
			{
				if (value == _selectedColumnIndex)
				{
					return;
				}

				_selectedColumnIndex = value;

				var ev = SelectedIndexChanged;
				if (ev != null)
				{
					ev(this, new MyraEventArgs(InputEventType.SelectedIndexChanged));
				}
			}
		}

		/// <summary>
		/// Occurs when the selected row or column changes.
		/// </summary>
		public event MyraEventHandler SelectedIndexChanged = null;

		/// <summary>
		/// Occurs when the hovered row or column changes.
		/// </summary>
		public event MyraEventHandler HoverIndexChanged = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="Grid"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply.</param>
		public Grid(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName)
		{
			ChildrenLayout = _layout;
			_layout.ColumnsProportions.CollectionChanged += OnProportionsChanged;
			_layout.RowsProportions.CollectionChanged += OnProportionsChanged;

			ShowGridLines = false;
			GridLinesColor = Color.White;
			HoverIndexCanBeNull = true;
			CanSelectNothing = false;

			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Grid"/> class.
		/// </summary>
		public Grid(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		/// <summary>
		/// Gets the width of the column at the specified index.
		/// </summary>
		/// <param name="index">The column index.</param>
		/// <returns>The column width in pixels.</returns>
		public int GetColumnWidth(int index) => _layout.GetColumnWidth(index);

		/// <summary>
		/// Gets the height of the row at the specified index.
		/// </summary>
		/// <param name="index">The row index.</param>
		/// <returns>The row height in pixels.</returns>
		public int GetRowHeight(int index) => _layout.GetRowHeight(index);

		/// <summary>
		/// Gets the X coordinate of the cell at the specified column.
		/// </summary>
		/// <param name="col">The column index.</param>
		/// <returns>The X coordinate in pixels.</returns>
		public int GetCellLocationX(int col) => _layout.GetCellLocationX(col);

		/// <summary>
		/// Gets the Y coordinate of the cell at the specified row.
		/// </summary>
		/// <param name="row">The row index.</param>
		/// <returns>The Y coordinate in pixels.</returns>
		public int GetCellLocationY(int row) => _layout.GetCellLocationY(row);

		/// <summary>
		/// Gets the bounds rectangle of the cell at the specified column and row.
		/// </summary>
		/// <param name="col">The column index.</param>
		/// <param name="row">The row index.</param>
		/// <returns>The cell bounds as a rectangle.</returns>
		public Rectangle GetCellRectangle(int col, int row) => _layout.GetCellRectangle(col, row);

		private void OnProportionsChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var i in args.NewItems)
				{
					((Proportion)i).Changed += OnProportionsChanged;
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var i in args.OldItems)
				{
					((Proportion)i).Changed -= OnProportionsChanged;
				}
			}

			HoverRowIndex = null;
			SelectedRowIndex = null;

			InvalidateMeasure();
		}

		private void OnProportionsChanged(object sender, MyraEventArgs args)
		{
			InvalidateMeasure();
		}

		/// <summary>
		/// Gets the proportion for the column at the specified index.
		/// </summary>
		/// <param name="col">The column index.</param>
		/// <returns>The column proportion.</returns>
		public Proportion GetColumnProportion(int col) => _layout.GetColumnProportion(col);

		/// <summary>
		/// Gets the proportion for the row at the specified index.
		/// </summary>
		/// <param name="row">The row index.</param>
		/// <returns>The row proportion, or the default if the index is out of range.</returns>
		public Proportion GetRowProportion(int row)
		{
			if (row < 0 || row >= RowsProportions.Count)
			{
				return DefaultRowProportion;
			}

			return RowsProportions[row];
		}

		/// <summary>
		/// Invalidates the child layout and clears grid-specific state.
		/// </summary>
		protected override void InvalidateChildren()
		{
			base.InvalidateChildren();

			HoverRowIndex = null;
			SelectedRowIndex = null;
		}

		private void RenderSelection(RenderContext context)
		{
			var bounds = ActualBounds;

			switch (GridSelectionMode)
			{
				case GridSelectionMode.None:
					break;
				case GridSelectionMode.Row:
					{
						if (HoverRowIndex != null && HoverRowIndex != SelectedRowIndex && SelectionHoverBackground != null)
						{
							var rect = new Rectangle(bounds.Left,
								CellLocationsY[HoverRowIndex.Value] + bounds.Top - RowSpacing / 2,
								bounds.Width,
								RowHeights[HoverRowIndex.Value] + RowSpacing);

							SelectionHoverBackground.Draw(context, rect);
						}

						if (SelectedRowIndex != null && SelectionBackground != null)
						{
							var rect = new Rectangle(bounds.Left,
								CellLocationsY[SelectedRowIndex.Value] + bounds.Top - RowSpacing / 2,
								bounds.Width,
								RowHeights[SelectedRowIndex.Value] + RowSpacing);

							SelectionBackground.Draw(context, rect);
						}
					}
					break;
				case GridSelectionMode.Column:
					{
						if (HoverColumnIndex != null && HoverColumnIndex != SelectedColumnIndex && SelectionHoverBackground != null)
						{
							var rect = new Rectangle(CellLocationsX[HoverColumnIndex.Value] + bounds.Left - ColumnSpacing / 2,
								bounds.Top,
								ColWidths[HoverColumnIndex.Value] + ColumnSpacing,
								bounds.Height);

							SelectionHoverBackground.Draw(context, rect);
						}

						if (SelectedColumnIndex != null && SelectionBackground != null)
						{
							var rect = new Rectangle(CellLocationsX[SelectedColumnIndex.Value] + bounds.Left - ColumnSpacing / 2,
								bounds.Top,
								ColWidths[SelectedColumnIndex.Value] + ColumnSpacing,
								bounds.Height);

							SelectionBackground.Draw(context, rect);
						}
					}
					break;
				case GridSelectionMode.Cell:
					{
						if (HoverRowIndex != null && HoverColumnIndex != null &&
							(HoverRowIndex != SelectedRowIndex || HoverColumnIndex != SelectedColumnIndex) &&
							SelectionHoverBackground != null)
						{
							var rect = new Rectangle(CellLocationsX[HoverColumnIndex.Value] + bounds.Left - ColumnSpacing / 2,
								CellLocationsY[HoverRowIndex.Value] + bounds.Top - RowSpacing / 2,
								ColWidths[HoverColumnIndex.Value] + ColumnSpacing,
								RowHeights[HoverRowIndex.Value] + RowSpacing);

							SelectionHoverBackground.Draw(context, rect);
						}

						if (SelectedRowIndex != null && SelectedColumnIndex != null && SelectionBackground != null)
						{
							var rect = new Rectangle(CellLocationsX[SelectedColumnIndex.Value] + bounds.Left - ColumnSpacing / 2,
								CellLocationsY[SelectedRowIndex.Value] + bounds.Top - RowSpacing / 2,
								ColWidths[SelectedColumnIndex.Value] + ColumnSpacing,
								RowHeights[SelectedRowIndex.Value] + RowSpacing);

							SelectionBackground.Draw(context, rect);
						}
					}
					break;
			}
		}

		/// <summary>
		/// Renders the grid content, selection, and grid lines.
		/// </summary>
		/// <param name="context">The render context used for drawing.</param>
		public override void InternalRender(RenderContext context)
		{
			var bounds = ActualBounds;

			RenderSelection(context);

			base.InternalRender(context);

			if (!ShowGridLines)
			{
				return;
			}

			int i;
			for (i = 0; i < GridLinesX.Count; ++i)
			{
				var x = GridLinesX[i] + bounds.Left;
				context.FillRectangle(new Rectangle(x, bounds.Top, 1, bounds.Height), GridLinesColor);
			}

			for (i = 0; i < GridLinesY.Count; ++i)
			{
				var y = GridLinesY[i] + bounds.Top;
				context.FillRectangle(new Rectangle(bounds.Left, y, bounds.Width, 1), GridLinesColor);
			}
		}

		private void UpdateHoverPosition(Point? position)
		{
			if (GridSelectionMode == GridSelectionMode.None)
			{
				return;
			}

			if (position == null)
			{
				if (HoverIndexCanBeNull)
				{
					HoverRowIndex = null;
					HoverColumnIndex = null;
				}
				return;
			}

			var pos = ToLocal(position.Value);
			var bounds = ActualBounds;
			if (GridSelectionMode == GridSelectionMode.Column || GridSelectionMode == GridSelectionMode.Cell)
			{
				var x = pos.X;
				for (var i = 0; i < CellLocationsX.Count; ++i)
				{
					var cx = CellLocationsX[i] + bounds.Left - ColumnSpacing / 2;
					if (x >= cx && x < cx + ColWidths[i] + ColumnSpacing / 2)
					{
						HoverColumnIndex = i;
						break;
					}
				}
			}

			if (GridSelectionMode == GridSelectionMode.Row || GridSelectionMode == GridSelectionMode.Cell)
			{
				var y = pos.Y;
				for (var i = 0; i < CellLocationsY.Count; ++i)
				{
					var cy = CellLocationsY[i] + bounds.Top - RowSpacing / 2;
					if (y >= cy && y < cy + RowHeights[i] + RowSpacing / 2)
					{
						HoverRowIndex = i;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Handles the event when the mouse leaves the grid.
		/// </summary>
		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			UpdateHoverPosition(null);
		}

		/// <summary>
		/// Handles the event when the mouse enters the grid.
		/// </summary>
		public override void OnMouseEntered()
		{
			base.OnMouseEntered();

			if (Desktop == null)
			{
				return;
			}

			UpdateHoverPosition(Desktop.MousePosition);
		}

		/// <summary>
		/// Handles the event when the mouse moves within the grid.
		/// </summary>
		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			if (Desktop == null)
			{
				return;
			}

			UpdateHoverPosition(Desktop.MousePosition);
		}

		/// <summary>
		/// Handles touch down events on the grid, including row/column/cell selection.
		/// </summary>
		public override void OnTouchDown()
		{
			base.OnTouchDown();

			if (Desktop == null)
			{
				return;
			}

			UpdateHoverPosition(Desktop.TouchPosition);

			if (HoverRowIndex != null)
			{
				if (SelectedRowIndex != HoverRowIndex)
				{
					SelectedRowIndex = HoverRowIndex;
				}
				else if (CanSelectNothing)
				{
					SelectedRowIndex = null;
				}
			}

			if (HoverColumnIndex != null)
			{
				if (SelectedColumnIndex != HoverColumnIndex)
				{
					SelectedColumnIndex = HoverColumnIndex;
				}
				else if (CanSelectNothing)
				{
					SelectedColumnIndex = null;
				}
			}
		}

		/// <summary>
		/// Copies the grid properties and column/row proportions from another grid.
		/// </summary>
		/// <param name="w">The source grid to copy from.</param>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var grid = (Grid)w;

			ShowGridLines = grid.ShowGridLines;
			GridLinesColor = grid.GridLinesColor;
			ColumnSpacing = grid.ColumnSpacing;
			RowSpacing = grid.RowSpacing;
			DefaultColumnProportion = grid.DefaultColumnProportion;
			DefaultRowProportion = grid.DefaultRowProportion;
			SelectionBackground = grid.SelectionBackground;
			SelectionHoverBackground = grid.SelectionHoverBackground;
			GridSelectionMode = grid.GridSelectionMode;
			HoverIndexCanBeNull = grid.HoverIndexCanBeNull;
			CanSelectNothing = grid.CanSelectNothing;

			foreach (var prop in grid.ColumnsProportions)
			{
				ColumnsProportions.Add(prop);
			}

			foreach (var prop in grid.RowsProportions)
			{
				RowsProportions.Add(prop);
			}
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.GridStyles;

		internal override bool CanStyleBeNull => true;

		/// <summary>
		/// Applies the specified widget style to this grid.
		/// </summary>
		/// <param name="style">The widget style to apply.</param>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var gridStyle = (GridStyle)style;

			ShowGridLines = gridStyle.ShowGridLines;
			GridLinesColor = gridStyle.GridLinesColor;
			ColumnSpacing = gridStyle.ColumnSpacing;
			RowSpacing = gridStyle.RowSpacing;
			SelectionBackground = gridStyle.SelectionBackground;
			SelectionHoverBackground = gridStyle.SelectionHoverBackground;
			GridSelectionMode = gridStyle.GridSelectionMode;
			HoverIndexCanBeNull = gridStyle.HoverIndexCanBeNull;
			CanSelectNothing = gridStyle.CanSelectNothing;
		}

		/// <summary>
		/// Gets the column index for the specified widget within the grid.
		/// </summary>
		/// <param name="widget">The widget to query.</param>
		/// <returns>The column index.</returns>
		public static int GetColumn(Widget widget) => ColumnProperty.GetValue(widget);

		/// <summary>
		/// Sets the column index for the specified widget within the grid.
		/// </summary>
		/// <param name="widget">The widget to modify.</param>
		/// <param name="value">The column index to set.</param>
		public static void SetColumn(Widget widget, int value) => ColumnProperty.SetValue(widget, value);

		/// <summary>
		/// Gets the row index for the specified widget within the grid.
		/// </summary>
		/// <param name="widget">The widget to query.</param>
		/// <returns>The row index.</returns>
		public static int GetRow(Widget widget) => RowProperty.GetValue(widget);

		/// <summary>
		/// Sets the row index for the specified widget within the grid.
		/// </summary>
		/// <param name="widget">The widget to modify.</param>
		/// <param name="value">The row index to set.</param>
		public static void SetRow(Widget widget, int value) => RowProperty.SetValue(widget, value);

		/// <summary>
		/// Gets the column span for the specified widget within the grid.
		/// </summary>
		/// <param name="widget">The widget to query.</param>
		/// <returns>The column span value.</returns>
		public static int GetColumnSpan(Widget widget) => ColumnSpanProperty.GetValue(widget);

		/// <summary>
		/// Sets the column span for the specified widget within the grid.
		/// </summary>
		/// <param name="widget">The widget to modify.</param>
		/// <param name="value">The column span to set.</param>
		public static void SetColumnSpan(Widget widget, int value) => ColumnSpanProperty.SetValue(widget, value);

		/// <summary>
		/// Gets the row span for the specified widget within the grid.
		/// </summary>
		/// <param name="widget">The widget to query.</param>
		/// <returns>The row span value.</returns>
		public static int GetRowSpan(Widget widget) => RowSpanProperty.GetValue(widget);

		/// <summary>
		/// Sets the row span for the specified widget within the grid.
		/// </summary>
		/// <param name="widget">The widget to modify.</param>
		/// <param name="value">The row span to set.</param>
		public static void SetRowSpan(Widget widget, int value) => RowSpanProperty.SetValue(widget, value);
	}
}