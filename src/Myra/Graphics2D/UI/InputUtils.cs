using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Myra.Graphics2D.UI
{
	internal static class InputUtils
	{
		public static void HandleMouseDown(this List<Widget> widgets, MouseButtons buttons)
		{
			for(var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];

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

		public static void HandleMouseDoubleClick(this List<Widget> widgets, MouseButtons buttons)
		{
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];

				if (!w.Visible)
				{
					continue;
				}

				if (w.IsMouseOver)
				{
					w.OnMouseDoubleClick(buttons);
					break;
				}
			}
		}

		public static void HandleMouseUp(this List<Widget> widgets, MouseButtons buttons)
		{
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];

				if (!w.Visible)
				{
					continue;
				}

				if (w.IsMouseOver)
				{
					w.OnMouseUp(buttons);
					break;
				}
			}
		}

		public static void HandleMouseMovement(this List<Widget> widgets)
		{
			foreach(var w in widgets)
			{
				if (!w.Visible)
				{
					continue;
				}

				if (w.IsMouseOver)
				{
					if (w.WasMouseOver)
					{
						// Already inside
						w.OnMouseMoved();
					}
					else
					{
						w.OnMouseEntered();
					}
				}
				else if (w.WasMouseOver)
				{
					// Left
					w.OnMouseLeft();
				}
			}
		}

		public static void HandleTouchDown(this List<Widget> widgets)
		{
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];

				if (!w.Visible)
				{
					continue;
				}

				if (w.IsMouseOver)
				{
					w.OnTouchDown();
					break;
				}
			}
		}

		public static void HandleTouchUp(this List<Widget> widgets)
		{
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];

				if (!w.Visible)
				{
					continue;
				}

				if (w.IsMouseOver)
				{
					w.OnTouchUp();
					break;
				}
			}
		}
	}
}