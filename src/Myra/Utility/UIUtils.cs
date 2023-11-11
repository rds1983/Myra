using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Utility
{
	internal static class UIUtils
	{
		public static bool ProcessWidgets(this Widget root, Func<Widget, bool> operation)
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

			foreach (var w in root.ChildrenCopy)
			{
				if (!ProcessWidgets(w, operation))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Sorts widgets by ZIndex using bubble sort
		/// </summary>
		/// <param name="list"></param>
		public static void SortWidgetsByZIndex(this List<Widget> list)
		{
			var n = list.Count;
			do
			{
				var newN = 0;
				for (var i = 1; i < n; ++i)
				{
					if (list[i - 1].ZIndex > list[i].ZIndex)
					{
						// Swap
						var temp = list[i - 1];
						list[i - 1] = list[i];
						list[i] = temp;

						newN = i;
					}
				}

				n = newN;
			} while (n > 1);
		}
	}
}
