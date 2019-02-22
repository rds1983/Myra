using System;
using System.Collections.Generic;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Clr = Microsoft.Xna.Framework.Color;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
using Clr = Xenko.Core.Mathematics.Color;
#endif


namespace Myra.Graphics2D.Text
{
	public class TextLineWithGlyphs : TextLine
	{
		private readonly List<GlyphInfo> _glyphs = new List<GlyphInfo>();

		public TextLineWithGlyphs(SpriteFont font, string text, Point size) : base(font, text, size)
		{
			if (!string.IsNullOrEmpty(_text))
			{
				for (var i = 0; i < _text.Length; ++i)
				{
					_glyphs.Add(new GlyphInfo
					{
						TextLine = this,
						Character = _text[i]
					});
				}
#if MONOGAME
				var fontGlyphs = _spriteFont.GetGlyphs();

				var offset = Vector2.Zero;
				var firstGlyphOfLine = true;

				for (var i = 0; i < _text.Length; ++i)
				{
					var c = _text[i];

					SpriteFont.Glyph g;
					if (!fontGlyphs.TryGetValue(c, out g) && font.DefaultCharacter != null && c != '\n' && c != '\r')
					{
						fontGlyphs.TryGetValue(font.DefaultCharacter.Value, out g);
					}

					// The first character on a line might have a negative left side bearing.
					// In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
					//  so that text does not hang off the left side of its rectangle.
					if (firstGlyphOfLine)
					{
						offset.X = Math.Max(g.LeftSideBearing, 0);
						firstGlyphOfLine = false;
					}
					else
					{
						offset.X += _spriteFont.Spacing + g.LeftSideBearing;
					}

					var p = offset;

					p += g.Cropping.Location.ToVector2();

					var result = new Rectangle((int)p.X, (int)p.Y, (int)(g.Width + g.RightSideBearing), g.BoundsInTexture.Height);

					_glyphs[i].Bounds = result;

					offset.X += g.Width + g.RightSideBearing;
				}
#else
				var offset = Vector2.Zero;
				for (var i = 0; i < _text.Length; ++i)
				{
					Vector2 v = _spriteFont.MeasureString(_text[i].ToString());
					var result = new Rectangle((int)offset.X, (int)offset.Y, (int)v.X, (int)v.Y);

					_glyphs[i].Bounds = result;

					offset.X += v.X;
				}
#endif
			}
		}

		public override void Draw(SpriteBatch batch, Point pos, Color color, float opacity = 1)
		{
			base.Draw(batch, pos, color, opacity);

#if MONOGAME
			if (UnderscoreIndex != null && UnderscoreIndex.Value < Count)
			{
				var g = _glyphs[UnderscoreIndex.Value];

				// Draw underscore
				batch.DrawRectangle(new Rectangle(pos.X + g.Bounds.X,
					pos.Y + g.Bounds.Bottom,
					g.Bounds.Width - 1, 1), color);
			}
#endif

			if (MyraEnvironment.DrawTextGlyphsFrames && !string.IsNullOrEmpty(_text))
			{
				for (var i = 0; i < _glyphs.Count; ++i)
				{
					var g = _glyphs[i];

					var r = new Rectangle(pos.X + g.Bounds.X,
						pos.Y + g.Bounds.Y,
						g.Bounds.Width, g.Bounds.Height);

					batch.DrawRectangle(r, Clr.White);
				}
			}
		}

		public GlyphInfo GetGlyphInfoByIndex(int index)
		{
			if (string.IsNullOrEmpty(_text) || index < 0 || index >= _text.Length)
			{
				return null;
			}

			return _glyphs[index];
		}
	}
}
