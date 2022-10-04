using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Utility
{
	internal static class GraphicsExtension
	{
		public static Point Size(this Rectangle r)
		{
			return new Point(r.Width, r.Height);
		}
	}
}