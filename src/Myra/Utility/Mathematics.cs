using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using System.Numerics;
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

		public static Point ToPoint(this Vector2 v)
		{
			return new Point((int)Math.Round(v.X), (int)Math.Round(v.Y));
		}

		public static Vector2 Multiply(this Vector2 v, Point p)
		{
			return new Vector2(v.X * p.X, v.Y * p.Y);
		}
	}
}
