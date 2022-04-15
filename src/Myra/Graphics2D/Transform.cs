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
			Offset = new Point(Offset.X + (int)(offset.X * Scale.X), Offset.Y + (int)(offset.Y * Scale.Y));
		}

		/// <summary>
		/// Adds scale to the transform
		/// </summary>
		/// <param name="offset"></param>
		public void AddScale(Vector2 scale)
		{
			Scale *= scale;
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

		public Matrix ToMatrix()
		{
			return Matrix.CreateScale(Scale.X, Scale.Y, 1.0f) * Matrix.CreateTranslation(Offset.X, Offset.Y, 1.0f);
		}
	}
}
