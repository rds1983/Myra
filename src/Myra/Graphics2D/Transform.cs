#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using System.Numerics;
using Matrix = System.Numerics.Matrix3x2;
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
			Offset = new Point(0, 0);
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
#if MONOGAME || FNA
			return Matrix.CreateScale(Scale.X, Scale.Y, 1.0f) * Matrix.CreateTranslation(Offset.X, Offset.Y, 1.0f);
#elif STRIDE
			return Matrix.Scaling(Scale.X, Scale.Y, 1.0f) * Matrix.Translation(Offset.X, Offset.Y, 1.0f);
#else
			return Matrix.CreateScale(Scale) * Matrix.CreateTranslation(new Vector2(Offset.X, Offset.Y));
#endif
		}
	}
}
