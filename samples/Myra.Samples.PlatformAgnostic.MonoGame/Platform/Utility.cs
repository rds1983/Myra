using FontStashSharp;
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


		public static Color ToXNA(this FSColor c)
		{
			return new Color(c.R, c.G, c.B, c.A);
		}

		public static System.Drawing.Color ToSystemDrawing(this Color c)
		{
			return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
		}

		public static Matrix ToXNA(this System.Numerics.Matrix3x2 matrix)
		{
			var result = Matrix.Identity;
			result.M11 = matrix.M11;
			result.M12 = matrix.M12;
			result.M21 = matrix.M21;
			result.M22 = matrix.M22;

			result.M41 = matrix.M31;
			result.M42 = matrix.M32;

			return result;
		}
	}
}
