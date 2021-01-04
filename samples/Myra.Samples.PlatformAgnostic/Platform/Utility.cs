using Microsoft.Xna.Framework;

namespace Myra.Samples.AllWidgets
{
	static class Utility
	{
		public static Vector2 ToXNA(this System.Numerics.Vector2 r)
		{
			return new Vector2(r.X, r.Y);
		}

		public static Rectangle ToXNA(this System.Drawing.Rectangle r)
		{
			return new Rectangle(r.Left, r.Top, r.Width, r.Height);
		}

		public static System.Drawing.Rectangle ToSystemDrawing(this Rectangle r)
		{
			return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
		}


		public static Color ToXNA(this System.Drawing.Color c)
		{
			return new Color(c.R, c.G, c.B, c.A);
		}

		public static System.Drawing.Color ToSystemDrawing(this Color c)
		{
			return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
		}
	}
}
