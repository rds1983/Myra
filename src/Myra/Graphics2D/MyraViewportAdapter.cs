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
	public struct MyraViewportAdapter
	{
		public int VirtualWidth { get; set; }

		public int VirtualHeight { get; set; }

		public int X { get; set; }

		public int Y { get; set; }

		public Matrix TransformMatrix { get; set; }
	}
}
