using Myra.Events;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Myra.Utility;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI.Data
{
	/// <summary>
	/// A tabular widget that displays data in rows and columns with support for column resizing,
	/// vertical scrolling, row selection, and customizable styling.
	/// </summary>
	public class DataGrid : Widget
	{
		private const int ColumnResizeHandleWidth = 4;
		private const int MinColumnWidth = 20;

		private readonly Grid _grid;
		private readonly SingleItemLayout<Grid> _layout;
		private Rectangle _verticalScrollbarFrame, _verticalScrollbarThumb;
		private int _startRow;
		private object[,] _data;
		private DataGridColumnBase[] _columns;
		private bool _hasIndexColumn = true;
		private int? _startBoundsPos;
		private int _thumbMaximumY;
		private int _indexColumnWidth = 50;
		private int? _resizingColumnIndex = null;
		private int _resizeStartX;
		private int _resizeOriginalWidth;

		/// <summary>
		/// Gets the number of data rows visible per page, based on the current layout size.
		/// </summary>
		public int RowsPerPage { get; private set; } = 1;

		/// <summary>
		/// Gets the total number of data rows loaded into the grid.
		/// </summary>
		public int TotalRows
		{
			get
			{
				if (_data == null)
				{
					return 0;
				}

				return _data.GetLength(0);
			}
		}


		/// <summary>
		/// Gets or sets the zero-based index of the first visible data row, used for vertical scrolling.
		/// </summary>
		public int StartRow
		{
			get => _startRow;

			set
			{
				if (value == _startRow)
				{
					return;
				}

				_startRow = value;
				RebuildGrid();
			}
		}

		/// <summary>
		/// Gets or sets the array of column definitions that define the grid's structure and data binding.
		/// Setting this property triggers a full rebuild of columns and the grid layout.
		/// </summary>
		public DataGridColumnBase[] Columns
		{
			get => _columns;

			set
			{
				if (value == null || value.Length == 0)
				{
					throw new ArgumentNullException(nameof(value));
				}

				for (var i = 0; i < value.Length; ++i)
				{
					var column = value[i];
					if (column == null)
					{
						throw new ArgumentNullException($"Column at index {i} is null.");
					}

					if (string.IsNullOrEmpty(column.Property))
					{
						throw new ArgumentNullException($"Column property must be defined. Index: {i}");
					}

					if (column.Width <= 0)
					{
						throw new ArgumentOutOfRangeException($"Column width must be a positive value. Index: {i}");
					}
				}

				_columns = value;
				RebuildColumns();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether grid lines are drawn between cells.
		/// </summary>
		[Category("Appearance")]
		public bool ShowGridLines
		{
			get => _grid.ShowGridLines;
			set => _grid.ShowGridLines = value;
		}

		/// <summary>
		/// Gets or sets the color of the grid lines.
		/// </summary>
		[Category("Appearance")]
		public Color GridLinesColor
		{
			get => _grid.GridLinesColor;
			set => _grid.GridLinesColor = value;
		}

		/// <summary>
		/// Gets or sets the spacing in pixels between grid columns.
		/// </summary>
		[Category("Appearance")]
		public int ColumnSpacing
		{
			get => _grid.ColumnSpacing;
			set => _grid.ColumnSpacing = value;
		}

		/// <summary>
		/// Gets or sets the spacing in pixels between grid rows.
		/// </summary>
		[Category("Appearance")]
		public int RowSpacing
		{
			get => _grid.RowSpacing;
			set => _grid.RowSpacing = value;
		}

		/// <summary>
		/// Gets or sets the brush used to draw the background of selected rows.
		/// </summary>
		[Category("Appearance")]
		public IBrush SelectionBackground
		{
			get => _grid.SelectionBackground;
			set => _grid.SelectionBackground = value;
		}

		/// <summary>
		/// Gets or sets the brush used to draw the background of hovered rows.
		/// </summary>
		[Category("Appearance")]
		public IBrush SelectionHoverBackground
		{
			get => _grid.SelectionHoverBackground;
			set => _grid.SelectionHoverBackground = value;
		}

		/// <summary>
		/// Gets or sets the selection mode for the grid (None, Row, Column, or Cell).
		/// </summary>
		[Category("Behavior")]
		public GridSelectionMode GridSelectionMode
		{
			get => _grid.GridSelectionMode;
			set => _grid.GridSelectionMode = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the hover index can be cleared when the pointer leaves the grid.
		/// </summary>
		[Category("Behavior")]
		public bool HoverIndexCanBeNull
		{
			get => _grid.HoverIndexCanBeNull;
			set => _grid.HoverIndexCanBeNull = value;

		}

		/// <summary>
		/// Gets or sets a value indicating whether clicking an already-selected row deselects it.
		/// </summary>
		[Category("Behavior")]
		public bool CanSelectNothing
		{
			get => _grid.CanSelectNothing;
			set => _grid.CanSelectNothing = value;
		}

		/// <summary>
		/// Gets or sets the image used for the vertical scrollbar track background.
		/// </summary>
		[Category("Appearance")]
		public IImage VerticalScrollBackground { get; set; }

		/// <summary>
		/// Gets or sets the image used for the vertical scrollbar thumb (knob).
		/// </summary>
		[Category("Appearance")]
		public IImage VerticalScrollKnob { get; set; }

		/// <summary>
		/// Gets or sets the number of rows to scroll per mouse wheel tick or touch drag step.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(10)]
		public int ScrollMultiplier { get; set; } = 10;

		/// <summary>
		/// Gets or sets the width in pixels of the optional row-index column displayed at the far left.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(50)]
		public int IndexColumnWidth
		{
			get => _indexColumnWidth;

			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				if (value == _indexColumnWidth)
				{
					return;
				}

				_indexColumnWidth = value;
				RebuildColumns();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether a row-number index column is displayed as the first column.
		/// </summary>
		[Category("Behavior")]
		public bool HasIndexColumn
		{
			get => _hasIndexColumn;
			set
			{
				if (value == _hasIndexColumn)
				{
					return;
				}

				_hasIndexColumn = value;
				RebuildColumns();
			}
		}

		/// <inheritdoc/>
		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}

		/// <inheritdoc/>
		[DefaultValue(VerticalAlignment.Stretch)]
		public override VerticalAlignment VerticalAlignment
		{
			get { return base.VerticalAlignment; }
			set { base.VerticalAlignment = value; }
		}

		/// <inheritdoc/>
		[DefaultValue(true)]
		public override bool ClipToBounds
		{
			get
			{
				return base.ClipToBounds;
			}
			set
			{
				base.ClipToBounds = value;
			}
		}

		/// <inheritdoc/>
		protected internal override bool AcceptsMouseWheel => VerticalScrollingOn;

		/// <inheritdoc/>
		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}

			internal set
			{
				if (Desktop != null)
				{
					Desktop.TouchMoved -= DesktopTouchMoved;
					Desktop.TouchUp -= DesktopTouchUp;
				}

				base.Desktop = value;

				if (Desktop != null)
				{
					Desktop.TouchMoved += DesktopTouchMoved;
					Desktop.TouchUp += DesktopTouchUp;
				}
			}
		}

		private bool VerticalScrollingOn => RowsPerPage < TotalRows;

		private int VerticalScrollbarWidth
		{
			get
			{
				var result = 0;
				if (VerticalScrollBackground != null)
				{
					result = VerticalScrollBackground.Size.X;
				}

				if (VerticalScrollKnob != null && VerticalScrollKnob.Size.X > result)
				{
					result = VerticalScrollKnob.Size.X;
				}

				return result;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGrid"/> class with the specified stylesheet and style name.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying styles.</param>
		/// <param name="styleName">The name of the style to apply from the stylesheet.</param>
		public DataGrid(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName)
		{
			ClipToBounds = true;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			_grid = new Grid
			{
				DefaultRowProportion = Proportion.Auto
			};

			_grid.HoverIndexChanged += OnGridHoverIndexChanged;
			_grid.SelectedIndexChanged += OnGridSelectedIndexChanged;

			_layout = new SingleItemLayout<Grid>(this)
			{
				Child = _grid
			};

			ChildrenLayout = _layout;

			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGrid"/> class using the current stylesheet.
		/// </summary>
		/// <param name="styleName">The name of the style to apply from the current stylesheet.</param>
		public DataGrid(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		private void RebuildColumns()
		{
			_grid.ColumnsProportions.Clear();

			if (HasIndexColumn)
			{
				_grid.ColumnsProportions.Add(new Proportion(ProportionType.Pixels, IndexColumnWidth));
			}

			for (var i = 0; i < _columns.Length; ++i)
			{
				_grid.ColumnsProportions.Add(new Proportion(ProportionType.Pixels, _columns[i].Width));
			}

			_grid.ColumnsProportions.Add(Proportion.Fill);

			RebuildGrid();
		}

		private bool BuildHeader()
		{
			if (Columns == null || Columns.Length == 0)
			{
				throw new Exception("Columns must be defined before building the DataGrid.");
			}

			var hasHeader = (from column in Columns where !string.IsNullOrEmpty(column.Header) select column).Any();
			if (!hasHeader)
			{
				return false;
			}


			if (HasIndexColumn)
			{
				var indexHeaderCell = new Label
				{
					Text = "#",
					ClipToBounds = true
				};
				Grid.SetRow(indexHeaderCell, 0);
				Grid.SetColumn(indexHeaderCell, 0);
				_grid.Widgets.Add(indexHeaderCell);
			}

			for (var i = 0; i < Columns.Length; ++i)
			{
				var column = Columns[i];
				if (string.IsNullOrEmpty(column.Header))
				{
					continue;
				}

				var headerCell = new Label
				{
					Text = column.Header,
					ClipToBounds = true
				};

				Grid.SetRow(headerCell, 0);
				Grid.SetColumn(headerCell, HasIndexColumn ? i + 1 : i);
				_grid.Widgets.Add(headerCell);
			}

			return true;
		}

		private void RebuildGrid()
		{
			try
			{
				SuppressInvalidateMeasure = true;

				_grid.Widgets.Clear();

				if (Columns == null || Columns.Length == 0)
				{
					return;
				}

				_grid.Width = null;

				var hasHeader = BuildHeader();

				var bounds = ActualBounds;
				var size = new Point(bounds.Width, bounds.Height);
				if (size.X == 0 || size.Y == 0 || _data == null)
				{
					return;
				}

				var count = 0;
				for (var row = StartRow; row < TotalRows; ++row)
				{
					var gridRow = hasHeader ? count + 1 : count;

					if (HasIndexColumn)
					{
						var cell = new Label
						{
							Text = row.ToString(),
							ClipToBounds = true
						};
						Grid.SetRow(cell, gridRow);
						Grid.SetColumn(cell, 0);
						_grid.Widgets.Add(cell);
					}

					for (var col = 0; col < Columns.Length; ++col)
					{
						var column = Columns[col];
						var value = _data[row, col];

						if (value == null)
						{
							continue;
						}

						var cell = new Label
						{
							Text = value.ToString(),
							ClipToBounds = true
						};
						Grid.SetRow(cell, gridRow);
						Grid.SetColumn(cell, HasIndexColumn ? col + 1 : col);
						_grid.Widgets.Add(cell);
					}

					var sz = _grid.Measure(size);
					if (sz.Y > size.Y)
					{
						break;
					}

					++count;
				}

				if (StartRow == 0)
				{
					RowsPerPage = count;
				}

				if (VerticalScrollingOn)
				{
					var vsWidth = VerticalScrollbarWidth;

					var bh = bounds.Height;
					_verticalScrollbarFrame = new Rectangle(
						bounds.Left + bounds.Width - vsWidth,
						bounds.Top,
						vsWidth,
						bh);

					var thumbHeight = Math.Max(VerticalScrollKnob.Size.Y, count * bh / TotalRows);
					_verticalScrollbarThumb = new Rectangle(
						bounds.Left + bounds.Width - vsWidth,
						bounds.Top,
						VerticalScrollKnob.Size.X,
						thumbHeight);

					_thumbMaximumY = bh - thumbHeight;
					if (_thumbMaximumY == 0)
					{
						_thumbMaximumY = 1;
					}

					_grid.Width = size.X - vsWidth;
				}
			}
			finally
			{
				SuppressInvalidateMeasure = false;
			}
		}

		/// <inheritdoc/>
		protected override void InternalArrange()
		{
			base.InternalArrange();

			RebuildGrid();
		}

		/// <summary>
		/// Populates the grid with rows by reading property values from each item in the collection.
		/// </summary>
		/// <param name="data">The collection of objects to display as rows.</param>
		public void Build(IEnumerable data)
		{
			if (Columns == null || Columns.Length == 0)
			{
				throw new Exception("Columns must be defined before building the DataGrid.");
			}

			var row = 0;
			foreach (var item in data)
			{
				++row;
			}

			_data = new object[row, Columns.Length];

			row = 0;
			foreach (var item in data)
			{
				var type = item.GetType();
				for (var col = 0; col < Columns.Length; ++col)
				{
					var column = Columns[col];
					if (string.IsNullOrEmpty(column.Property))
					{
						throw new Exception("Column property must be defined. Index: " + col);
					}

					var property = type.GetProperty(column.Property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					if (property == null)
					{
						throw new Exception($"Property not found: {column.Property} in type {type.FullName}");
					}

					var value = property.GetValue(item);
					if (value == null)
					{
						continue;
					}

					_data[row, col] = value;
				}

				++row;
			}

			RebuildGrid();
		}

		/// <inheritdoc/>
		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (!VerticalScrollingOn)
			{
				return;
			}

			if (VerticalScrollBackground != null)
			{
				VerticalScrollBackground.Draw(context, _verticalScrollbarFrame);
			}

			if (VerticalScrollKnob != null)
			{
				var r = _verticalScrollbarThumb;

				var bh = ActualBounds.Height;
				r.Y += StartRow * bh / TotalRows;
				VerticalScrollKnob.Draw(context, r);
			}
		}

		/// <inheritdoc/>
		public override void OnMouseWheel(float delta)
		{
			base.OnMouseWheel(delta);

			if (!VerticalScrollingOn)
			{
				return;
			}

			var step = ScrollMultiplier;
			if (delta > 0)
			{
				step = -step;
			}

			var newStartRow = StartRow + step;
			newStartRow = Mathematics.Clamp(newStartRow, 0, TotalRows - RowsPerPage);

			StartRow = newStartRow;
		}

		/// <inheritdoc/>
		public override void OnTouchDown()
		{
			base.OnTouchDown();

			if (Desktop == null)
			{
				return;
			}

			var touchPosition = ToLocal(Desktop.TouchPosition.Value);

			if (VerticalScrollingOn)
			{
				var bh = ActualBounds.Height;

				var r = _verticalScrollbarThumb;
				r.Y += (StartRow * bh / TotalRows);
				if (r.Contains(touchPosition))
				{
					_startBoundsPos = Desktop.TouchPosition.Value.Y;
				}
				else if (_verticalScrollbarFrame.Contains(touchPosition))
				{
					var fraction = (float)(touchPosition.Y - _verticalScrollbarFrame.Top) / bh;
					var targetRow = (int)(fraction * TotalRows);
					targetRow = Mathematics.Clamp(targetRow, 0, TotalRows - RowsPerPage);
					StartRow = targetRow;

					_startBoundsPos = Desktop.TouchPosition.Value.Y;
				}
			}

			var localPos = LocalTouchPosition;
			if (localPos != null && IsInHeaderRow(localPos.Value))
			{
				var boundaryIndex = GetColumnBoundaryIndex(localPos.Value);
				if (boundaryIndex != null)
				{
					_resizingColumnIndex = boundaryIndex.Value;
					_resizeStartX = Desktop.TouchPosition.Value.X;
					_resizeOriginalWidth = _grid.ColWidths[boundaryIndex.Value];
					Desktop.TouchMoved += DesktopColumnResizeMoved;
				}
			}
		}

		/// <inheritdoc/>
		public override void OnTouchUp()
		{
			base.OnTouchUp();

			_startBoundsPos = null;

			if (_resizingColumnIndex >= 0)
			{
				_resizingColumnIndex = -1;

				if (Desktop != null)
				{
					Desktop.TouchMoved -= DesktopColumnResizeMoved;
				}
			}
		}

		private void DesktopTouchMoved(object sender, MyraEventArgs args)
		{
			if (!_startBoundsPos.HasValue || Desktop == null || !VerticalScrollingOn)
			{
				return;
			}

			var touchPosition = Desktop.TouchPosition;
			var bh = ActualBounds.Height;

			var delta = (touchPosition.Value.Y - _startBoundsPos.Value) * TotalRows / bh;
			_startBoundsPos = touchPosition.Value.Y;

			var newStartRow = StartRow + delta;
			newStartRow = Mathematics.Clamp(newStartRow, 0, TotalRows - RowsPerPage);

			StartRow = newStartRow;
		}

		private void DesktopTouchUp(object sender, MyraEventArgs args)
		{
			_startBoundsPos = null;
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.DataGridStyles;

		/// <inheritdoc/>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var dataGridStyle = (DataGridStyle)style;

			ShowGridLines = dataGridStyle.ShowGridLines;
			GridLinesColor = dataGridStyle.GridLinesColor;
			ColumnSpacing = dataGridStyle.ColumnSpacing;
			RowSpacing = dataGridStyle.RowSpacing;
			SelectionBackground = dataGridStyle.SelectionBackground;
			SelectionHoverBackground = dataGridStyle.SelectionHoverBackground;
			GridSelectionMode = dataGridStyle.GridSelectionMode;
			HoverIndexCanBeNull = dataGridStyle.HoverIndexCanBeNull;
			CanSelectNothing = dataGridStyle.CanSelectNothing;
			VerticalScrollBackground = dataGridStyle.VerticalScrollBackground;
			VerticalScrollKnob = dataGridStyle.VerticalScrollKnob;
		}

		private bool IsInHeaderRow(Point localPos)
		{
			if (_grid.RowHeights.Count == 0)
			{
				return false;
			}

			return localPos.Y >= 0 && localPos.Y < _grid.RowHeights[0];
		}

		private bool HasHeader => Columns != null &&
			(from column in Columns where !string.IsNullOrEmpty(column.Header) select column).Any();

		private void OnGridHoverIndexChanged(object sender, MyraEventArgs args)
		{
			if (HasHeader && _grid.HoverRowIndex == 0)
			{
				_grid.HoverRowIndex = null;
			}
		}

		private void OnGridSelectedIndexChanged(object sender, MyraEventArgs args)
		{
			if (HasHeader && _grid.SelectedRowIndex == 0)
			{
				_grid.SelectedRowIndex = null;
			}
		}

		private int? GetColumnBoundaryIndex(Point localPos)
		{
			var bounds = ActualBounds;
			var offsetX = bounds.Left;

			for (var i = 1; i < _grid.CellLocationsX.Count; ++i)
			{
				var boundaryX = offsetX + _grid.CellLocationsX[i] - _grid.ColumnSpacing / 2;
				if (Math.Abs(localPos.X - boundaryX) < ColumnResizeHandleWidth)
				{
					return i - 1;
				}
			}

			return null;
		}

		/// <inheritdoc/>
		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			if (Desktop == null)
			{
				return;
			}

			var localPos = LocalMousePosition;
			if (localPos == null)
			{
				return;
			}

			if (_resizingColumnIndex >= 0)
			{
				MyraEnvironment.MouseCursorType = MouseCursorType.SizeWE;
			}
			else if (IsInHeaderRow(localPos.Value) && GetColumnBoundaryIndex(localPos.Value) >= 0)
			{
				MyraEnvironment.MouseCursorType = MouseCursorType.SizeWE;
			}
			else
			{
				MyraEnvironment.MouseCursorType = MouseCursor ?? MyraEnvironment.DefaultMouseCursorType;
			}
		}

		/// <inheritdoc/>
		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			_resizingColumnIndex = null;
		}

		private void DesktopColumnResizeMoved(object sender, MyraEventArgs args)
		{
			if (_resizingColumnIndex < 0 || Desktop == null)
			{
				return;
			}

			var deltaX = Desktop.TouchPosition.Value.X - _resizeStartX;
			var newWidth = _resizeOriginalWidth + deltaX;
			if (newWidth < MinColumnWidth)
			{
				newWidth = MinColumnWidth;
			}

			if (_resizingColumnIndex != null)
			{
				_grid.ColumnsProportions[_resizingColumnIndex.Value].Value = newWidth;
			}
		}
	}
}
