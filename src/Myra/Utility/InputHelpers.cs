using Myra.Graphics2D.UI;
using System.Collections.Generic;

namespace Myra.Utility
{
	internal static class InputHelpers
	{
		private static bool CommonTouchCheck(this Widget w)
		{
			return w.Visible && w.Active && w.Enabled && w.ContainsMouse;
		}

		public static void ProcessTouchDown(this List<Widget> widgets)
		{
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];

				if (w.CommonTouchCheck())
				{
					w.OnTouchDown();
					break;
				}

				if (w.IsModal)
				{
					break;
				}
			}
		}

		public static void ProcessTouchUp(this List<Widget> widgets)
		{
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];

				if (w.IsTouchInside)
				{
					w.OnTouchUp();
				}
			}
		}

		public static void ProcessTouchDoubleClick(this List<Widget> widgets)
		{
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];

				if (w.CommonTouchCheck())
				{
					w.OnTouchDoubleClick();
					break;
				}

				if (w.IsModal)
				{
					break;
				}
			}
		}

		public static void ProcessMouseMovement(this List<Widget> widgets)
		{
			// First run: call on OnMouseLeft on all widgets if it is required
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];
				if (!w.ContainsMouse && w.IsMouseInside)
				{
					w.OnMouseLeft();
				}
			}

			// Second run: OnMouseEnter/OnMouseMoved
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];

				if (w.CommonTouchCheck())
				{
					var isMouseOver = w.ContainsMouse;
					var wasMouseOver = w.IsMouseInside;

					if (isMouseOver && !wasMouseOver)
					{
						w.OnMouseEntered();
					}

					if (isMouseOver && wasMouseOver)
					{
						w.OnMouseMoved();
					}

					break;
				}

				if (w.IsModal)
				{
					break;
				}
			}
		}

		public static void ProcessTouchMovement(this List<Widget> widgets)
		{
			// First run: call on OnTouchLeft on all widgets if it is required
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];
				if (!w.ContainsTouch && w.IsTouchInside)
				{
					w.OnTouchLeft();
				}
			}

			// Second run: OnTouchEnter/OnTouchMoved
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];

				if (w.CommonTouchCheck())
				{
					var isTouchOver = w.ContainsTouch;
					var wasTouchOver = w.IsTouchInside;

					if (isTouchOver && !wasTouchOver)
					{
						w.OnTouchEntered();
					}

					if (isTouchOver && wasTouchOver)
					{
						w.OnTouchMoved();
					}

					break;
				}

				if (w.IsModal)
				{
					break;
				}
			}
		}
	}
}