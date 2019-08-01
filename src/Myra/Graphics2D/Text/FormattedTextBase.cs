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
	public class FormattedTextBase<T> where T : TextLine
	{
		public const int NewLineWidth = 0;

		private SpriteFont _font;
		private string _text = string.Empty;
		private string _displayText = string.Empty;
		private int _verticalSpacing;
		private int? _width;
		private T[] _strings;
		private Point _size;
		private bool _dirty = true;
		private StringBuilder _stringBuilder = new StringBuilder();
		private readonly Func<SpriteFont, string, Point, T> _lineCreator;

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
			}
		}

		public bool IsPassword
		{
			get; set;
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
				_displayText = IsPassword ? new string('*', _text.Length) : _text;
				InvalidateLayout();
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

		public T[] Strings
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

		public FormattedTextBase(Func<SpriteFont, string, Point, T> creator)
		{
			if (creator == null)
			{
				throw new ArgumentNullException("creator");
			}

			_lineCreator = creator;
		}

		internal TextEditRow LayoutRow(int startIndex, int? width)
		{
			var r = new TextEditRow();

			if (string.IsNullOrEmpty(_text))
			{
				return r;
			}

			_stringBuilder.Clear();
			int? lastBreakPosition = null;
			Point? lastBreakMeasure = null;

			for (var i = startIndex; i < _text.Length; ++i)
			{
				var c = _displayText[i];

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
						r.CharsCount = lastBreakPosition.Value - startIndex;
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

		public Point Measure(int? width)
		{
			var result = Point.Zero;
			if (_text != null)
			{
				var i = 0;
				var y = 0;
				while (i < _text.Length)
				{
					var r = LayoutRow(i, width);

					if (r.CharsCount == 0)
					{
						break;
					}

					if (r.X > result.X)
					{
						result.X = r.X;
					}

					i += r.CharsCount;

					y += r.Y;
					y += _verticalSpacing;
				}

				result.Y = y;
			}

			if (result.Y == 0)
			{
				result.Y = CrossEngineStuff.LineSpacing(_font);
			}

			return result;
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			if (string.IsNullOrEmpty(_text))
			{
				_strings = new T[0];
				_dirty = false;
				return;
			}

			var lines = new List<T>();

			var i = 0;
			while (i < _text.Length)
			{
				var r = LayoutRow(i, Width);

				if (r.CharsCount == 0)
				{
					break;
				}

				var line = _lineCreator(_font, _displayText.Substring(i, r.CharsCount), new Point(r.X, r.Y));
				line.LineStart = i;
				lines.Add(line);

				i += r.CharsCount;
			}

			_strings = lines.ToArray();

			// Calculate size
			_size = Point.Zero;
			for (i = 0; i < _strings.Length; ++i)
			{
				var line = _strings[i];

				line.LineIndex = i;
				line.Top = _size.Y;

				if (line.Size.X > _size.X)
				{
					_size.X = line.Size.X;
				}

				_size.Y += line.Size.Y;

				if (i < _strings.Length - 1)
				{
					_size.Y += _verticalSpacing;
				}
			}

			_dirty = false;
		}

		public T GetLineByCursorPosition(int cursorPosition)
		{
			Update();

			if (_strings.Length == 0)
			{
				return null;
			}

			if (cursorPosition < 0)
			{
				return _strings[0];
			}

			for(var i = 0; i < _strings.Length; ++i)
			{
				var s = _strings[i];
				if (s.LineStart <= cursorPosition && cursorPosition < s.LineStart + s.Text.Length())
				{
					return s;
				}
			}

			return _strings[_strings.Length - 1];
		}

		public T GetLineByY(int y)
		{
			if (string.IsNullOrEmpty(Text) || y < 0)
			{
				return null;
			}

			Update();

			for (var i = 0; i < _strings.Length; ++i)
			{
				var s = _strings[i];

				if (s.Top <= y && y < s.Top + s.Size.Y)
				{
					return s;
				}
			}

			return null;
		}

		public void Draw(SpriteBatch batch, Point position, Rectangle clip, Color textColor, float opacity = 1.0f)
		{
			var strings = Strings;

			if (strings == null || strings.Length == 0)
			{
				return;
			}

			var y = position.Y;

			foreach (var si in strings)
			{
				if (y + si.Size.Y >= clip.Top && y <= clip.Bottom)
				{
					si.Draw(batch, new Point(position.X, y), textColor, opacity);
				}

				y += si.Size.Y;
				y += _verticalSpacing;
			}
		}

		private void InvalidateLayout()
		{
			_dirty = true;
		}
	}
}