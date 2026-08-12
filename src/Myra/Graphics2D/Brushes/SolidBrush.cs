using FontStashSharp.RichText;
using System;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Myra.MML;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.Brushes
{
	/// <summary>
	/// A brush that fills areas with a solid color.
	/// </summary>
	public class SolidBrush : IBrush, IHasColor
	{
		private Color _color = Color.White;

		/// <summary>
		/// Gets or sets the color of the brush.
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
		/// Initializes a new instance of the <see cref="SolidBrush"/> class with the specified color.
		/// </summary>
		/// <param name="color">The color of the brush.</param>
		public SolidBrush(Color color)
		{
			Color = color;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SolidBrush"/> class with the specified color name.
		/// </summary>
		/// <param name="color">The name of the color.</param>
		/// <exception cref="ArgumentException">The color name is not recognized.</exception>
		public SolidBrush(string color)
		{
			var c = ColorStorage.FromName(color);
			if (c == null)
			{
				throw new ArgumentException(string.Format("Could not recognize color '{0}'", color));
			}

			Color = c.Value;
		}

		/// <summary>
		/// Draws a solid color rectangle to the specified render context at the given destination.
		/// </summary>
		/// <param name="context">The render context to draw to.</param>
		/// <param name="dest">The destination rectangle where the brush will be drawn.</param>
		/// <param name="color">The color to blend with the brush's color.</param>
		public void Draw(RenderContext context, Rectangle dest, Color color)
		{
			var white = Stylesheet.Current.WhiteRegion;

			if (color == Color.White)
			{
				white.Draw(context, dest, Color);
			}
			else
			{
				var c = new Color((int)(Color.R * color.R / 255.0f),
					(int)(Color.G * color.G / 255.0f),
					(int)(Color.B * color.B / 255.0f),
					(int)(Color.A * color.A / 255.0f));

				white.Draw(context, dest, c);
			}
		}

		/// <summary>
		/// Returns a string representation of the brush's color.
		/// </summary>
		/// <returns>A string containing the color information.</returns>
		public override string ToString() => Color.ToColorString();
	}
}