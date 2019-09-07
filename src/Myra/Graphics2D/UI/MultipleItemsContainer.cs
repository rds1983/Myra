using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Myra.Attributes;

namespace Myra.Graphics2D.UI
{
	public abstract class MultipleItemsContainer : Container
	{
		protected readonly ObservableCollection<Widget> _widgets = new ObservableCollection<Widget>();

		public override int ChildrenCount
		{
			get
			{
				return _widgets.Count;
			}
		}

		[HiddenInEditor]
		[Content]
		public virtual ObservableCollection<Widget> Widgets
		{
			get { return _widgets; }
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

		public MultipleItemsContainer()
		{
			_widgets.CollectionChanged += WidgetsOnCollectionChanged;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		public override Widget GetChild(int index)
		{
			return _widgets[index];
		}

		private void WidgetsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (Widget w in args.NewItems)
				{
					w.Desktop = Desktop;
					w.Parent = this;
					w.MeasureChanged += ChildOnMeasureChanged;
					w.VisibleChanged += ChildOnVisibleChanged;
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (Widget w in args.OldItems)
				{
					w.Desktop = null;
					w.Parent = null;
					w.VisibleChanged -= ChildOnVisibleChanged;
					w.MeasureChanged -= ChildOnMeasureChanged;
				}
			}

			InvalidateMeasure();
			InvalidateChildren();
		}

		private void ChildOnMeasureChanged(object sender, EventArgs eventArgs)
		{
			var widget = (Widget) sender;
			if (widget.Visible)
			{
				InvalidateMeasure();
			}
		}

		private void ChildOnVisibleChanged(object sender, EventArgs eventArgs)
		{
			InvalidateMeasure();
		}

		public override void RemoveChild(Widget widget)
		{
			_widgets.Remove(widget);
		}
	}
}