using System;
using System.Collections.Generic;
using System.Text;
using Myra.Utility;
using StbTextEditSharp;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.Text
{
	public class FormattedTextBase<T> where T: TextLine
	{
		public const int NewLineWidth = 0;

		private SpriteFont _font;
		private string _text = string.Empty;
		private int _verticalSpacing;
		private int? _width;
		private T[] _strings;
		private Point _size;
		private bool _dirty = true;
		private StringBuilder _stringBuilder = new StringBuilder();
		private readonly Func<SpriteFont, string, Point, T> _lineCreator;

		public SpriteFont Font
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
			Vector2? lastBreakMeasure = null;

			for (var i = startIndex; i < _text.Length; ++i)
			{
				var c = Text[i];

				_stringBuilder.Append(c);

				Vector2 sz =Vector2.Zero;

				if (c != '\n')
				{
					sz = Font.MeasureString(_stringBuilder);
				}
				else
				{
					sz = new Vector2(r.x1 + NewLineWidth, Math.Max(r.ymax, CrossEngineStuff.LineSpacing(_font)));
				}

				if (width != null && c == '\n')
				{
					// Break right here
					++r.num_chars;
					r.x1 = sz.X;
					r.ymax = sz.Y;
					r.baseline_y_delta = sz.Y;
					break;
				}

				if (width != null && sz.X > width.Value)
				{
					if (lastBreakPosition != null)
					{
						r.num_chars = lastBreakPosition.Value - startIndex;
					}

					if (lastBreakMeasure != null)
					{
						r.x1 = lastBreakMeasure.Value.X;
						r.ymax = lastBreakMeasure.Value.Y;
						r.baseline_y_delta = lastBreakMeasure.Value.Y;
					}

					break;
				}

				if (char.IsWhiteSpace(c))
				{
					lastBreakPosition = i + 1;
					lastBreakMeasure = sz;
				}

				++r.num_chars;
				r.x1 = sz.X;
				r.ymax = sz.Y;
				r.baseline_y_delta = sz.Y;
			}

			return r;
		}

		public Point Measure(int? width)
		{
			var result = Point.Zero;
			if (_text != null)
			{
				var i = 0;

				float y = 0;
				while (i < _text.Length)
				{
					var r = LayoutRow(i, width);

					if (r.num_chars == 0)
					{
						break;
					}

					if (r.x1 > result.X)
					{
						result.X = (int)r.x1;
					}

					i += r.num_chars;

					y += r.ymax;
					y += _verticalSpacing;
				}

				result.Y = (int)y;
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

				if (r.num_chars == 0)
				{
					break;
				}

				var line = _lineCreator(_font, _text.Substring(i, r.num_chars), new Point((int)(r.x1 - r.x0), (int)(r.baseline_y_delta)));
				lines.Add(line);

				i += r.num_chars;
			}

			_strings = lines.ToArray();

			// Calculate size
			_size = Point.Zero;
			var y = 0;
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