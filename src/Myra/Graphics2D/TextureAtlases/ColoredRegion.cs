using System;

#if !STRIDE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Stride.Core.Mathematics;
using Stride.Graphics;
#endif

namespace Myra.Graphics2D.TextureAtlases
{
	[Obsolete("Use SolidBrush instead")]
	public class ColoredRegion: IImage
	{
		private Color _color = Color.White;

		public TextureRegion TextureRegion { get; set; }

		public Point Size
		{
			get
			{
				return new Point(TextureRegion.Bounds.Width, TextureRegion.Bounds.Height);
			}
		}

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

		public ColoredRegion(TextureRegion textureRegion, Color color)
		{
			if (textureRegion == null)
			{
				throw new ArgumentNullException("textureRegion");
			}

			TextureRegion = textureRegion;
			Color = color;
		}

		public void Draw(SpriteBatch batch, Rectangle dest, Color color)
		{
			if (color == Color.White)
			{
				TextureRegion.Draw(batch, dest, Color);
			} else
			{
				var c = new Color((int)(Color.R * color.R / 255.0f),
					(int)(Color.G * color.G / 255.0f),
					(int)(Color.B * color.B / 255.0f),
					(int)(Color.A * color.A / 255.0f));

				TextureRegion.Draw(batch, dest, c);
			}
		}
	}
}