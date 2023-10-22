using System.Collections.ObjectModel;
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
	public abstract class StackPanel: MultipleItemsContainerBase
	{
		private readonly GridLayout _layout = new GridLayout();
		private int _spacing;
		private bool _dirty = true;

		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation { get; }

		[Category("Debug")]
		[DefaultValue(false)]
		public bool ShowGridLines { get; set; }

		[Category("Debug")]
		[DefaultValue("White")]
		public Color GridLinesColor { get; set; }

		[Category("Layout")]
		[DefaultValue(0)]
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

		[Browsable(false)]
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

		[Browsable(false)]
		public ObservableCollection<Proportion> Proportions
		{
			get => Orientation == Orientation.Horizontal ? _layout.ColumnsProportions : _layout.RowsProportions;
		}

		protected StackPanel()
		{
			DefaultProportion = Proportion.StackPanelDefault;
			Widgets.CollectionChanged += Widgets_CollectionChanged;
		}

		private void UpdateGrid()
		{
			if (!_dirty)
			{
				return;
			}

			var index = 0;
			foreach (var widget in Widgets)
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

			_dirty = false;
		}

		private void Widgets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			_dirty = true;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			UpdateGrid();

			return _layout.Measure(Widgets, availableSize);
		}

		protected override void InternalArrange()
		{
			UpdateGrid();

			_layout.Arrange(Widgets, ActualBounds);
		}
	}
}
