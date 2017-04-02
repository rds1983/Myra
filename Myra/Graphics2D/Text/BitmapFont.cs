using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.Text
{
	public partial class BitmapFont
	{
		public static bool DrawFames { get; set; }

		private readonly TextureRegion[] _pages;
		private readonly Dictionary<char, Glyph> _glyphs;

		public Dictionary<char, Glyph> Glyphs
		{
			get { return _glyphs; }
		}

		public TextureRegion[] Pages
		{
			get { return _pages; }
		}

		public int LineHeight { get; private set; }

		public BitmapFont(Dictionary<char, Glyph> glyphs, TextureRegion[] pages, int lineHeight)
		{
			if (glyphs == null)
			{
				throw new ArgumentNullException("glyphs");
			}

			if (pages == null)
			{
				throw new ArgumentNullException("pages");
			}

			if (lineHeight <= 0)
			{
				throw new ArgumentOutOfRangeException("lineHeight");
			}

			_glyphs = glyphs;
			_pages = pages;
			LineHeight = lineHeight;
		}

		public void Draw(SpriteBatch batch, string text, Point pos, Color color)
		{
			for (var i = 0; i < text.Length; ++i)
			{
				var ch = text[i];
				Glyph glyph;
				if (!_glyphs.TryGetValue(ch, out glyph))
				{
					// Glyph not found
					continue;
				}

				var dest = new Rectangle(pos.X + glyph.Offset.X,
					pos.Y + glyph.Offset.Y,
					glyph.Region.Bounds.Width,
					glyph.Region.Bounds.Height);

				glyph.Region.Draw(batch, dest, color);

				pos.X += glyph.XAdvance;

				if (i < text.Length - 1)
				{
					pos.X += glyph.GetKerning(text[i + 1]);
				}
			}
		}

		public Point Measure(string text)
		{
			var result = Point.Zero;

			result.Y = LineHeight;

			if (string.IsNullOrEmpty(text))
			{
				return result;
			}

			var posX = 0;
			for (var i = 0; i < text.Length; ++i)
			{
				var ch = text[i];
				Glyph glyph;
				if (!_glyphs.TryGetValue(ch, out glyph))
				{
					// Glyph not found
					continue;
				}

				posX += glyph.XAdvance;

				if (i < text.Length - 1)
				{
					posX += glyph.GetKerning(text[i + 1]);
				}
			}

			result.X = posX;

			return result;
		}
	}
}