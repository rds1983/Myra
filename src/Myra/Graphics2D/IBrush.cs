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
	/// <summary>
	/// Represents a brush that can be used to fill areas with color or patterns.
	/// </summary>
	public interface IBrush
	{
		/// <summary>
		/// Draws the brush content to the specified destination rectangle.
		/// </summary>
		/// <param name="context">The render context to draw with.</param>
		/// <param name="dest">The destination rectangle where the brush should be drawn.</param>
		/// <param name="color">The color to apply when drawing the brush.</param>
		void Draw(RenderContext context, Rectangle dest, Color color);
	}

	/// <summary>
	/// Extension methods for IBrush objects.
	/// </summary>
	public static class IBrushExtensions
	{
		/// <summary>
		/// Draws the brush with white color.
		/// </summary>
		/// <param name="brush">The brush to draw.</param>
		/// <param name="context">The render context to draw with.</param>
		/// <param name="dest">The destination rectangle where the brush should be drawn.</param>
		public static void Draw(this IBrush brush, RenderContext context, Rectangle dest)
		{
			brush.Draw(context, dest, Color.White);
		}
	}
}
