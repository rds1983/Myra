using Myra.Graphics2D.UI;
using System;

namespace Myra.Utility
{
	public static class UIUtils
	{
		public static bool ProcessWidgets(Widget root, Func<Widget, bool> operation)
		{
			if (!root.Visible)
			{
				return true;
			}

			var result = operation(root);
			if (!result)
			{
				return false;
			}

			var asContainer = root as Container;
			if (asContainer != null)
			{
				foreach (var w in asContainer.ChildrenCopy)
				{
					if (!ProcessWidgets(w, operation))
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
