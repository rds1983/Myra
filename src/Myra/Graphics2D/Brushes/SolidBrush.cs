using FontStashSharp.RichText;
using System;
using Myra.Graphics2D.UI.Styles;


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
	public class SolidBrush : IBrush
	{
		private Color _color = Color.White;

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

		public SolidBrush(Color color)
		{
			Color = color;
		}

		public SolidBrush(string color)
		{
			var c = ColorStorage.FromName(color);
			if (c == null)
			{
				throw new ArgumentException(string.Format("Could not recognize color '{0}'", color));
			}

			Color = c.Value;
		}

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
	}
}