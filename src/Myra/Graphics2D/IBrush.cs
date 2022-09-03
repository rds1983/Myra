#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D
{
	public interface IBrush
	{
		void Draw(RenderContext context, Rectangle dest, Color color);
	}

	public static class IBrushExtensions
	{
		public static void Draw(this IBrush brush, RenderContext context, Rectangle dest)
		{
			brush.Draw(context, dest, Color.White);
		}
	}
}
