using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Myra.Attributes;

namespace Myra.Graphics2D.UI
{
	public abstract class MultipleItemsContainer : Container
	{
		private bool _widgetsDirty = true;
		private readonly List<Widget> _widgetsCopy = new List<Widget>();
		private readonly List<Widget> _reverseWidgetsCopy = new List<Widget>();
		protected readonly ObservableCollection<Widget> _widgets = new ObservableCollection<Widget>();

		public override IEnumerable<Widget> Children
		{
			get
			{
				// We return copy of our collection
				// To prevent exception when someone modifies the collection during the iteration
				UpdateWidgets();

				return _widgetsCopy;
			}
		}

		public override IEnumerable<Widget> ReverseChildren
		{
			get
			{
				UpdateWidgets();

				return _reverseWidgetsCopy;
			}
		}

		public override int ChildCount
		{
			get { return _widgets.Count; }
		}

		[HiddenInEditor]
		public virtual IList<Widget> Widgets
		{
			get { return _widgets; }
		}

		public MultipleItemsContainer()
		{
			_widgets.CollectionChanged += WidgetsOnCollectionChanged;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		private void UpdateWidgets()
		{
			if (_widgetsDirty)
			{
				_widgetsCopy.Clear();
				_widgetsCopy.AddRange(_widgets);

				_reverseWidgetsCopy.Clear();
				_reverseWidgetsCopy.AddRange(_widgets);
				_reverseWidgetsCopy.Reverse();

				_widgetsDirty = false;
			}
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

			_widgetsDirty = true;
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