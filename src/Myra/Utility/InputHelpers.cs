using Myra.Graphics2D.UI;
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
	internal static class InputHelpers
	{
		private static bool CommonTouchCheck(this Widget w)
		{
			return w.Visible && w.Active && w.Enabled && w.ContainsTouch;
		}

		private static bool CommonMouseCheck(this Widget w)
		{
			return w.Visible && w.Active && w.Enabled && w.ContainsMouse;
		}

		public static bool FallsThrough(this Widget w, Point p)
		{
			// Only containers can fall through
			if (!(w is Grid ||
				w is StackPanel ||
				w is Panel ||
				w is SplitPane ||
				w is ScrollViewer))
			{
				return false;
			}

			// Real containers are solid only if backround is set
			if (w.Background != null)
			{
				return false;
			}

			var asScrollViewer = w as ScrollViewer;
			if (asScrollViewer != null)
			{
				// Special case
				if (asScrollViewer._horizontalScrollingOn && asScrollViewer._horizontalScrollbarFrame.Contains(p) ||
					asScrollViewer._verticalScrollingOn && asScrollViewer._verticalScrollbarFrame.Contains(p))
				{
					return false;
				}
			}

			return true;
		}
        
        /// <summary>
		/// Returns true if the event was handled
		/// </summary>
		/// <param name="widgets"></param>
		/// <returns></returns>
		public static bool ProcessTouchDown(this List<Widget> widgets)
		{
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];

				if (w.CommonTouchCheck())
				{
					// Since OnTouchDown may reset Desktop, we need to save break condition before calling it
					var doBreak = (w.Desktop != null && !w.FallsThrough(w.Desktop.TouchPosition));
					var inputHandled = w.OnTouchDown();
					if (doBreak || inputHandled)
					{
						return true;
					}
				}

				if (w.IsModal)
				{
					return true;
				}
			}
			return false;
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
					if (w.Desktop != null && !w.FallsThrough(w.Desktop.TouchPosition))
					{
						break;
					}
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
					w.IsMouseInside = false;
				}
			}

			// Second run: OnMouseEnter/OnMouseMoved
			for (var i = widgets.Count - 1; i >= 0; --i)
			{
				var w = widgets[i];

				if (w.CommonMouseCheck())
				{
					w.IsMouseInside = true;

					if (w.Desktop != null && !w.FallsThrough(w.Desktop.MousePosition))
					{
						break;
					}
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

					if (w.Desktop != null && !w.FallsThrough(w.Desktop.TouchPosition))
					{
						break;
					}
				}

				if (w.IsModal)
				{
					break;
				}
			}
		}
	}
}