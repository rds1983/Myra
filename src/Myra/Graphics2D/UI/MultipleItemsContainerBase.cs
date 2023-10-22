using Myra.Attributes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public abstract class MultipleItemsContainerBase : Container, IMultipleItemsContainer
	{
		protected readonly ObservableCollection<Widget> _widgets = new ObservableCollection<Widget>();

		public override int ChildrenCount
		{
			get
			{
				return _widgets.Count;
			}
		}

		[Browsable(false)]
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

		public MultipleItemsContainerBase()
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
					OnChildAdded(w);
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (Widget w in args.OldItems)
				{
					OnChildRemoved(w);
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Reset)
			{
				foreach (Widget w in ChildrenCopy)
				{
					OnChildRemoved(w);
				}
			}

			InvalidateChildren();
		}

		protected virtual void OnChildAdded(Widget w)
		{
			w.Desktop = Desktop;
			w.Parent = this;
		}

		protected virtual void OnChildRemoved(Widget w)
		{
			w.Desktop = null;
			w.Parent = null;
		}


		public T AddChild<T>(T widget) where T : Widget
		{
			Widgets.Add(widget);
			return widget;
		}

		public override void RemoveChild(Widget widget)
		{
			_widgets.Remove(widget);
		}
	}
}