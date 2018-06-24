using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.Text
{
	public class TextRun
	{
		private string _text;
		private readonly SpriteFont _spriteFont;
		private bool _dirty = true;
		private Point _size;
		private Point? _renderedPosition;
		private readonly List<GlyphInfo> _glyphs = new List<GlyphInfo>();

		public int Count
		{
			get { return _text != null ? _text.Length : 0; }
		}

		public string Text
		{
			get { return _text; }
		}

		public Point Size
		{
			get
			{
				Update();

				return _size;
			}
		}

		public Color? Color { get; private set; }

		public SpriteFont SpriteFont
		{
			get { return _spriteFont; }
		}

		public int? UnderscoreIndex { get; set; }

		public Point? RenderedPosition
		{
			get { return _renderedPosition; }
		}

		public TextRun(SpriteFont font, Color? color)
		{
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}

			_spriteFont = font;
			Color = color;
		}

		public void Clear()
		{
			_glyphs.Clear();
			_size = Point.Zero;
			_text = string.Empty;

			_dirty = true;
		}

		public void Append(CharInfo ci)
		{
			_dirty = true;

			_text += ci.Value;

			var g = new GlyphInfo
			{
				Index = ci.OriginalIndex,
				Character = ci.Value,
				TextRun = this
			};

			_glyphs.Add(g);
		}

		public void Append(IEnumerable<CharInfo> charInfos)
		{
			foreach (var ci in charInfos)
			{
				Append(ci);
			}
		}

		internal void Update()
		{
			if (!_dirty)
			{
				return;
			}

			if (!string.IsNullOrEmpty(_text))
			{
				var sz = _spriteFont.MeasureString(_text);
				_size = new Point((int) sz.X, (int) sz.Y);

				var fontGlyphs = _spriteFont.GetGlyphs();

				var offset = Vector2.Zero;
				var firstGlyphOfLine = true;

				for (var i = 0; i < _text.Length; ++i)
				{
					SpriteFont.Glyph g;
					fontGlyphs.TryGetValue(_text[i], out g);

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
			}
			else
			{
				_size = new Point(0, _spriteFont.LineSpacing);
			}

			_dirty = false;
		}

		public void Draw(SpriteBatch batch, Point pos, Color color)
		{
			Update();

			batch.DrawString(_spriteFont, _text, pos.ToVector2(), color);

			if (MyraEnvironment.ShowUnderscores && UnderscoreIndex != null && UnderscoreIndex.Value < Count)
			{
				var g = _glyphs[UnderscoreIndex.Value];

				// Draw underscore
				batch.DrawRectangle(new Rectangle(pos.X + g.Bounds.X, 
					pos.Y + g.Bounds.Bottom, 
					g.Bounds.Width - 1, 1), color);
			}

			if (MyraEnvironment.DrawTextGlyphsFrames && !string.IsNullOrEmpty(_text))
			{
				for (var i = 0; i < _glyphs.Count; ++i)
				{
					var g = _glyphs[i];

					var r = new Rectangle(pos.X + g.Bounds.X, 
						pos.Y + g.Bounds.Y, 
						g.Bounds.Width, g.Bounds.Height);

					batch.DrawRectangle(r, Microsoft.Xna.Framework.Color.White);
				}
			}

			_renderedPosition = pos;
		}

		public GlyphInfo GetGlyphInfoByIndex(int index)
		{
			if (string.IsNullOrEmpty(_text) || _renderedPosition == null || index < 0 || index >= _text.Length)
			{
				return null;
			}

			Update();

			return _glyphs[index];
		}

		public GlyphInfo Hit(Point position)
		{
			if (string.IsNullOrEmpty(_text) || _renderedPosition == null)
			{
				return null;
			}

			var r = new Rectangle(_renderedPosition.Value, Size);
			if (!r.Contains(position))
			{
				return null;
			}

			for (var i = 0; i < _text.Length; ++i)
			{
				var g = _glyphs[i];

				r = new Rectangle(_renderedPosition.Value.X + g.Bounds.X, 
					_renderedPosition.Value.Y,
					g.Bounds.Width,
					_size.Y);

				if (r.Contains(position))
				{
					return g;
				}
			}

			return null;
		}
	}
}
