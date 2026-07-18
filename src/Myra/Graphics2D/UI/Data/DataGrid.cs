using Microsoft.Xna.Framework;
using Myra.Events;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Myra.Graphics2D.UI.Data
{
	public class DataGrid : Widget
	{
		private readonly Grid _grid;
		private readonly SingleItemLayout<Grid> _layout;
		private bool _verticalScrollingOn;
		private Rectangle _verticalScrollbarFrame, _verticalScrollbarThumb;
		private int _startRow;
		private object[,] _data;
		private DataGridColumnBase[] _columns;
		private bool _hasIndexColumn = true;
		private int? _startBoundsPos;
		private int _thumbMaximumY;
		private int _indexColumnWidth = 50;

		public int RowsPerPage { get; private set; } = 1;

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

		[Category("Appearance")]
		public IImage VerticalScrollBackground { get; set; }

		[Category("Appearance")]
		public IImage VerticalScrollKnob { get; set; }

		[Category("Appearance")]
		[DefaultValue(10)]
		public int ScrollMultiplier { get; set; } = 10;

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

		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}

		[DefaultValue(VerticalAlignment.Stretch)]
		public override VerticalAlignment VerticalAlignment
		{
			get { return base.VerticalAlignment; }
			set { base.VerticalAlignment = value; }
		}

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

		protected internal override bool AcceptsMouseWheel => _verticalScrollingOn;

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

		public DataGrid(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName)
		{
			ClipToBounds = true;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			_grid = new Grid
			{
				DefaultRowProportion = Proportion.Auto
			};

			_layout = new SingleItemLayout<Grid>(this)
			{
				Child = _grid
			};

			ChildrenLayout = _layout;

			SetStyle(stylesheet, styleName);
		}

		public DataGrid(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		private void RebuildColumns()
		{
			// Set columns proportions based on the defined columns
			_grid.ColumnsProportions.Clear();

			if (HasIndexColumn)
			{
				_grid.ColumnsProportions.Add(new Proportion(ProportionType.Pixels, IndexColumnWidth));
			}

			for (var i = 0; i < _columns.Length; ++i)
			{
				_grid.ColumnsProportions.Add(new Proportion(ProportionType.Pixels, _columns[i].Width));
			}

			// Add a fill proportion for the last column to take up remaining space
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

				// Clear widgets
				_grid.Widgets.Clear();

				if (Columns == null || Columns.Length == 0)
				{
					return;
				}

				_grid.Width = null;

				// Build header
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
						_verticalScrollingOn = true;
						break;
					}

					++count;
				}

				RowsPerPage = count;
			}
			finally
			{
				SuppressInvalidateMeasure = false;
			}
		}

		protected override void InternalArrange()
		{
			base.InternalArrange();

			RebuildGrid();
		}


		public void Build(object data)
		{
			if (Columns == null || Columns.Length == 0)
			{
				throw new Exception("Columns must be defined before building the DataGrid.");
			}

			// Add data rows
			var asEnumerable = data as IEnumerable;
			if (asEnumerable == null)
			{
				throw new Exception("Data must be an IEnumerable.");
			}

			// Build data
			var row = 0;
			foreach (var item in asEnumerable)
			{
				++row;
			}

			_data = new object[row, Columns.Length];

			row = 0;
			foreach (var item in asEnumerable)
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

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (!_verticalScrollingOn)
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
				r.Y += (StartRow * bh / TotalRows);
				VerticalScrollKnob.Draw(context, r);
			}
		}

		public override void OnMouseWheel(float delta)
		{
			base.OnMouseWheel(delta);

			if (!_verticalScrollingOn)
			{
				return;
			}

			var step = ScrollMultiplier;
			if (delta > 0)
			{
				step = -step;
			}

			var newStartRow = StartRow + step;
			newStartRow = MathHelper.Clamp(newStartRow, 0, TotalRows - RowsPerPage);

			StartRow = newStartRow;
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			if (Desktop == null || !_verticalScrollingOn)
			{
				return;
			}

			var touchPosition = ToLocal(Desktop.TouchPosition.Value);
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
				targetRow = MathHelper.Clamp(targetRow, 0, TotalRows - RowsPerPage);
				StartRow = targetRow;

				_startBoundsPos = Desktop.TouchPosition.Value.Y;
			}
		}

		public override void OnTouchUp()
		{
			base.OnTouchUp();

			_startBoundsPos = null;
		}

		private void DesktopTouchMoved(object sender, MyraEventArgs args)
		{
			if (!_startBoundsPos.HasValue || Desktop == null || !_verticalScrollingOn)
			{
				return;
			}

			var touchPosition = Desktop.TouchPosition;
			var bh = ActualBounds.Height;

			var delta = (touchPosition.Value.Y - _startBoundsPos.Value) * TotalRows / bh;
			_startBoundsPos = touchPosition.Value.Y;

			var newStartRow = StartRow + (int)delta;
			newStartRow = MathHelper.Clamp(newStartRow, 0, TotalRows - RowsPerPage);

			StartRow = newStartRow;
		}

		private void DesktopTouchUp(object sender, MyraEventArgs args)
		{
			_startBoundsPos = null;
		}

		public override bool InputFallsThrough(Point localPos)
		{
			if (Background != null)
			{
				return false;
			}

			if (_verticalScrollingOn && _verticalScrollbarFrame.Contains(localPos))
			{
				return false;
			}

			return true;
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.DataGridStyles;

		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var dataGridStyle = (DataGridStyle)style;
			_grid.ApplyOnlyGridStyle(dataGridStyle);

			VerticalScrollBackground = dataGridStyle.VerticalScrollBackground;
			VerticalScrollKnob = dataGridStyle.VerticalScrollKnob;
		}
	}
}
