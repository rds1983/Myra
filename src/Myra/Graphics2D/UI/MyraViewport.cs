using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Matrix = System.Numerics.Matrix3x2;
#endif

namespace Myra.Graphics2D.UI
{
	public struct MyraViewport
	{
		public Rectangle Bounds;
		public Matrix Transform;

		public MyraViewport(Rectangle bounds, Matrix transform)
		{
			Bounds = bounds;
			Transform = transform;
		}

		/// <summary>
		/// Creates a new instance of the viewport that matches the current view size
		/// </summary>
		public static MyraViewport CreateDefault()
		{
			var size = CrossEngineStuff.ViewSize;
			return new MyraViewport(new Rectangle(0, 0, size.X, size.Y), Matrix.Identity);
		}

		public override int GetHashCode()
		{
			int hashCode = 92326435;
			hashCode = hashCode * -1521134295 + Bounds.GetHashCode();
			hashCode = hashCode * -1521134295 + Transform.GetHashCode();
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is MyraViewport))
			{
				return false;
			}
			var viewport = (MyraViewport)obj;
			return Equals(this, viewport);
		}

		private static bool Equals(MyraViewport a, MyraViewport b)
		{
			return (a.Bounds == b.Bounds && a.Transform == b.Transform);
		}

		public static bool operator ==(MyraViewport a, MyraViewport b) => Equals(a, b);

		public static bool operator !=(MyraViewport a, MyraViewport b) => !Equals(a, b);
	}
}
