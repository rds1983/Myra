using Myra.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	public abstract class Box: SingleItemContainer<Grid>, IMultipleItemsContainer
	{
		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation
		{
			get;
		}

		[Browsable(false)]
		[Content]
		public ObservableCollection<Widget> Widgets => InternalChild.Widgets;

		[Category("Box")]
		[DefaultValue(false)]
		public bool ShowGridLines
		{
			get => InternalChild.ShowGridLines;
			set => InternalChild.ShowGridLines = value;
		}

		[Category("Box")]
		[DefaultValue(0)]
		public int Spacing
		{
			get => Orientation == Orientation.Horizontal ? InternalChild.ColumnSpacing : InternalChild.RowSpacing;
			set
			{
				if (Orientation == Orientation.Horizontal)
				{
					InternalChild.ColumnSpacing = value;
				}
				else
				{
					InternalChild.RowSpacing = value;
				}
			}
		}

		[Browsable(false)]
		public ObservableCollection<Proportion> Proportions
		{
			get => Orientation == Orientation.Horizontal ? InternalChild.ColumnsProportions : InternalChild.RowsProportions;
		}

		protected Box()
		{
			InternalChild = new Grid();
			if (Orientation == Orientation.Horizontal)
			{
				InternalChild.DefaultColumnProportion = Proportion.Auto;
			}
			else
			{
				InternalChild.DefaultRowProportion = Proportion.Auto;
			}

			Widgets.CollectionChanged += Widgets_CollectionChanged;
		}

		private void Widgets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			var index = 0;
			foreach (var widget in Widgets)
			{
				if (Orientation == Orientation.Horizontal)
				{
					widget.GridColumn = index;
				}
				else
				{
					widget.GridRow = index;
				}

				++index;
			}
		}
	}
}
