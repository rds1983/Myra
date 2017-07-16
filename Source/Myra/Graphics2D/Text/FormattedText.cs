using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Myra.Utility;

namespace Myra.Graphics2D.Text
{
	public class FormattedText
	{
		public struct Options
		{
			public int? Width;
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
				public readonly StringBuilder text = new StringBuilder();
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
			private readonly List<GlyphRun> _result = new List<GlyphRun>();
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
				_lastRun = new GlyphRun(_font);

				_fullString.Clear();
			}

			public GlyphRun[] Format(BitmapFont font, string text, Options options)
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
						} else if (c == ' ')
						{
							_lexeme.AppendChar(c, pos);
						}
						else if (c > ' ')
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
				_lastRun = new GlyphRun(_font);
				_fullString.Clear();

				foreach (var li in _lexemes)
				{
					switch (li.type)
					{
						case LexemeType.LineBreak:
							_lastRun.Append(li.chars, _currentColor);
							StoreRun();
							break;
						case LexemeType.Space:
						case LexemeType.Word:
							_fullString.Append(li.text);

							if (options.Width.HasValue)
							{
								var sz = font.MeasureString(_fullString.ToString());
								if (sz.Width <= options.Width.Value)
								{
									_lastRun.Append(li.chars, _currentColor);
								}
								else
								{
									if (_lastRun.Count > 0)
									{
										StoreRun();
									}

									if (li.type == LexemeType.Word)
									{
										_fullString.Append(li.text);
										_lastRun.Append(li.chars, _currentColor);
									}
								}
							}
							else
							{
								_lastRun.Append(li.chars, _currentColor);
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
		private int? _width;
		private GlyphRun[] _strings;
		private Point _size;
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

		public int? Width
		{
			get { return _width; }
			set
			{
				if (value == _width)
				{
					return;
				}

				_width = value;
				InvalidateLayout();
			}
		}

		public bool IsColored
		{
			get { return _colored; }
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

		public Point Size
		{
			get
			{
				Update();
				return _size;
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
				Width = _width
			};

			_strings = parser.Format(_font, _text, options);

			// Calculate size
			_size = Point.Zero;
			for (var i = 0; i < _strings.Length; ++i)
			{
				var si = _strings[i];

				if (si.Size.X > _size.X)
				{
					_size.X = si.Size.X;
				}

				_size.Y += si.Size.Y;

				if (i < _strings.Length - 1)
				{
					_size.Y += _verticalSpacing;
				}
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

				if (y >= bounds.Top && y < bounds.Bottom)
				{
					si.Draw(batch, new Point(bounds.Left, y), bounds.Width, textColor);
				}

				y += si.Size.Y;
				y += _verticalSpacing;
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
					if (gr.RenderedBounds.HasValue && gr.RenderedBounds.Value.Contains(pos))
					{
						return gr;
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
			if (Text != null && charIndex >= Text.Length)
			{
				charIndex = Text.Length - 1;
			}

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

		public FormattedText Clone()
		{
			return new FormattedText
			{
				IsColored = IsColored,
				Font = Font,
				Text = Text,
				VerticalSpacing = VerticalSpacing,
				Width = Width
			};
		}
	}
}