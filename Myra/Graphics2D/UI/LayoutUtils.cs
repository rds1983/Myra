using Microsoft.Xna.Framework;
using Myra.Utility;

namespace Myra.Graphics2D.UI
{
	public static class LayoutUtils
	{
		public static Rectangle Align(Point containerSize, Point controlSize, HorizontalAlignment horizontalAlignment,
			VerticalAlignment verticalAlignment)
		{
			var result = new Rectangle
			{
				Width = controlSize.X,
				Height = controlSize.Y
			};

			switch (horizontalAlignment)
			{
				case HorizontalAlignment.Center:
					result.X = (containerSize.X - controlSize.X)/2;
					break;
				case HorizontalAlignment.Right:
					result.X = containerSize.X - controlSize.X;
					break;
				case HorizontalAlignment.Stretch:
					result.Width = containerSize.X;
					break;
			}

			switch (verticalAlignment)
			{
				case VerticalAlignment.Center:
					result.Y = (containerSize.Y - controlSize.Y)/2;
					break;
				case VerticalAlignment.Bottom:
					result.Y = containerSize.Y - controlSize.Y;
					break;
				case VerticalAlignment.Stretch:
					result.Height = containerSize.Y;
					break;
			}

			return result;
		}

		public static void LayoutChild(this Widget control, Rectangle containerBounds)
		{
			Point size;

			if (control.HorizontalAlignment != HorizontalAlignment.Stretch ||
				control.VerticalAlignment != VerticalAlignment.Stretch)
			{
				size = control.Measure(containerBounds.Size());
			}
			else
			{
				size = containerBounds.Size();
			}

			if (size.X > containerBounds.Width)
			{
				size.X = containerBounds.Width;
			}

			if (size.Y > containerBounds.Height)
			{
				size.Y = containerBounds.Height;
			}

			// Align
			var controlBounds = Align(containerBounds.Size(), size, control.HorizontalAlignment, control.VerticalAlignment);
			controlBounds.Offset(containerBounds.Location);

			var hintOffset = new Point(control.XHint, control.YHint);
			controlBounds.Offset(hintOffset);

			control.Bounds = controlBounds;
		}
	}
}