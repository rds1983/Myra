using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Myra.Utility;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public enum GridSelectionMode
	{
		None,
		Row,
		Column,
		Cell
	}

	public class Grid : MultipleItemsContainerBase
	{
		private int _columnSpacing;
		private int _rowSpacing;
		private readonly ObservableCollection<Proportion> _columnsProportions = new ObservableCollection<Proportion>();
		private readonly ObservableCollection<Proportion> _rowsProportions = new ObservableCollection<Proportion>();
		private float? _totalColumnsPart, _totalRowsPart;
		private readonly List<int> _cellLocationsX = new List<int>();
		private readonly List<int> _cellLocationsY = new List<int>();
		private readonly List<int> _gridLinesX = new List<int>();
		private readonly List<int> _gridLinesY = new List<int>();
		private Point _actualSize;

		private readonly List<int> _measureColWidths = new List<int>();
		private readonly List<int> _measureRowHeights = new List<int>();
		private readonly List<Widget> _visibleWidgets = new List<Widget>();
		private readonly List<int> _colWidths = new List<int>();
		private readonly List<int> _rowHeights = new List<int>();
		private int? _hoverRowIndex = null;
		private int? _hoverColumnIndex = null;
		private int? _selectedRowIndex = null;
		private int? _selectedColumnIndex = null;
		private List<Widget>[,] _widgetsByGridPosition;

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool ShowGridLines { get; set; }

		[Category("Behavior")]
		[DefaultValue("White")]
		public Color GridLinesColor { get; set; }

		[Category("Grid")]
		[DefaultValue(0)]
		public int ColumnSpacing
		{
			get { return _columnSpacing; }
			set
			{
				if (value == _columnSpacing)
				{
					return;
				}

				_columnSpacing = value;
				InvalidateMeasure();
			}
		}

		[Category("Grid")]
		[DefaultValue(0)]
		public int RowSpacing
		{
			get { return _rowSpacing; }
			set
			{
				if (value == _rowSpacing)
				{
					return;
				}

				_rowSpacing = value;
				InvalidateMeasure();
			}
		}

		[Browsable(false)]
		public Proportion DefaultColumnProportion { get; set; } = Proportion.GridDefault;

		[Browsable(false)]
		public Proportion DefaultRowProportion { get; set; } = Proportion.GridDefault;

		[Browsable(false)]
		public ObservableCollection<Proportion> ColumnsProportions
		{
			get { return _columnsProportions; }
		}

		[Browsable(false)]
		public ObservableCollection<Proportion> RowsProportions
		{
			get { return _rowsProportions; }
		}

		[Category("Grid")]
		[DefaultValue(null)]
		public float? TotalRowsPart
		{
			get { return _totalRowsPart; }

			set
			{
				if (value == _totalRowsPart)
				{
					return;
				}

				_totalRowsPart = value;
				InvalidateLayout();
			}
		}

		[Category("Grid")]
		[DefaultValue(null)]
		public float? TotalColumnsPart
		{
			get { return _totalColumnsPart; }

			set
			{
				if (value == _totalColumnsPart)
				{
					return;
				}

				_totalColumnsPart = value;
				InvalidateLayout();
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public IRenderable SelectionBackground { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public IRenderable SelectionHoverBackground { get; set; }

		[Category("Behavior")]
		[DefaultValue(GridSelectionMode.None)]
		public GridSelectionMode GridSelectionMode { get; set; }

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool CanSelectNothing { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public List<int> GridLinesX
		{
			get
			{
				return _gridLinesX;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public List<int> GridLinesY
		{
			get
			{
				return _gridLinesY;
			}
		}

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
					ev(this, EventArgs.Empty);
				}
			}
		}

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
					ev(this, EventArgs.Empty);
				}
			}
		}

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
					ev(this, EventArgs.Empty);
				}
			}
		}

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
					ev(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler SelectedIndexChanged = null;
		public event EventHandler HoverIndexChanged = null;

		public Grid()
		{
			_columnsProportions.CollectionChanged += OnProportionsChanged;
			_rowsProportions.CollectionChanged += OnProportionsChanged;

			ShowGridLines = false;
			GridLinesColor = Color.White;
			CanSelectNothing = false;
		}

		public int GetColumnWidth(int index)
		{
			if (_colWidths == null || index < 0 || index >= _colWidths.Count)
			{
				return 0;
			}

			return _colWidths[index];
		}

		public int GetRowHeight(int index)
		{
			if (_rowHeights == null || index < 0 || index >= _rowHeights.Count)
			{
				return 0;
			}

			return _rowHeights[index];
		}

		public int GetCellLocationX(int col)
		{
			if (col < 0 || col >= _cellLocationsX.Count)
			{
				return 0;
			}

			return _cellLocationsX[col];
		}

		public int GetCellLocationY(int row)
		{
			if (row < 0 || row >= _cellLocationsY.Count)
			{
				return 0;
			}

			return _cellLocationsY[row];
		}

		public Rectangle GetCellRectangle(int col, int row)
		{
			if (col < 0 || col >= _cellLocationsX.Count ||
				row < 0 || row >= _cellLocationsY.Count)
			{
				return Rectangle.Empty;
			}

			return new Rectangle(_cellLocationsX[col], _cellLocationsY[row],
				_colWidths[col], _rowHeights[row]);
		}

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

		private void OnProportionsChanged(object sender, EventArgs args)
		{
			InvalidateMeasure();
		}

		public Proportion GetColumnProportion(int col)
		{
			if (col < 0 || col >= ColumnsProportions.Count)
			{
				return DefaultColumnProportion;
			}

			return ColumnsProportions[col];
		}

		public Proportion GetRowProportion(int row)
		{
			if (row < 0 || row >= RowsProportions.Count)
			{
				return DefaultRowProportion;
			}

			return RowsProportions[row];
		}

		private Point GetActualGridPosition(Widget child)
		{
			return new Point(child.GridColumn, child.GridRow);
		}

		private Point LayoutProcessFixed(Point availableSize)
		{
			var rows = 0;
			var columns = 0;

			_visibleWidgets.Clear();
			foreach (var child in Widgets)
			{
				if (child.Visible)
				{
					_visibleWidgets.Add(child);

					var gridPosition = GetActualGridPosition(child);
					var c = gridPosition.X + Math.Max(child.GridColumnSpan, 1);
					if (c > columns)
					{
						columns = c;
					}

					var r = gridPosition.Y + Math.Max(child.GridRowSpan, 1);
					if (r > rows)
					{
						rows = r;
					}
				}
			}

			if (ColumnsProportions.Count > columns)
			{
				columns = ColumnsProportions.Count;
			}

			if (RowsProportions.Count > rows)
			{
				rows = RowsProportions.Count;
			}

			_measureColWidths.Clear();
			int i;
			for (i = 0; i < columns; ++i)
			{
				_measureColWidths.Add(0);
			}

			_measureRowHeights.Clear();
			for (i = 0; i < rows; ++i)
			{
				_measureRowHeights.Add(0);
			}

			// Put all visible widget into 2d array
			if (_widgetsByGridPosition == null ||
				_widgetsByGridPosition.GetLength(0) < rows ||
				_widgetsByGridPosition.GetLength(1) < columns)
			{
				_widgetsByGridPosition = new List<Widget>[rows, columns];
			}

			for (var row = 0; row < rows; ++row)
			{
				for (var col = 0; col < columns; ++col)
				{
					if (_widgetsByGridPosition[row, col] == null)
					{
						_widgetsByGridPosition[row, col] = new List<Widget>();
					}

					_widgetsByGridPosition[row, col].Clear();
				}
			}

			foreach (var widget in _visibleWidgets)
			{
				_widgetsByGridPosition[widget.GridRow, widget.GridColumn].Add(widget);
			}

			availableSize.X -= (_measureColWidths.Count - 1) * _columnSpacing;
			availableSize.Y -= (_measureRowHeights.Count - 1) * _rowSpacing;

			for (var row = 0; row < rows; ++row)
			{
				for (var col = 0; col < columns; ++col)
				{
					var rowProportion = GetRowProportion(row);
					var colProportion = GetColumnProportion(col);

					if (colProportion.Type == ProportionType.Pixels)
					{
						_measureColWidths[col] = (int)colProportion.Value;
					}

					if (rowProportion.Type == ProportionType.Pixels)
					{
						_measureRowHeights[row] = (int)rowProportion.Value;
					}

					var widgets = _widgetsByGridPosition[row, col];
					foreach (var widget in widgets)
					{
						var gridPosition = GetActualGridPosition(widget);

						var measuredSize = Point.Zero;
						if (rowProportion.Type != ProportionType.Pixels ||
							colProportion.Type != ProportionType.Pixels)
						{
							measuredSize = widget.Measure(availableSize);
						}

						if (widget.GridColumnSpan != 1)
						{
							measuredSize.X = 0;
						}

						if (widget.GridRowSpan != 1)
						{
							measuredSize.Y = 0;
						}

						if (measuredSize.X > _measureColWidths[col] && colProportion.Type != ProportionType.Pixels)
						{
							_measureColWidths[col] = measuredSize.X;
						}

						if (measuredSize.Y > _measureRowHeights[row] && rowProportion.Type != ProportionType.Pixels)
						{
							_measureRowHeights[row] = measuredSize.Y;
						}
					}
				}
			}

			var result = Point.Zero;

			for (i = 0; i < _measureColWidths.Count; ++i)
			{
				var w = _measureColWidths[i];

				result.X += w;
				if (i < _measureColWidths.Count - 1)
				{
					result.X += _columnSpacing;
				}
			}

			for (i = 0; i < _measureRowHeights.Count; ++i)
			{
				var h = _measureRowHeights[i];
				result.Y += h;

				if (i < _measureRowHeights.Count - 1)
				{
					result.Y += _rowSpacing;
				}
			}

			return result;
		}

		public override void Arrange()
		{
			var bounds = ActualBounds;
			LayoutProcessFixed(bounds.Size());

			_colWidths.Clear();
			for (var i = 0; i < _measureColWidths.Count; ++i)
			{
				_colWidths.Add(_measureColWidths[i]);
			}

			_rowHeights.Clear();
			for (var i = 0; i < _measureRowHeights.Count; ++i)
			{
				_rowHeights.Add(_measureRowHeights[i]);
			}

			// Partition available space
			int row, col;

			// Dynamic widths
			// First run: calculate available width
			var availableWidth = (float)bounds.Width;
			availableWidth -= (_colWidths.Count - 1) * _columnSpacing;

			var totalPart = 0.0f;
			for (col = 0; col < _colWidths.Count; ++col)
			{
				var colWidth = _colWidths[col];
				var prop = GetColumnProportion(col);
				if (prop.Type == ProportionType.Auto || prop.Type == ProportionType.Pixels)
				{
					// Fixed width
					availableWidth -= colWidth;
				}
				else
				{
					totalPart += prop.Value;
				}
			}

			if (TotalColumnsPart.HasValue)
			{
				totalPart = TotalColumnsPart.Value;
			}

			if (!totalPart.IsZero())
			{
				// Second run update dynamic widths
				var tookSpace = 0.0f;
				for (col = 0; col < _colWidths.Count; ++col)
				{
					var prop = GetColumnProportion(col);
					if (prop.Type == ProportionType.Part)
					{
						_colWidths[col] = (int)(prop.Value * availableWidth / totalPart);
						tookSpace += _colWidths[col];
					}
				}

				availableWidth -= tookSpace;
			}

			// Update part fill widths
			for (col = 0; col < _colWidths.Count; ++col)
			{
				var prop = GetColumnProportion(col);
				if (prop.Type == ProportionType.Fill)
				{
					_colWidths[col] = (int)availableWidth;
					break;
				}
			}

			// Same with row heights
			var availableHeight = (float)bounds.Height;
			availableHeight -= (_rowHeights.Count - 1) * _rowSpacing;

			totalPart = 0.0f;
			for (col = 0; col < _rowHeights.Count; ++col)
			{
				var colHeight = _rowHeights[col];
				var prop = GetRowProportion(col);
				if (prop.Type == ProportionType.Auto || prop.Type == ProportionType.Pixels)
				{
					// Fixed height
					availableHeight -= colHeight;
				}
				else
				{
					totalPart += prop.Value;
				}
			}

			if (TotalRowsPart.HasValue)
			{
				totalPart = TotalRowsPart.Value;
			}

			if (!totalPart.IsZero())
			{
				var tookSpace = 0.0f;
				for (row = 0; row < _rowHeights.Count; ++row)
				{
					var prop = GetRowProportion(row);
					if (prop.Type != ProportionType.Part) continue;

					_rowHeights[row] = (int)(prop.Value * availableHeight / totalPart);
					tookSpace += _rowHeights[row];
				}

				availableHeight -= tookSpace;
			}

			// Update part fill heights
			for (row = 0; row < _rowHeights.Count; ++row)
			{
				var prop = GetRowProportion(row);
				if (prop.Type == ProportionType.Fill)
				{
					_rowHeights[row] = (int)availableHeight;
					break;
				}
			}

			_actualSize = Point.Zero;
			_gridLinesX.Clear();
			_cellLocationsX.Clear();

			var p = Point.Zero;

			for (var i = 0; i < _colWidths.Count; ++i)
			{
				_cellLocationsX.Add(p.X);
				var w = _colWidths[i];
				p.X += w;

				if (i < _colWidths.Count - 1)
				{
					_gridLinesX.Add(p.X + _columnSpacing / 2);
				}

				p.X += _columnSpacing;

				_actualSize.X += _colWidths[i];
			}

			_gridLinesY.Clear();
			_cellLocationsY.Clear();

			for (var i = 0; i < _rowHeights.Count; ++i)
			{
				_cellLocationsY.Add(p.Y);
				var h = _rowHeights[i];
				p.Y += h;

				if (i < _rowHeights.Count - 1)
				{
					_gridLinesY.Add(p.Y + _rowSpacing / 2);
				}

				p.Y += _rowSpacing;

				_actualSize.Y += _rowHeights[i];
			}

			foreach (var control in _visibleWidgets)
			{
				LayoutControl(control);
			}
		}

		private void LayoutControl(Widget control)
		{
			var gridPosition = GetActualGridPosition(control);
			var col = gridPosition.X;
			var row = gridPosition.Y;

			var cellSize = Point.Zero;

			for (var i = col; i < col + control.GridColumnSpan; ++i)
			{
				cellSize.X += _colWidths[i];

				if (i < col + control.GridColumnSpan - 1)
				{
					cellSize.X += _columnSpacing;
				}
			}

			for (var i = row; i < row + control.GridRowSpan; ++i)
			{
				cellSize.Y += _rowHeights[i];

				if (i < row + control.GridRowSpan - 1)
				{
					cellSize.Y += _rowSpacing;
				}
			}

			var bounds = ActualBounds;

			var rect = new Rectangle(bounds.Left + _cellLocationsX[col], bounds.Top + _cellLocationsY[row], cellSize.X, cellSize.Y);

			if (rect.Right > bounds.Right)
			{
				rect.Width = bounds.Right - rect.X;
			}

			if (rect.Width < 0)
			{
				rect.Width = 0;
			}

			if (rect.Width > bounds.Width)
			{
				rect.Width = bounds.Width;
			}

			if (rect.Bottom > bounds.Bottom)
			{
				rect.Height = bounds.Bottom - rect.Y;
			}

			if (rect.Height < 0)
			{
				rect.Height = 0;
			}

			if (rect.Height > bounds.Height)
			{
				rect.Height = bounds.Height;
			}

			control.Layout(rect);
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
								_cellLocationsY[HoverRowIndex.Value] + bounds.Top - RowSpacing / 2,
								bounds.Width,
								_rowHeights[HoverRowIndex.Value] + RowSpacing / 2);

							context.Draw(SelectionHoverBackground, rect);
						}

						if (SelectedRowIndex != null && SelectionBackground != null)
						{
							var rect = new Rectangle(bounds.Left,
								_cellLocationsY[SelectedRowIndex.Value] + bounds.Top - RowSpacing / 2,
								bounds.Width,
								_rowHeights[SelectedRowIndex.Value] + RowSpacing / 2);

							context.Draw(SelectionBackground, rect);
						}
					}
					break;
				case GridSelectionMode.Column:
					{
						if (HoverColumnIndex != null && HoverColumnIndex != SelectedColumnIndex && SelectionHoverBackground != null)
						{
							var rect = new Rectangle(_cellLocationsX[HoverColumnIndex.Value] + bounds.Left - ColumnSpacing / 2,
								bounds.Top,
								_colWidths[HoverColumnIndex.Value] + ColumnSpacing / 2,
								bounds.Height);

							context.Draw(SelectionHoverBackground, rect);
						}

						if (SelectedColumnIndex != null && SelectionBackground != null)
						{
							var rect = new Rectangle(_cellLocationsX[SelectedColumnIndex.Value] + bounds.Left - ColumnSpacing / 2,
								bounds.Top,
								_colWidths[SelectedColumnIndex.Value] + ColumnSpacing / 2,
								bounds.Height);

							context.Draw(SelectionBackground, rect);
						}
					}
					break;
				case GridSelectionMode.Cell:
					{
						if (HoverRowIndex != null && HoverColumnIndex != null &&
							(HoverRowIndex != SelectedRowIndex || HoverColumnIndex != SelectedColumnIndex) &&
							SelectionHoverBackground != null)
						{
							var rect = new Rectangle(_cellLocationsX[HoverColumnIndex.Value] + bounds.Left - ColumnSpacing / 2,
								_cellLocationsY[HoverRowIndex.Value] + bounds.Top - RowSpacing / 2,
								_colWidths[HoverColumnIndex.Value] + ColumnSpacing / 2,
								_rowHeights[HoverRowIndex.Value] + RowSpacing / 2);

							context.Draw(SelectionHoverBackground, rect);
						}

						if (SelectedRowIndex != null && SelectedColumnIndex != null && SelectionBackground != null)
						{
							var rect = new Rectangle(_cellLocationsX[SelectedColumnIndex.Value] + bounds.Left - ColumnSpacing / 2,
								_cellLocationsY[SelectedRowIndex.Value] + bounds.Top - RowSpacing / 2,
								_colWidths[SelectedColumnIndex.Value] + ColumnSpacing / 2,
								_rowHeights[SelectedRowIndex.Value] + RowSpacing / 2);

							context.Draw(SelectionBackground, rect);
						}
					}
					break;
			}
		}

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
			for (i = 0; i < _gridLinesX.Count; ++i)
			{
				var x = _gridLinesX[i] + bounds.Left;
				context.FillRectangle(new Rectangle(x, bounds.Top, 1, bounds.Height), GridLinesColor);
			}

			for (i = 0; i < _gridLinesY.Count; ++i)
			{
				var y = _gridLinesY[i] + bounds.Top;
				context.FillRectangle(new Rectangle(bounds.Left, y, bounds.Width, 1), GridLinesColor);
			}
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			return LayoutProcessFixed(availableSize);
		}

		private void UpdateHoverPosition(Point? position)
		{
			if (GridSelectionMode == GridSelectionMode.None)
			{
				return;
			}

			if (position == null)
			{
				HoverRowIndex = null;
				HoverColumnIndex = null;
				return;
			}

			var bounds = ActualBounds;

			if (GridSelectionMode == GridSelectionMode.Column || GridSelectionMode == GridSelectionMode.Cell)
			{
				var x = position.Value.X;
				for (var i = 0; i < _cellLocationsX.Count; ++i)
				{
					var cx = _cellLocationsX[i] + bounds.Left;
					if (x >= cx && x < cx + _colWidths[i])
					{
						HoverColumnIndex = i;
						break;
					}
				}
			}

			if (GridSelectionMode == GridSelectionMode.Row || GridSelectionMode == GridSelectionMode.Cell)
			{
				var y = position.Value.Y;
				for (var i = 0; i < _cellLocationsY.Count; ++i)
				{
					var cy = _cellLocationsY[i] + bounds.Top;
					if (y >= cy && y < cy + _rowHeights[i])
					{
						HoverRowIndex = i;
						break;
					}
				}
			}
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			UpdateHoverPosition(null);
		}

		public override void OnMouseEntered()
		{
			base.OnMouseEntered();

			UpdateHoverPosition(Desktop.MousePosition);
		}

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			UpdateHoverPosition(Desktop.MousePosition);
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			if (Desktop != null)
			{
				UpdateHoverPosition(Desktop.TouchPosition);
			}

			if (HoverRowIndex != null)
			{
				if (SelectedRowIndex != HoverRowIndex)
				{
					SelectedRowIndex = HoverRowIndex;
				} else if (CanSelectNothing)
				{
					SelectedRowIndex = null;
				}
			}

			if (HoverColumnIndex != null)
			{
				if (SelectedColumnIndex != HoverColumnIndex)
				{
					SelectedColumnIndex = HoverColumnIndex;
				} else if (CanSelectNothing)
				{
					SelectedColumnIndex = null;
				}
			}
		}
	}
}