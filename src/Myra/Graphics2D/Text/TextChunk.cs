using Myra.Utility;
using System;
using System.Collections.Generic;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.Text
{
	public class TextChunk
	{
		protected string _text;
		protected readonly SpriteFont _font;
		protected Point _size;

		public List<GlyphInfo> Glyphs { get; private set; }

		public int Count { get { return _text.Length(); } }
		public string Text { get { return _text; } }
		public Point Size { get { return _size; } }

		public int LineIndex { get; internal set; }
		public int ChunkIndex { get; internal set; }
		public int Top { get; internal set; }
		public int TextStartIndex { get; internal set; }
		public Color? Color;

		public TextChunk(SpriteFont font, string text, Point size, bool calculateGlyps)
		{
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}

			_font = font;
			_text = text;
			_size = size;

			if (calculateGlyps)
			{
				CalculateGlyphs();
			}
		}

		private void CalculateGlyphs()
		{
			if (string.IsNullOrEmpty(_text))
			{
				return;
			}

			Glyphs = new List<GlyphInfo>();

			for (var i = 0; i < _text.Length; ++i)
			{
				Glyphs.Add(new GlyphInfo
				{
					TextChunk = this,
					Character = _text[i],
					Index = i
				});
			}
#if MONOGAME
			var fontGlyphs = _font.GetGlyphs();

			var offset = Vector2.Zero;
			var firstGlyphOfLine = true;

			for (var i = 0; i < _text.Length; ++i)
			{
				var c = _text[i];

				SpriteFont.Glyph g;
				if (!fontGlyphs.TryGetValue(c, out g) && _font.DefaultCharacter != null && c != '\n' && c != '\r')
				{
					fontGlyphs.TryGetValue(_font.DefaultCharacter.Value, out g);
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
					offset.X += _font.Spacing + g.LeftSideBearing;
				}

				var p = offset;

				p += g.Cropping.Location.ToVector2();

				var result = new Rectangle((int)p.X, (int)p.Y, (int)(g.Width + g.RightSideBearing), g.BoundsInTexture.Height);

				Glyphs[i].Bounds = result;

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

		public GlyphInfo GetGlyphInfoByIndex(int index)
		{
			if (string.IsNullOrEmpty(_text) || index < 0 || index >= _text.Length)
			{
				return null;
			}

			return Glyphs[index];
		}

		public int? GetGlyphIndexByX(int x)
		{
			if (Glyphs.Count == 0 || x < 0)
			{
				return null;
			}

			var i = 0;
			for (; i < Glyphs.Count; ++i)
			{
				var glyph = Glyphs[i];
				var right = glyph.Bounds.Right;
				if (i < Glyphs.Count - 1)
				{
					right = Glyphs[i + 1].Bounds.X;
				}

				if (glyph.Bounds.X <= x && x <= right)
				{
					if (x - glyph.Bounds.X >= glyph.Bounds.Width / 2)
					{
						++i;
					}

					break;
				}
			}

			if (i - 1 >= 0 && i - 1 < Glyphs.Count && Glyphs[i - 1].Character == '\n')
			{
				--i;
			}

			return i;
		}

		public virtual void Draw(SpriteBatch batch, Point pos, Color color, float opacity = 1.0f)
		{
			batch.DrawString(_font, _text, new Vector2(pos.X, pos.Y), color * opacity);

			if (MyraEnvironment.DrawTextGlyphsFrames && !string.IsNullOrEmpty(_text) && Glyphs != null)
			{
				for (var i = 0; i < Glyphs.Count; ++i)
				{
					var g = Glyphs[i];

					var r = new Rectangle(pos.X + g.Bounds.X,
						pos.Y + g.Bounds.Y,
						g.Bounds.Width, g.Bounds.Height);

					batch.DrawRectangle(r, Microsoft.Xna.Framework.Color.White);
				}
			}
		}
	}
}
