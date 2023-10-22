using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.MML;

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
	public enum GridSelectionMode
	{
		None,
		Row,
		Column,
		Cell
	}

	public class Grid : MultipleItemsContainerBase
	{
		public static AttachedPropertyInfo<int> ColumnProperty = AttachedPropertiesRegistry.Create(typeof(Grid), "Column", 0);
		public static AttachedPropertyInfo<int> RowProperty = AttachedPropertiesRegistry.Create(typeof(Grid), "Row", 0);
		public static AttachedPropertyInfo<int> ColumnSpanProperty = AttachedPropertiesRegistry.Create(typeof(Grid), "ColumnSpan", 1);
		public static AttachedPropertyInfo<int> RowSpanProperty = AttachedPropertiesRegistry.Create(typeof(Grid), "RowSpan", 1);

		private int _columnSpacing;
		private int _rowSpacing;
		private readonly ObservableCollection<Proportion> _columnsProportions = new ObservableCollection<Proportion>();
		private readonly ObservableCollection<Proportion> _rowsProportions = new ObservableCollection<Proportion>();
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

		[Category("Appearance")]
		public IBrush SelectionBackground { get; set; }

		[Category("Appearance")]
		public IBrush SelectionHoverBackground { get; set; }

		[Category("Behavior")]
		[DefaultValue(GridSelectionMode.None)]
		public GridSelectionMode GridSelectionMode { get; set; }

		[Category("Behavior")]
		[DefaultValue(true)]
		public bool HoverIndexCanBeNull { get; set; }

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
			HoverIndexCanBeNull = true;
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

		protected override void OnChildAdded(Widget w)
		{
			base.OnChildAdded(w);


		}

		protected override void OnChildRemoved(Widget w)
		{
			base.OnChildRemoved(w);
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
			return new Point(GetColumn(child), GetRow(child));
		}

		private void LayoutProcessFixedPart()
		{
			int i = 0, size = 0;

			// First run - find maximum size
			for (i = 0; i < _measureColWidths.Count; ++i)
			{
				var prop = GetColumnProportion(i);
				if (prop.Type != ProportionType.Part)
				{
					continue;
				}

				if (_measureColWidths[i] > size)
				{
					size = _measureColWidths[i];
				}
			}

			// Second run - update
			for (i = 0; i < _measureColWidths.Count; ++i)
			{
				var prop = GetColumnProportion(i);
				if (prop.Type != ProportionType.Part)
				{
					continue;
				}

				_measureColWidths[i] = (int)(size * prop.Value);
			}

			size = 0;

			// First run - find maximum size
			for (i = 0; i < _measureRowHeights.Count; ++i)
			{
				var prop = GetRowProportion(i);
				if (prop.Type != ProportionType.Part)
				{
					continue;
				}

				if (_measureRowHeights[i] > size)
				{
					size = _measureRowHeights[i];
				}
			}

			// Second run - update
			for (i = 0; i < _measureRowHeights.Count; ++i)
			{
				var prop = GetRowProportion(i);
				if (prop.Type != ProportionType.Part)
				{
					continue;
				}

				_measureRowHeights[i] = (int)(size * prop.Value);
			}
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
					var c = gridPosition.X + Math.Max(GetColumnSpan(child), 1);
					if (c > columns)
					{
						columns = c;
					}

					var r = gridPosition.Y + Math.Max(GetRowSpan(child), 1);
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
				_widgetsByGridPosition[GetRow(widget), GetColumn(widget)].Add(widget);
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

						var measuredSize = Mathematics.PointZero;
						if (rowProportion.Type != ProportionType.Pixels ||
							colProportion.Type != ProportionType.Pixels)
						{
							measuredSize = widget.Measure(availableSize);
						}

						if (GetColumnSpan(widget) != 1)
						{
							measuredSize.X = 0;
						}

						if (GetRowSpan(widget) != 1)
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

			// #181: All Part proportions must have maximum size
			LayoutProcessFixedPart();

			var result = Mathematics.PointZero;
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

		public override void InternalArrange()
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

			_actualSize = Mathematics.PointZero;
			_gridLinesX.Clear();
			_cellLocationsX.Clear();

			var p = Mathematics.PointZero;

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

			var cellSize = Mathematics.PointZero;

			for (var i = col; i < col + GetColumnSpan(control); ++i)
			{
				cellSize.X += _colWidths[i];

				if (i < col + GetColumnSpan(control) - 1)
				{
					cellSize.X += _columnSpacing;
				}
			}

			for (var i = row; i < row + GetRowSpan(control); ++i)
			{
				cellSize.Y += _rowHeights[i];

				if (i < row + GetRowSpan(control) - 1)
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

			if (rect.Bottom > bounds.Bottom)
			{
				rect.Height = bounds.Bottom - rect.Y;
			}

			if (rect.Height < 0)
			{
				rect.Height = 0;
			}

			control.Arrange(rect);
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
								_rowHeights[HoverRowIndex.Value] + RowSpacing);

							SelectionHoverBackground.Draw(context, rect);
						}

						if (SelectedRowIndex != null && SelectionBackground != null)
						{
							var rect = new Rectangle(bounds.Left,
								_cellLocationsY[SelectedRowIndex.Value] + bounds.Top - RowSpacing / 2,
								bounds.Width,
								_rowHeights[SelectedRowIndex.Value] + RowSpacing);

							SelectionBackground.Draw(context, rect);
						}
					}
					break;
				case GridSelectionMode.Column:
					{
						if (HoverColumnIndex != null && HoverColumnIndex != SelectedColumnIndex && SelectionHoverBackground != null)
						{
							var rect = new Rectangle(_cellLocationsX[HoverColumnIndex.Value] + bounds.Left - ColumnSpacing / 2,
								bounds.Top,
								_colWidths[HoverColumnIndex.Value] + ColumnSpacing,
								bounds.Height);

							SelectionHoverBackground.Draw(context, rect);
						}

						if (SelectedColumnIndex != null && SelectionBackground != null)
						{
							var rect = new Rectangle(_cellLocationsX[SelectedColumnIndex.Value] + bounds.Left - ColumnSpacing / 2,
								bounds.Top,
								_colWidths[SelectedColumnIndex.Value] + ColumnSpacing,
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
							var rect = new Rectangle(_cellLocationsX[HoverColumnIndex.Value] + bounds.Left - ColumnSpacing / 2,
								_cellLocationsY[HoverRowIndex.Value] + bounds.Top - RowSpacing / 2,
								_colWidths[HoverColumnIndex.Value] + ColumnSpacing,
								_rowHeights[HoverRowIndex.Value] + RowSpacing);

							SelectionHoverBackground.Draw(context, rect);
						}

						if (SelectedRowIndex != null && SelectedColumnIndex != null && SelectionBackground != null)
						{
							var rect = new Rectangle(_cellLocationsX[SelectedColumnIndex.Value] + bounds.Left - ColumnSpacing / 2,
								_cellLocationsY[SelectedRowIndex.Value] + bounds.Top - RowSpacing / 2,
								_colWidths[SelectedColumnIndex.Value] + ColumnSpacing,
								_rowHeights[SelectedRowIndex.Value] + RowSpacing);

							SelectionBackground.Draw(context, rect);
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
				for (var i = 0; i < _cellLocationsX.Count; ++i)
				{
					var cx = _cellLocationsX[i] + bounds.Left - ColumnSpacing / 2;
					if (x >= cx && x < cx + _colWidths[i] + ColumnSpacing / 2)
					{
						HoverColumnIndex = i;
						break;
					}
				}
			}

			if (GridSelectionMode == GridSelectionMode.Row || GridSelectionMode == GridSelectionMode.Cell)
			{
				var y = pos.Y;
				for (var i = 0; i < _cellLocationsY.Count; ++i)
				{
					var cy = _cellLocationsY[i] + bounds.Top - RowSpacing / 2;
					if (y >= cy && y < cy + _rowHeights[i] + RowSpacing / 2)
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

			if (Desktop.MousePosition != Desktop.PreviousMousePosition)
			{
				UpdateHoverPosition(Desktop.MousePosition);
			}
		}

		public override bool OnTouchDown()
		{
			base.OnTouchDown();

			if (Desktop == null)
			{
				return false;
			}

			UpdateHoverPosition(Desktop.TouchPosition);

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

			return (SelectedRowIndex != null && SelectedColumnIndex != null);
		}

		public static int GetColumn(Widget widget) => ColumnProperty.GetValue(widget);
		public static void SetColumn(Widget widget, int value) => ColumnProperty.SetValue(widget, value);
		public static int GetRow(Widget widget) => RowProperty.GetValue(widget);
		public static void SetRow(Widget widget, int value) => RowProperty.SetValue(widget, value);
		public static int GetColumnSpan(Widget widget) => ColumnSpanProperty.GetValue(widget);
		public static void SetColumnSpan(Widget widget, int value) => ColumnSpanProperty.SetValue(widget, value);
		public static int GetRowSpan(Widget widget) => RowSpanProperty.GetValue(widget);
		public static void SetRowSpan(Widget widget, int value) => RowSpanProperty.SetValue(widget, value);
	}
}