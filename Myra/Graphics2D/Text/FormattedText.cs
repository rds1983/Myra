using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Utility;

namespace Myra.Graphics2D.Text
{
	public class FormattedText
	{
		public struct Options
		{
			public bool Wrap;
			public bool Color;
		}

		private class TextParser
		{
			private enum ParseType
			{
				Text,
				Color
			}

			private enum LexemeType
			{
				Word,
				Space,
				LineBreak,
				Color
			}

			private class Lexeme
			{
				public StringBuilder text = new StringBuilder();
				public LexemeType type = LexemeType.Word;
				public readonly List<CharInfo> chars = new List<CharInfo>();
				public Color color;

				public void AppendChar(char c, int originalIndex)
				{
					chars.Add(new CharInfo
					{
						Value = c,
						OriginalIndex = originalIndex
					}
						);

					text.Append(c);
				}
			}

			private readonly List<Lexeme> _lexemes = new List<Lexeme>();
			private Lexeme _lexeme = new Lexeme();
			private Color? _currentColor;
			private readonly StringBuilder _fullString = new StringBuilder();
			private GlyphRun _lastRun;
			private readonly List<GlyphRun>  _result = new List<GlyphRun>();
			private BitmapFont _font;

			private void StoreLexeme()
			{
				if (_lexeme.chars.Count == 0)
				{
					return;
				}

				_lexemes.Add(_lexeme);
				_lexeme = new Lexeme();
			}

			private void StoreRun()
			{
				_result.Add(_lastRun);
				_lastRun = new GlyphRun(_font.LineHeight);

				_fullString.Clear();
			}

			public GlyphRun[] Format(BitmapFont font, string text, int width, Options options)
			{
				if (string.IsNullOrEmpty(text))
				{
					return new GlyphRun[0];
				}

				_font = font;

				var pt = ParseType.Text;

				// First run - gather lexemes
				_lexeme = new Lexeme();
				_lexemes.Clear();
				var str = new StringBuilder();
				for (var pos = 0; pos < text.Length; ++pos)
				{
					var c = text[pos];
					if (pt == ParseType.Text)
					{
						if (options.Color && c == '[' && pos < text.Length - 1 && text[pos + 1] != '[')
						{
							// Switch to color parsing
							StoreLexeme();
							pt = ParseType.Color;
						}
						else if (c == '\n')
						{
							StoreLexeme();

							// Add linebreak lexeme
							_lexeme.type = LexemeType.LineBreak;
							_lexeme.AppendChar(c, pos);
							StoreLexeme();
						}
						else if (c == ' ' && _lexeme.type == LexemeType.Word)
						{
							StoreLexeme();
							_lexeme.type = LexemeType.Space;
							_lexeme.AppendChar(c, pos);
						}
						else if (c > 32)
						{
							if (_lexeme.type == LexemeType.Space)
							{
								StoreLexeme();
								_lexeme.type = LexemeType.Word;
							}

							_lexeme.AppendChar(c, pos);

							if (options.Color && c == '[')
							{
								// Means we have [[
								// So one [ should be skipped
								++pos;
							}
						}
					} 
					else if (pt == ParseType.Color)
					{
						if (c != ']')
						{
							str.Append(c);
						}
						else if (str.Length >= 1)
						{
							var color = str.ToString().FromName();
							str.Clear();

							var l = new Lexeme
							{
								type = LexemeType.Color,
								color = color ?? Color.White
							};

							_lexemes.Add(l);
							pt = ParseType.Text;
						}
					}
				}

				StoreLexeme();

				// Second run - go through lexemes
				_result.Clear();
				_lastRun = new GlyphRun(_font.LineHeight);
				_fullString.Clear();

				foreach (var li in _lexemes)
				{
					switch (li.type)
					{
						case LexemeType.LineBreak:
							_lastRun.Append(font, li.chars, _currentColor);
							StoreRun();
							break;
						case LexemeType.Space:
						case LexemeType.Word:
							_fullString.Append(li.text);

							if (options.Wrap)
							{
								var sz = font.Measure(_fullString.ToString());
								if (sz.X <= width)
								{
									_lastRun.Append(font, li.chars, _currentColor);
								}
								else
								{
									StoreRun();

									if (li.type == LexemeType.Word)
									{
										_fullString.Append(li.text);
										_lastRun.Append(font, li.chars, _currentColor);
									}
								}
							}
							else
							{
								_lastRun.Append(font, li.chars, _currentColor);
							}
							break;
						case LexemeType.Color:
							_currentColor = li.color;
							break;
					}
				}

				if (_lastRun.Count > 0)
				{
					StoreRun();
				}

				return _result.ToArray();
			}
		}

		private BitmapFont _font;
		private string _text = "";
		private int _verticalSpacing;
		private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;
		private VerticalAlignment _verticalAlignment = VerticalAlignment.Top;
		private Point _size = new Point(10000, 0);
		private GlyphRun[] _strings;
		private int _totalHeight;
		private bool _wrap;
		private bool _colored = true;
		private bool _dirty = true;

		public BitmapFont Font
		{
			get { return _font; }
			set
			{
				if (value == _font)
				{
					return;
				}

				_font = value;
				InvalidateLayout();
			}
		}

		public string Text
		{
			get { return _text; }
			set
			{
				if (value == _text)
				{
					return;
				}

				_text = value;
				InvalidateLayout();
			}
		}

		public int VerticalSpacing
		{
			get { return _verticalSpacing; }

			set
			{
				if (value == _verticalSpacing)
				{
					return;
				}

				_verticalSpacing = value;
				InvalidateLayout();
			}
		}

		public HorizontalAlignment HorizontalAlignment
		{
			get { return _horizontalAlignment; }
			set
			{
				if (value == _horizontalAlignment)
				{
					return;
				}

				_horizontalAlignment = value;
				InvalidateLayout();
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			get { return _verticalAlignment; }
			set
			{
				if (value == _verticalAlignment)
				{
					return;
				}

				_verticalAlignment = value;
				InvalidateLayout();
			}
		}

		public Point Size
		{
			get { return _size; }
			set
			{
				if (value == _size)
				{
					return;
				}

				_size = value;
				InvalidateLayout();
			}
		}

		public bool Wrap
		{
			get { return _wrap; }

			set
			{
				if (value == _wrap)
				{
					return;
				}

				_wrap = value;
				InvalidateLayout();
			}
		}

		public bool IsColored
		{
			get
			{
				return _colored;
			}
			set
			{
				if (value == _colored)
				{
					return;
				}

				_colored = value;
				InvalidateLayout();
			}
		}

		public GlyphRun[] Strings
		{
			get
			{
				Update();
				return _strings;
			}
		}

		public int TotalHeight
		{
			get
			{
				Update();
				return _totalHeight;
			}
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			var parser = new TextParser();

			var options = new Options
			{
				Color = _colored,
				Wrap = _wrap
			};

			_strings = parser.Format(_font, _text, _size.X, options);

			// Calculate total height
			_totalHeight = 0;
			for (var i = 0; i < _strings.Length; ++i)
			{
				var si = _strings[i];

				_totalHeight += si.Bounds.Height;

				if (i < _strings.Length - 1)
				{
					_totalHeight += _verticalSpacing;
				}
			}

			// Apply alignments
			var offset = Point.Zero;
			if (VerticalAlignment == VerticalAlignment.Center)
			{
				offset.Y = (_size.Y - _totalHeight)/2;
			}
			else if (VerticalAlignment == VerticalAlignment.Bottom)
			{
				offset.Y = _size.Y - _totalHeight;
			}

			foreach (var si in _strings)
			{
				if (_horizontalAlignment == HorizontalAlignment.Center)
				{
					offset.X = (_size.X - si.Bounds.Width)/2;
				}
				else if (_horizontalAlignment == HorizontalAlignment.Right)
				{
					offset.X = _size.X - si.Bounds.Width;
				}

				si.Bounds.Location += offset;
			}

			_dirty = false;
		}

		public void Draw(SpriteBatch batch, Rectangle bounds, Color textColor)
		{
			var strings = Strings;

			if (strings == null || strings.Length == 0)
			{
				return;
			}

			var y = bounds.Top;

			foreach (var si in strings)
			{
				si.ResetDraw();

				if (y + si.Bounds.Y >= bounds.Top && y < bounds.Bottom)
				{
					var x = bounds.Left + si.Bounds.Left;

					si.Draw(batch, new Point(x, y), textColor);
				}

				y += si.Bounds.Height;
				y += VerticalSpacing;
			}
		}

		public void Draw(SpriteBatch batch, Point pos, Color textColor)
		{
			Draw(batch, new Rectangle(pos.X, pos.Y, _size.X, _size.Y), textColor);
		}

		private void InvalidateLayout()
		{
			_dirty = true;
		}

		public GlyphRender Hit(Point pos)
		{
			var strings = Strings;

			foreach (var si in strings)
			{
				if (si.RenderedBounds == null)
				{
					continue;
				}

				foreach (var gr in si.GlyphRenders)
				{
					if (gr.RenderedBounds.HasValue)
					{
						var glyphHitBounds = new Rectangle(gr.RenderedBounds.Value.Left,
							si.RenderedBounds.Value.Top,
							gr.RenderedBounds.Value.Width,
							si.RenderedBounds.Value.Height);

						if (glyphHitBounds.Contains(pos))
						{
							return gr;
						}
					}
				}

				if (pos.Y >= si.RenderedBounds.Value.Top &&
				    pos.Y < si.RenderedBounds.Value.Bottom &&
					si.GlyphRenders.Count > 0)
				{
					// If position fits into the entire line, use the last glyph as result
					return si.GlyphRenders[si.GlyphRenders.Count - 1];
				}
			}

			return null;
		}

		public int? GetIndexByGlyphRender(GlyphRender glyphRender)
		{
			var strings = Strings;

			var ch = 0;
			foreach (var si in strings)
			{
				foreach (var gr in si.GlyphRenders)
				{
					if (gr == glyphRender)
					{
						return ch;
					}

					++ch;
				}
			}

			return null;
		}

		public bool GetPositionByCharIndex(int charIndex, out int lineIndex, out int glyphIndex)
		{
			var strings = Strings;

			lineIndex = 0;
			glyphIndex = 0;

			var line = 0;
			var ch = 0;
			foreach (var si in strings)
			{
				var glyph = 0;

				foreach (var gr in si.GlyphRenders)
				{
					if (charIndex == ch)
					{
						lineIndex = line;
						glyphIndex = glyph;

						return true;
					}

					++ch;
					++glyph;
				}

				++line;
			}

			return false;
		}

		public int? GetCharIndexByPosition(int lineIndex, int glyphIndex)
		{
			var strings = Strings;

			var line = 0;

			var ch = 0;
			foreach (var si in strings)
			{
				var glyph = 0;
				foreach (var gr in si.GlyphRenders)
				{
					if (line == lineIndex && glyphIndex == glyph)
					{
						return ch;
					}

					++glyph;
					++ch;
				}

				++line;
			}

			return null;
		}

		public GlyphRender GetGlyphRenderByIndex(int charIndex)
		{
			var strings = Strings;

			var i = 0;
			foreach (var si in strings)
			{
				foreach (var gr in si.GlyphRenders)
				{
					if (charIndex == i)
					{
						return gr;
					}

					++i;
				}
			}

			return null;
		}
	}
}