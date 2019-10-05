using System;
using System.Linq;
using Myra.Attributes;

namespace Myra.Graphics2D.UI.Styles
{
	public class StyleNamesProvider : IItemsProvider
	{
		public object[] Items { get; private set; }

		public StyleNamesProvider(Control widget)
		{
			if (widget == null)
			{
				throw new ArgumentNullException("widget");
			}

			var styleNames = widget.GetStyleNames(Stylesheet.Current);

			if (styleNames == null)
			{
				styleNames = new[] {Stylesheet.DefaultStyleName};
			}

			Items = (from s in styleNames select (object) s).ToArray();
		}
	}
}
