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

		public List<GlyphInfo> Glyphs { get; } = new List<GlyphInfo>();

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

			var rects = _font.GetGlyphRects(_text, Vector2.Zero);

			Glyphs.Clear();
			for(var i = 0; i < rects.Count; ++i)
			{
				Glyphs.Add(new GlyphInfo
				{
					TextChunk = this,
					Character = _text[i],
					Index = i,
					Bounds = rects[i]
				});
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
