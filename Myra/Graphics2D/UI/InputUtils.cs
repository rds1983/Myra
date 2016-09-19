using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Myra.Graphics2D.UI
{
	internal static class InputUtils
	{
		public static void HandleMouseDown(this IEnumerable<Widget> widgets, MouseButtons buttons)
		{
			foreach (var w in widgets)
			{
				if (!w.Visible)
				{
					continue;
				}

				if (w.IsMouseOver)
				{
					w.OnMouseDown(buttons);
				}
			}
		}

		public static void HandleMouseUp(this IEnumerable<Widget> widgets, MouseButtons buttons)
		{
			foreach (var w in widgets)
			{
				if (!w.Visible)
				{
					continue;
				}

				if (w.MouseButtonsDown != null)
				{
					w.OnMouseUp(buttons);
				}
			}
		}

		public static void HandleMouseMovement(this IEnumerable<Widget> widgets, Point mousePosition)
		{
			foreach (var w in widgets)
			{
				if (!w.Visible)
				{
					continue;
				}

				if (w.Bounds.Contains(mousePosition))
				{
					if (w.IsMouseOver)
					{
						// Already inside
						w.OnMouseMoved(mousePosition);
					}
					else
					{
						w.OnMouseEntered(mousePosition);
					}
				} else if (w.IsMouseOver)
				{
					// Left
					w.OnMouseLeft();
				}
			}
		}
	}
}
