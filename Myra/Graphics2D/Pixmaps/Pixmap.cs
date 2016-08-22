using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.Pixmaps
{
	public abstract class Pixmap
	{
		public Point Size { get; private set; }

		public abstract SurfaceFormat Format { get; }

		protected Pixmap(Point size)
		{
			Size = size;
		}

		public abstract Color GetColor(Point p);
		public abstract void SetColor(Point p, Color value);

		public static void Copy(Pixmap source, Rectangle sourceRect, Pixmap dest, Point destPos)
		{
			sourceRect = Rectangle.Intersect(new Rectangle(0, 0, source.Size.X, source.Size.Y), sourceRect);

			sourceRect.X = Math.Min(sourceRect.X, dest.Size.X - destPos.X);
			sourceRect.Y = Math.Min(sourceRect.Y, dest.Size.Y - destPos.Y);

			Point pos;

			for (pos.Y = 0; pos.Y < sourceRect.Y; ++pos.Y)
			{
				for (pos.X = 0; pos.X < sourceRect.X; ++pos.X)
				{
					var color = source.GetColor(pos + sourceRect.Location);
					dest.SetColor(pos + destPos, color);
				}
			}
		}
	}
}
