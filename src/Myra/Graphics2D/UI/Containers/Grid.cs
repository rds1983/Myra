using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

	public class Grid : Container
	{
		public static readonly AttachedPropertyInfo<int> ColumnProperty = 
			AttachedPropertiesRegistry.Create(typeof(Grid), "Column", 0, AttachedPropertyOption.AffectsArrange);
		public static readonly AttachedPropertyInfo<int> RowProperty = 
			AttachedPropertiesRegistry.Create(typeof(Grid), "Row", 0, AttachedPropertyOption.AffectsArrange);
		public static readonly AttachedPropertyInfo<int> ColumnSpanProperty = 
			AttachedPropertiesRegistry.Create(typeof(Grid), "ColumnSpan", 1, AttachedPropertyOption.AffectsArrange);
		public static readonly AttachedPropertyInfo<int> RowSpanProperty = 
			AttachedPropertiesRegistry.Create(typeof(Grid), "RowSpan", 1, AttachedPropertyOption.AffectsArrange);

		private readonly GridLayout _layout = new GridLayout();

		private int? _hoverRowIndex = null;
		private int? _hoverColumnIndex = null;
		private int? _selectedRowIndex = null;
		private int? _selectedColumnIndex = null;

		[Category("Debug")]
		[DefaultValue(false)]
		public bool ShowGridLines { get; set; }

		[Category("Debug")]
		[DefaultValue("White")]
		public Color GridLinesColor { get; set; }

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

		[Browsable(false)]
		public Proportion DefaultColumnProportion
		{
			get => _layout.DefaultColumnProportion;
			set => _layout.DefaultColumnProportion = value;
		}

		[Browsable(false)]
		public Proportion DefaultRowProportion
		{
			get => _layout.DefaultRowProportion;
			set => _layout.DefaultRowProportion = value;
		}


		[Browsable(false)]
		public ObservableCollection<Proportion> ColumnsProportions => _layout.ColumnsProportions;

		[Browsable(false)]
		public ObservableCollection<Proportion> RowsProportions => _layout.RowsProportions;


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
		public List<int> GridLinesX => _layout.GridLinesX;

		[Browsable(false)]
		[XmlIgnore]
		public List<int> GridLinesY => _layout.GridLinesY;

		[Browsable(false)]
		[XmlIgnore]
		public List<int> ColWidths => _layout.ColWidths;

		[Browsable(false)]
		[XmlIgnore]
		public List<int> RowHeights => _layout.RowHeights;

		[Browsable(false)]
		[XmlIgnore]
		public List<int> CellLocationsX => _layout.CellLocationsX;

		[Browsable(false)]
		[XmlIgnore]
		public List<int> CellLocationsY => _layout.CellLocationsY;

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
			ChildrenLayout = _layout;
			_layout.ColumnsProportions.CollectionChanged += OnProportionsChanged;
			_layout.RowsProportions.CollectionChanged += OnProportionsChanged;

			ShowGridLines = false;
			GridLinesColor = Color.White;
			HoverIndexCanBeNull = true;
			CanSelectNothing = false;
		}

		public int GetColumnWidth(int index) => _layout.GetColumnWidth(index);
		
		public int GetRowHeight(int index) => _layout.GetRowHeight(index);

		public int GetCellLocationX(int col) => _layout.GetCellLocationX(col);

		public int GetCellLocationY(int row) => _layout.GetCellLocationY(row);

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

		private void OnProportionsChanged(object sender, EventArgs args)
		{
			InvalidateMeasure();
		}

		public Proportion GetColumnProportion(int col) => _layout.GetColumnProportion(col);

		public Proportion GetRowProportion(int row)
		{
			if (row < 0 || row >= RowsProportions.Count)
			{
				return DefaultRowProportion;
			}

			return RowsProportions[row];
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

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			UpdateHoverPosition(null);
		}

		public override void OnMouseEntered()
		{
			base.OnMouseEntered();

			if (Desktop == null)
			{
				return;
			}

			UpdateHoverPosition(Desktop.MousePosition);
		}

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			if (Desktop == null)
			{
				return;
			}

			UpdateHoverPosition(Desktop.MousePosition);
		}

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

			foreach(var prop in grid.ColumnsProportions)
			{
				ColumnsProportions.Add(prop);
			}

			foreach(var prop in grid.RowsProportions)
			{
				RowsProportions.Add(prop);
			}
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