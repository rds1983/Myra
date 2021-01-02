using System;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Graphics;
#else
using System.Drawing;
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
			var white = DefaultAssets.WhiteRegion;

			if (color == Color.White)
			{
				white.Draw(context, dest, Color);
			}
			else
			{
				var c = CrossEngineStuff.CreateColor((int)(Color.R * color.R / 255.0f),
					(int)(Color.G * color.G / 255.0f),
					(int)(Color.B * color.B / 255.0f),
					(int)(Color.A * color.A / 255.0f));

				white.Draw(context, dest, c);
			}
		}
	}
}