using Myra.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	public class GridLayout: ILayout
	{
		private readonly List<int> _measureColWidths = new List<int>();
		private readonly List<int> _measureRowHeights = new List<int>();
		private readonly List<Widget> _visibleWidgets = new List<Widget>();
		private List<Widget>[,] _widgetsByGridPosition;
		private Point _actualSize;

		public int ColumnSpacing { get; set; }

		public int RowSpacing { get; set; }

		public Proportion DefaultColumnProportion { get; set; } = Proportion.GridDefault;

		public Proportion DefaultRowProportion { get; set; } = Proportion.GridDefault;

		public ObservableCollection<Proportion> ColumnsProportions { get; } = new ObservableCollection<Proportion>();

		public ObservableCollection<Proportion> RowsProportions { get; } = new ObservableCollection<Proportion>();
		
		public List<int> GridLinesX { get; } = new List<int>();
		public List<int> GridLinesY { get; } = new List<int>();
		public List<int> ColWidths { get; } = new List<int>();
		public List<int> RowHeights { get; } = new List<int>();
		public List<int> CellLocationsX { get; } = new List<int>();
		public List<int> CellLocationsY { get; } = new List<int>();


		public GridLayout()
		{
		}

		public int GetColumnWidth(int index)
		{
			if (ColWidths == null || index < 0 || index >= ColWidths.Count)
			{
				return 0;
			}

			return ColWidths[index];
		}

		public int GetRowHeight(int index)
		{
			if (RowHeights == null || index < 0 || index >= RowHeights.Count)
			{
				return 0;
			}

			return RowHeights[index];
		}

		public int GetCellLocationX(int col)
		{
			if (col < 0 || col >= CellLocationsX.Count)
			{
				return 0;
			}

			return CellLocationsX[col];
		}

		public int GetCellLocationY(int row)
		{
			if (row < 0 || row >= CellLocationsY.Count)
			{
				return 0;
			}

			return CellLocationsY[row];
		}

		public Rectangle GetCellRectangle(int col, int row)
		{
			if (col < 0 || col >= CellLocationsX.Count ||
				row < 0 || row >= CellLocationsY.Count)
			{
				return Rectangle.Empty;
			}

			return new Rectangle(CellLocationsX[col], CellLocationsY[row],
				ColWidths[col], RowHeights[row]);
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
			return new Point(Grid.GetColumn(child), Grid.GetRow(child));
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

		public Point Measure(IEnumerable<Widget> widgets, Point availableSize)
		{
			var rows = 0;
			var columns = 0;

			_visibleWidgets.Clear();
			foreach (var child in widgets)
			{
				if (child.Visible)
				{
					_visibleWidgets.Add(child);

					var gridPosition = GetActualGridPosition(child);
					var c = gridPosition.X + Math.Max(Grid.GetColumnSpan(child), 1);
					if (c > columns)
					{
						columns = c;
					}

					var r = gridPosition.Y + Math.Max(Grid.GetRowSpan(child), 1);
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
				_widgetsByGridPosition[Grid.GetRow(widget), Grid.GetColumn(widget)].Add(widget);
			}

			availableSize.X -= (_measureColWidths.Count - 1) * ColumnSpacing;
			availableSize.Y -= (_measureRowHeights.Count - 1) * RowSpacing;

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

					var widgetsAtPosition = _widgetsByGridPosition[row, col];
					foreach (var widget in widgetsAtPosition)
					{
						var gridPosition = GetActualGridPosition(widget);

						var measuredSize = Mathematics.PointZero;
						if (rowProportion.Type != ProportionType.Pixels ||
							colProportion.Type != ProportionType.Pixels)
						{
							measuredSize = widget.Measure(availableSize);
						}

						if (Grid.GetColumnSpan(widget) != 1)
						{
							measuredSize.X = 0;
						}

						if (Grid.GetRowSpan(widget) != 1)
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
					result.X += ColumnSpacing;
				}
			}

			for (i = 0; i < _measureRowHeights.Count; ++i)
			{
				var h = _measureRowHeights[i];
				result.Y += h;

				if (i < _measureRowHeights.Count - 1)
				{
					result.Y += RowSpacing;
				}
			}

			return result;
		}

		public void Arrange(IEnumerable<Widget> widgets, Rectangle bounds)
		{
			Measure(widgets, bounds.Size());

			ColWidths.Clear();
			for (var i = 0; i < _measureColWidths.Count; ++i)
			{
				ColWidths.Add(_measureColWidths[i]);
			}

			RowHeights.Clear();
			for (var i = 0; i < _measureRowHeights.Count; ++i)
			{
				RowHeights.Add(_measureRowHeights[i]);
			}

			// Partition available space
			int row, col;

			// Dynamic widths
			// First run: calculate available width
			var availableWidth = (float)bounds.Width;
			availableWidth -= (ColWidths.Count - 1) * ColumnSpacing;

			var totalPart = 0.0f;
			for (col = 0; col < ColWidths.Count; ++col)
			{
				var colWidth = ColWidths[col];
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
				for (col = 0; col < ColWidths.Count; ++col)
				{
					var prop = GetColumnProportion(col);
					if (prop.Type == ProportionType.Part)
					{
						ColWidths[col] = (int)(prop.Value * availableWidth / totalPart);
						tookSpace += ColWidths[col];
					}
				}

				availableWidth -= tookSpace;
			}

			// Update part fill widths
			for (col = 0; col < ColWidths.Count; ++col)
			{
				var prop = GetColumnProportion(col);
				if (prop.Type == ProportionType.Fill)
				{
					ColWidths[col] = (int)availableWidth;
					break;
				}
			}

			// Same with row heights
			var availableHeight = (float)bounds.Height;
			availableHeight -= (RowHeights.Count - 1) * RowSpacing;

			totalPart = 0.0f;
			for (col = 0; col < RowHeights.Count; ++col)
			{
				var colHeight = RowHeights[col];
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
				for (row = 0; row < RowHeights.Count; ++row)
				{
					var prop = GetRowProportion(row);
					if (prop.Type != ProportionType.Part) continue;

					RowHeights[row] = (int)(prop.Value * availableHeight / totalPart);
					tookSpace += RowHeights[row];
				}

				availableHeight -= tookSpace;
			}

			// Update part fill heights
			for (row = 0; row < RowHeights.Count; ++row)
			{
				var prop = GetRowProportion(row);
				if (prop.Type == ProportionType.Fill)
				{
					RowHeights[row] = (int)availableHeight;
					break;
				}
			}

			_actualSize = Mathematics.PointZero;
			GridLinesX.Clear();
			CellLocationsX.Clear();

			var p = Mathematics.PointZero;

			for (var i = 0; i < ColWidths.Count; ++i)
			{
				CellLocationsX.Add(p.X);
				var w = ColWidths[i];
				p.X += w;

				if (i < ColWidths.Count - 1)
				{
					GridLinesX.Add(p.X + ColumnSpacing / 2);
				}

				p.X += ColumnSpacing;

				_actualSize.X += ColWidths[i];
			}

			GridLinesY.Clear();
			CellLocationsY.Clear();

			for (var i = 0; i < RowHeights.Count; ++i)
			{
				CellLocationsY.Add(p.Y);
				var h = RowHeights[i];
				p.Y += h;

				if (i < RowHeights.Count - 1)
				{
					GridLinesY.Add(p.Y + RowSpacing / 2);
				}

				p.Y += RowSpacing;

				_actualSize.Y += RowHeights[i];
			}

			foreach (var control in _visibleWidgets)
			{
				LayoutControl(control, bounds);
			}
		}

		private void LayoutControl(Widget control, Rectangle bounds)
		{
			var gridPosition = GetActualGridPosition(control);
			var col = gridPosition.X;
			var row = gridPosition.Y;

			var cellSize = Mathematics.PointZero;

			for (var i = col; i < col + Grid.GetColumnSpan(control); ++i)
			{
				cellSize.X += ColWidths[i];

				if (i < col + Grid.GetColumnSpan(control) - 1)
				{
					cellSize.X += ColumnSpacing;
				}
			}

			for (var i = row; i < row + Grid.GetRowSpan(control); ++i)
			{
				cellSize.Y += RowHeights[i];

				if (i < row + Grid.GetRowSpan(control) - 1)
				{
					cellSize.Y += RowSpacing;
				}
			}

			var rect = new Rectangle(bounds.Left + CellLocationsX[col], bounds.Top + CellLocationsY[row], cellSize.X, cellSize.Y);

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
	}
}
