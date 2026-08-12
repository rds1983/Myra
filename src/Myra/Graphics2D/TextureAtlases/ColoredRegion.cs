using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.TextureAtlases
{
	/// <summary>
	/// Represents a texture region with an applied color tint.
	/// </summary>
	public class ColoredRegion : IImage
	{
		private Color _color = Color.White;

		/// <summary>
		/// Gets or sets the texture region.
		/// </summary>
		public TextureRegion TextureRegion { get; set; }

		/// <summary>
		/// Gets the size of the colored region.
		/// </summary>
		public Point Size
		{
			get
			{
				return new Point(TextureRegion.Bounds.Width, TextureRegion.Bounds.Height);
			}
		}

		/// <summary>
		/// Gets or sets the color tint applied to the texture region.
		/// </summary>
		public Color Color
		{
			get
			{
				return _color;
			}

			set
			{
				_color = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ColoredRegion"/> class with the specified texture region and color.
		/// </summary>
		/// <param name="textureRegion">The texture region to use.</param>
		/// <param name="color">The color tint to apply.</param>
		/// <exception cref="ArgumentNullException"><paramref name="textureRegion"/> is null.</exception>
		public ColoredRegion(TextureRegion textureRegion, Color color)
		{
			if (textureRegion == null)
			{
				throw new ArgumentNullException("textureRegion");
			}

			TextureRegion = textureRegion;
			Color = color;
		}

		/// <summary>
		/// Draws the colored region to the specified render context at the given destination.
		/// </summary>
		/// <param name="context">The render context to draw to.</param>
		/// <param name="dest">The destination rectangle where the region will be drawn.</param>
		/// <param name="color">The color to blend with the region's color.</param>
		public void Draw(RenderContext context, Rectangle dest, Color color)
		{
			if (color == Color.White)
			{
				TextureRegion.Draw(context, dest, Color);
			}
			else
			{
				var c = new Color((int)(Color.R * color.R / 255.0f),
					(int)(Color.G * color.G / 255.0f),
					(int)(Color.B * color.B / 255.0f),
					(int)(Color.A * color.A / 255.0f));

				TextureRegion.Draw(context, dest, c);
			}
		}
	}
}