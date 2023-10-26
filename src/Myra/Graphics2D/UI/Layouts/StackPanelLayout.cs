using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	public class StackPanelLayout : ILayout
	{
		private readonly GridLayout _layout = new GridLayout();
		private int _spacing;

		public Orientation Orientation { get; private set; }

		public int Spacing
		{
			get => _spacing;
			set
			{
				_spacing = value;
				if (Orientation == Orientation.Horizontal)
				{
					_layout.ColumnSpacing = value;
				}
				else
				{
					_layout.RowSpacing = value;
				}
			}
		}

		public Proportion DefaultProportion
		{
			get => Orientation == Orientation.Horizontal ? _layout.DefaultColumnProportion : _layout.DefaultRowProportion;
			set
			{
				if (Orientation == Orientation.Horizontal)
				{
					_layout.DefaultColumnProportion = value;
				}
				else
				{
					_layout.DefaultRowProportion = value;
				}
			}
		}

		public ObservableCollection<Proportion> Proportions
		{
			get => Orientation == Orientation.Horizontal ? _layout.ColumnsProportions : _layout.RowsProportions;
		}

		public List<int> GridLinesX => _layout.GridLinesX;

		public List<int> GridLinesY => _layout.GridLinesY;

		public StackPanelLayout(Orientation orientation)
		{
			Orientation = orientation;
		}

		private void UpdatePositions(IEnumerable<Widget> widgets)
		{
			var index = 0;
			foreach (var widget in widgets)
			{
				if (Orientation == Orientation.Horizontal)
				{
					Grid.SetColumn(widget, index);
				}
				else
				{
					Grid.SetRow(widget, index);
				}

				++index;
			}
		}

		public Point Measure(IEnumerable<Widget> widgets, Point availableSize)
		{
			UpdatePositions(widgets);
			return _layout.Measure(widgets, availableSize);
		}

		public void Arrange(IEnumerable<Widget> widgets, Rectangle bounds)
		{
			UpdatePositions(widgets);
			_layout.Arrange(widgets, bounds);
		}
	}
}