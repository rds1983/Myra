using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.UI
{
	public static class RenderUtils
	{
		public static void DrawRectWidth(SpriteBatch batch, Color color, Rectangle rect, int lineWidth)
		{
			if (lineWidth < 1 || lineWidth > 100)
			{
				throw new ArgumentOutOfRangeException("lineWidth");
			}

			if (lineWidth == 1)
			{
				batch.DrawRect(color, rect);
			}
			else
			{
				batch.FillSolidRect(color, new Rectangle(rect.Left, rect.Top, rect.Width, lineWidth));
				batch.FillSolidRect(color, new Rectangle(rect.Right - lineWidth, rect.Top, lineWidth, rect.Height));
				batch.FillSolidRect(color, new Rectangle(rect.Left, rect.Bottom - lineWidth, rect.Width, lineWidth));
				batch.FillSolidRect(color, new Rectangle(rect.Left, rect.Top, lineWidth, rect.Height));
			}
		}
	}
}