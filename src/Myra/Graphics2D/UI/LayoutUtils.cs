using Microsoft.Xna.Framework;

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
					result.X = (containerSize.X - controlSize.X) / 2;
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
					result.Y = (containerSize.Y - controlSize.Y) / 2;
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
	}
}