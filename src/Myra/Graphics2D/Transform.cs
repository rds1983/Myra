#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D
{
	public struct Transform
	{
		public Point Offset;
		public Vector2 Scale;

		/// <summary>
		/// Resets the transform
		/// </summary>
		public void Reset()
		{
			Offset = Point.Zero;
			Scale = Vector2.One;
		}

		/// <summary>
		/// Adds offset to the transform
		/// </summary>
		/// <param name="offset"></param>
		public void AddOffset(Point offset)
		{
			Offset = new Point(Offset.X + offset.X, Offset.Y + offset.Y);
		}

		/// <summary>
		/// Adds scale to the transform
		/// </summary>
		/// <param name="offset"></param>
		public void AddScale(Vector2 scale)
		{
			Scale *= scale;
		}

		/// <summary>
		/// Adds another transform
		/// </summary>
		/// <param name="transform"></param>
		public void AddTransform(Transform transform)
		{
			AddOffset(transform.Offset);
			AddScale(transform.Scale);
		}

		public Vector2 Apply(Vector2 source)
		{
			return new Vector2(Offset.X, Offset.Y) + source * Scale;
		}

		public Rectangle Apply(Rectangle source)
		{
			return new Rectangle((int)(Offset.X + source.X * Scale.X),
				(int)(Offset.Y + source.Y * Scale.Y),
				(int)(source.Width * Scale.X),
				(int)(source.Height * Scale.Y));
		}
	}
}
