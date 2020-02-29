using System;
using System.Collections.Generic;
using System.Text;
using Myra.Utility;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.Text
{
	public class FormattedText
	{
		public const int NewLineWidth = 0;

		private SpriteFont _font;
		private string _text = string.Empty;
		private int _verticalSpacing;
		private int? _width;
		private readonly List<TextLine> _lines = new List<TextLine>();
		private bool _calculateGlyphs, _supportsCommands;
		private Point _size;
		private bool _dirty = true;
		private StringBuilder _stringBuilder = new StringBuilder();
		private readonly Dictionary<int, Point> _measures = new Dictionary<int, Point>();

		public SpriteFont Font
		{
			get
			{
				return _font;
			}
			set
			{
				if (value == _font)
				{
					return;
				}

				_font = value;
				InvalidateLayout();
				InvalidateMeasures();
			}
		}

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				if (value == _text)
				{
					return;
				}

				_text = value;
				InvalidateLayout();
				InvalidateMeasures();
			}
		}

		public int VerticalSpacing
		{
			get
			{
				return _verticalSpacing;
			}

			set
			{
				if (value == _verticalSpacing)
				{
					return;
				}

				_verticalSpacing = value;
				InvalidateLayout();
				InvalidateMeasures();
			}
		}

		public int? Width
		{
			get
			{
				return _width;
			}
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

		public List<TextLine> Lines
		{
			get
			{
				Update();
				return _lines;
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

		public bool CalculateGlyphs
		{
			get
			{
				return _calculateGlyphs;
			}

			set
			{
				if (value == _calculateGlyphs)
				{
					return;
				}

				_calculateGlyphs = value;
				InvalidateLayout();
				InvalidateMeasures();
			}
		}

		public bool SupportsCommands
		{
			get
			{
				return _supportsCommands;
			}

			set
			{
				if (value == _supportsCommands)
				{
					return;
				}

				_supportsCommands = value;
				InvalidateLayout();
				InvalidateMeasures();
			}
		}

		internal ChunkInfo LayoutRow(int startIndex, int? width, bool parseCommands)
		{
			var r = new ChunkInfo
			{
				StartIndex = startIndex,
				LineEnd = true
			};

			if (string.IsNullOrEmpty(_text))
			{
				return r;
			}

			_stringBuilder.Clear();
			int? lastBreakPosition = null;
			Point? lastBreakMeasure = null;

			for (var i = r.StartIndex; i < _text.Length; ++i)
			{
				var c = _text[i];

				if (SupportsCommands && c == '\\')
				{
					if (i < _text.Length - 2 && _text[i + 1] == 'c' && _text[i + 2] == '{')
					{
						// Find end
						var startPos = i + 3;
						var j = _text.IndexOf('}', startPos);

						if (j != -1)
						{
							// Found
							if (i > r.StartIndex)
							{
								// Break right here, as next chunk has another color
								r.LineEnd = false;
								return r;
							}

							if (parseCommands)
							{
								r.Color = ColorStorage.FromName(_text.Substring(startPos, j - startPos));
							}

							r.StartIndex = j + 1;
							i = j;
							continue;
						}
					}
				}

				_stringBuilder.Append(c);

				var sz = Point.Zero;

				if (c != '\n')
				{
					var v = Font.MeasureString(_stringBuilder);
					sz = new Point((int)v.X, (int)v.Y);
				}
				else
				{
					sz = new Point(r.X + NewLineWidth, Math.Max(r.Y, CrossEngineStuff.LineSpacing(_font)));

					// Break right here
					++r.CharsCount;
					r.X = sz.X;
					r.Y = sz.Y;
					break;
				}

				if (width != null && sz.X > width.Value)
				{
					if (lastBreakPosition != null)
					{
						r.CharsCount = lastBreakPosition.Value - r.StartIndex;
					}

					if (lastBreakMeasure != null)
					{
						r.X = lastBreakMeasure.Value.X;
						r.Y = lastBreakMeasure.Value.Y;
					}

					break;
				}

				if (char.IsWhiteSpace(c))
				{
					lastBreakPosition = i + 1;
					lastBreakMeasure = sz;
				}

				++r.CharsCount;
				r.X = sz.X;
				r.Y = sz.Y;
			}

			return r;
		}

		private static int GetMeasureKey(int? width)
		{
			return width != null ? width.Value : -1;
		}

		public Point Measure(int? width)
		{
			var result = Point.Zero;

			var key = GetMeasureKey(width);
			if (_measures.TryGetValue(key, out result))
			{
				return result;
			}

			if (!string.IsNullOrEmpty(_text))
			{
				var i = 0;
				var y = 0;

				var remainingWidth = width;
				var lineWidth = 0;
				while (i < _text.Length)
				{
					var chunkInfo = LayoutRow(i, remainingWidth, false);
					if (i == chunkInfo.StartIndex && chunkInfo.CharsCount == 0)
						break;

					lineWidth += chunkInfo.X;
					i = chunkInfo.StartIndex + chunkInfo.CharsCount;

					if (remainingWidth.HasValue)
					{
						remainingWidth = remainingWidth.Value - chunkInfo.X;
					}

					if (chunkInfo.LineEnd)
					{
						if (lineWidth > result.X)
						{
							result.X = lineWidth;
						}

						lineWidth = 0;
						remainingWidth = width;

						y += chunkInfo.Y;
						y += _verticalSpacing;
					}
				}

				result.Y = y;
			}

			if (result.Y == 0)
			{
				result.Y = CrossEngineStuff.LineSpacing(_font);
			}

			_measures[key] = result;

			return result;
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			_lines.Clear();

			if (string.IsNullOrEmpty(_text))
			{
				_dirty = false;
				return;
			}

			var i = 0;
			var line = new TextLine
			{
				TextStartIndex = i
			};

			var width = Width;
			while (i < _text.Length)
			{
				var c = LayoutRow(i, width, true);
				if (i == c.StartIndex && c.CharsCount == 0)
					break;

				var chunk = new TextChunk(_font, _text.Substring(c.StartIndex, c.CharsCount), new Point(c.X, c.Y), CalculateGlyphs)
				{
					TextStartIndex = i,
					Color = c.Color
				};

				width -= chunk.Size.X;

				i = c.StartIndex + c.CharsCount;

				line.Chunks.Add(chunk);
				line.Count += chunk.Count;

				line.Size.X += chunk.Size.X;
				if (chunk.Size.Y > line.Size.Y)
				{
					line.Size.Y = chunk.Size.Y;
				}

				if (c.LineEnd)
				{
					// New line
					_lines.Add(line);

					line = new TextLine
					{
						TextStartIndex = i
					};

					width = Width;
				}
			}

			// Calculate size
			_size = Point.Zero;
			for (i = 0; i < _lines.Count; ++i)
			{
				line = _lines[i];

				line.LineIndex = i;
				line.Top = _size.Y;

				for(var j = 0; j < line.Chunks.Count; ++j)
				{
					var chunk = line.Chunks[j];
					chunk.LineIndex = line.LineIndex;
					chunk.ChunkIndex = j;
					chunk.Top = line.Top;
				}

				if (line.Size.X > _size.X)
				{
					_size.X = line.Size.X;
				}

				_size.Y += line.Size.Y;

				if (i < _lines.Count - 1)
				{
					_size.Y += _verticalSpacing;
				}
			}

			var key = GetMeasureKey(Width);
			_measures[key] = _size;

			_dirty = false;
		}

		public TextLine GetLineByCursorPosition(int cursorPosition)
		{
			Update();

			if (_lines.Count == 0)
			{
				return null;
			}

			if (cursorPosition < 0)
			{
				return _lines[0];
			}

			for(var i = 0; i < _lines.Count; ++i)
			{
				var s = _lines[i];
				if (s.TextStartIndex <= cursorPosition && cursorPosition < s.TextStartIndex + s.Count)
				{
					return s;
				}
			}

			return _lines[_lines.Count - 1];
		}

		public TextLine GetLineByY(int y)
		{
			if (string.IsNullOrEmpty(_text) || y < 0)
			{
				return null;
			}

			Update();

			for (var i = 0; i < _lines.Count; ++i)
			{
				var s = _lines[i];

				if (s.Top <= y && y < s.Top + s.Size.Y)
				{
					return s;
				}
			}

			return null;
		}

		public GlyphInfo GetGlyphInfoByIndex(int charIndex)
		{
			var strings = Lines;

			foreach (var si in strings)
			{
				if (charIndex >= si.Count)
				{
					charIndex -= si.Count;
				}
				else
				{
					return si.GetGlyphInfoByIndex(charIndex);
				}
			}

			return null;
		}

		public void Draw(SpriteBatch batch, Point position, Rectangle clip, Color textColor, bool useChunkColor, float opacity = 1.0f)
		{
			var y = position.Y;
			foreach (var line in Lines)
			{
				if (y + line.Size.Y >= clip.Top && y <= clip.Bottom)
				{
					textColor = line.Draw(batch, new Point(position.X, y), textColor, useChunkColor, opacity);
				} else
				{
					foreach (var chunk in line.Chunks)
					{
						if (useChunkColor && chunk.Color != null)
						{
							textColor = chunk.Color.Value;
						}
					}
				}

				y += line.Size.Y;
				y += _verticalSpacing;
			}
		}

		private void InvalidateLayout()
		{
			_dirty = true;
		}

		private void InvalidateMeasures()
		{
			_measures.Clear();
		}
	}
}