using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using System.Numerics;
using Matrix = System.Numerics.Matrix3x2;
#endif

namespace Myra.Utility
{
	internal static class Mathematics
	{
		public static readonly Point PointZero = new Point(0, 0);

		/// <summary>
		/// The value for which all absolute numbers smaller than are considered equal to zero.
		/// </summary>
		public const float ZeroTolerance = 1e-6f;

		/// <summary>
		/// Compares two floating point numbers based on an epsilon zero tolerance.
		/// </summary>
		/// <param name="left">The first number to compare.</param>
		/// <param name="right">The second number to compare.</param>
		/// <param name="epsilon">The epsilon value to use for zero tolerance.</param>
		/// <returns><c>true</c> if <paramref name="left"/> is within epsilon of <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		public static bool EpsilonEquals(this float left, float right, float epsilon = ZeroTolerance)
		{
			return Math.Abs(left - right) <= epsilon;
		}

		public static bool IsZero(this float a)
		{
			return a.EpsilonEquals(0.0f);
		}

		public static Point ToPoint(this Vector2 v) => new Point((int)Math.Round(v.X), (int)Math.Round(v.Y));

		public static Vector2 ToVector2(this Point p) => new Vector2(p.X, p.Y);

		public static Vector2 Transform(this Vector2 v, ref Matrix matrix)
		{
#if MONOGAME || FNA
			Vector2 result;
			Vector2.Transform(ref v, ref matrix, out result);
			return result;
#elif STRIDE
			Vector4 result;
			Vector2.Transform(ref v, ref matrix, out result);
			return new Vector2(result.X, result.Y);
#else
			return Vector2.Transform(v, matrix);
#endif
		}

		public static Rectangle Transform(this Rectangle r, ref Matrix matrix)
		{
			var position = new Vector2(r.X, r.Y).Transform(ref matrix);

			var transformScale = new Vector2(matrix.M11, matrix.M22);
			var scale = new Vector2(r.Width * transformScale.X, r.Height * transformScale.Y);

			return new Rectangle((int)position.X, (int)position.Y, (int)scale.X, (int)scale.Y);
		}
	}
}
