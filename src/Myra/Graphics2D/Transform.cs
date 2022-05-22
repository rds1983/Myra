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
		private Vector2 _offset;
		private Vector2 _scale;
		private Matrix? _matrix, _inverseMatrix;
		private bool _originNonZero;

		public Vector2 Offset
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

		public Vector2 LocalOrigin { get; private set; }
		public Vector2 Origin { get; private set; }

		public float Rotation { get; private set; }

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
					Matrix.Invert(Matrix, out inverse);
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
			Scale = Vector2.One;
			_originNonZero = false;
		}

		public void AddTransform(Vector2 offset, Vector2 origin, Vector2 scale, float rotation)
		{
			Offset += offset * Scale;
			Scale *= scale;
			Rotation += rotation;

			if (rotation != 0 || origin != Vector2.Zero)
			{
				LocalOrigin = Offset + origin;
				Origin = Offset + origin * scale;
			}
		}

		public Vector2 Apply(Vector2 source)
		{
			return Offset + source * Scale;
		}

		public Rectangle Apply(Rectangle source)
		{
			var pos = Mathematics.ToPoint(Apply(source.Location.ToVector2()));
			var size = Mathematics.ToPoint(Scale.Multiply(source.Size()));

			return new Rectangle(pos.X, pos.Y, size.X, size.Y);
		}

		private void ResetMatrices()
		{
			_matrix = null;
			_inverseMatrix = null;
		}
	}
}
