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
					break;
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
					break;
				}
			}
		}

		public static void HandleMouseMovement(this IEnumerable<Widget> widgets, Point mousePosition)
		{
			var movedHandled = false;
			foreach (var w in widgets)
			{
				if (!w.Visible)
				{
					continue;
				}

				if (!movedHandled && w.Bounds.Contains(mousePosition))
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

					movedHandled = true;
				}
				else if (w.IsMouseOver)
				{
					// Left
					w.OnMouseLeft();
				}
			}
		}
	}
}