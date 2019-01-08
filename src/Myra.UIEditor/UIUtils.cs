using Myra.Graphics2D.UI;
using System.Collections.Generic;

namespace Myra.UIEditor
{
	public static class UIUtils
	{
		public static IEnumerable<Widget> GetRealChildren(this Widget w)
		{
			IEnumerable<Widget> widgets = null;
			if (w is Window)
			{
				widgets = ((Container)w).Children;
			}
			else if (w is SplitPane)
			{
				widgets = ((SplitPane)w).Widgets;
			}
			else if (w is MultipleItemsContainer)
			{
				var container = w as MultipleItemsContainer;
				widgets = container.Widgets;
			}
			else if (w is ScrollPane)
			{
				widgets = new Widget[] { ((ScrollPane)w).Widget };
			}

			return widgets;
		}
	}
}
