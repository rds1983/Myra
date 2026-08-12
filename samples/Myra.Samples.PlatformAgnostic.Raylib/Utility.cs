using FontStashSharp;
using System.Drawing;

namespace Myra.Samples.AllWidgets
{
	internal static class Utility
	{
		public static Raylib_cs.Rectangle ToRaylib(this Rectangle r) => new Raylib_cs.Rectangle(r.X, r.Y, r.Width, r.Height);

		public static Raylib_cs.Color ToRaylib(this FSColor c) => new Raylib_cs.Color(c.R, c.G, c.B, c.A);
	}
}
