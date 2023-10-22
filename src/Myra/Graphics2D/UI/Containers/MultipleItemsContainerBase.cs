using Myra.Attributes;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public abstract class MultipleItemsContainerBase : Widget
	{
		[Content]
		[Browsable(false)]
		public virtual ObservableCollection<Widget> Widgets => Children;

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
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		[Obsolete("Use Widgets.Add")]
		public void AddChild(Widget child)
		{
			Widgets.Add(child);
		}

		[Obsolete("Use Widgets.Remove")]
		public void RemoveChild(Widget child)
		{
			Widgets.Remove(child);
		}

	}
}