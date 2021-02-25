using System;
using System.Collections.Generic;

namespace Myra.Graphics2D.UI.Styles
{
	public static class StylesheetExtensions
	{
		public static T SafelyGetStyle<T>(this Dictionary<string, T> styles, string id) where T : WidgetStyle
		{
			T result;
			if (!styles.TryGetValue(id, out result))
			{
				if (id == Stylesheet.DefaultStyleName)
				{
					throw new Exception("Stylesheet doesn't have the default " + typeof(T).Name);
				}
				else
				{
					throw new Exception("Stylesheet lacks the " + typeof(T).Name + " with id '" + id + "'");
				}
			}

			return result;
		}
	}
}
