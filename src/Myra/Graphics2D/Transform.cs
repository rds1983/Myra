using Myra.Utility;

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
		private Point _offset;
		private Vector2 _scale;
		private Matrix? _matrix, _inverseMatrix;

		public Point Offset
		{
			get => _offset;
			set
			{
				if (value == _offset)
				{
					return;
				}

				_offset = value;
				ResetMatrices();
			}
		}

		public Vector2 Scale
		{
			get => _scale;

			set
			{
				if (value == _scale)
				{
					return;
				}

				_scale = value;
				ResetMatrices();
			}
		}

		public Matrix Matrix
		{
			get
			{
				if (_matrix == null)
				{
#if MONOGAME || FNA
					_matrix = Matrix.CreateScale(Scale.X, Scale.Y, 1.0f) * Matrix.CreateTranslation(Offset.X, Offset.Y, 1.0f);
#elif STRIDE
					_matrix = Matrix.Scaling(Scale.X, Scale.Y, 1.0f) * Matrix.Translation(Offset.X, Offset.Y, 1.0f);
#else
					_matrix = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(new Vector2(Offset.X, Offset.Y));
#endif
				}

				return _matrix.Value;
			}
		}

		public Matrix InverseMatrix
		{
			get
			{
				if (_inverseMatrix == null)
				{
#if MONOGAME || FNA || STRIDE
					_inverseMatrix = Matrix.Invert(Matrix);
#else
					Matrix inverse = Matrix.Identity;
					Matrix.Invert(_matrix, out inverse);
					_inverseMatrix = inverse;
#endif
				}

				return _inverseMatrix.Value;
			}
		}


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
		public void AddOffset(int x, int y) => AddOffset(new Point(x, y));


		/// <summary>
		/// Adds offset to the transform
		/// </summary>
		/// <param name="offset"></param>
		public void AddOffset(Point offset)
		{
			Offset += Scale.Multiply(offset).ToPoint();
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

		public Point Apply(Point source)
		{
			return Offset + Scale.Multiply(source).ToPoint();
		}


		public Rectangle Apply(Rectangle source)
		{
			var pos = Apply(source.Location);
			var size = Scale.Multiply(source.Size).ToPoint();

			return new Rectangle(pos.X, pos.Y, size.X, size.Y);
		}

		private void ResetMatrices()
		{
			_matrix = null;
			_inverseMatrix = null;
		}
	}
}
