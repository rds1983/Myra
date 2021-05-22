using Myra.Utility;
using System;
using System.Collections.Generic;
using FontStashSharp;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Clr = Microsoft.Xna.Framework.Color;
#elif STRIDE
using Stride.Core.Mathematics;
using Clr = Stride.Core.Mathematics.Color;
#else
using System.Drawing;
using Clr = System.Drawing.Color;
using System.Numerics;
#endif

namespace Myra.Graphics2D.Text
{
	public class TextChunk
	{
		protected string _text;
		protected readonly SpriteFontBase _font;
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

		public TextChunk(SpriteFontBase font, string text, Point size, bool calculateGlyps)
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
					Index = i,
				});
			}

			var offset = Vector2.Zero;
			for (var i = 0; i < _text.Length; ++i)
			{
				char c = _text[i];
				
				if (char.IsLowSurrogate(c))
				{
					// Hopefully the text is valid UTF16...
					Glyphs[i].Bounds = Glyphs[i - 1].Bounds;
					continue;	
				}

				Vector2 v = _font.MeasureString(char.IsHighSurrogate(c) ? _text.Substring(i, 2) : _text[i].ToString());

				var result = new Rectangle((int)offset.X, (int)offset.Y, (int)v.X, (int)v.Y);

				Glyphs[i].Bounds = result;

				offset.X += v.X;	
			}
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

		public void Draw(RenderContext context, Vector2 pos, Color color)
		{
			context.DrawString(_font, _text, pos, color);

			if (MyraEnvironment.DrawTextGlyphsFrames && !string.IsNullOrEmpty(_text) && Glyphs != null)
			{
				for (var i = 0; i < Glyphs.Count; ++i)
				{
					var g = Glyphs[i];

					var r = new Rectangle((int)pos.X + g.Bounds.X,
						(int)pos.Y + g.Bounds.Y,
						g.Bounds.Width, g.Bounds.Height);

					context.DrawRectangle(r, Clr.White);
				}
			}
		}
	}
}
