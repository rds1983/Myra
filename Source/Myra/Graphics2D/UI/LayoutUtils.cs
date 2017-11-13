using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

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

		internal static void UpdateImageSize(this Image image, TextureRegion2D textureRegion2D)
		{
			if (textureRegion2D == null)
			{
				return;
			}

			if (image.WidthHint == null || textureRegion2D.Size.Width > image.WidthHint.Value)
			{
				image.WidthHint = (int)textureRegion2D.Size.Width;
			}

			if (image.HeightHint == null || textureRegion2D.Size.Height > image.HeightHint.Value)
			{
				image.HeightHint = (int)textureRegion2D.Size.Height;
			}
		}
	}
}