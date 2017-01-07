using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Edit;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class TextField : Widget
	{
		private DateTime _lastBlinkStamp = DateTime.Now;
		private bool _cursorOn = true;
		private readonly FormattedText _formattedText = new FormattedText();
		private int _cursorIndex;
		private bool _wrap = true;

		public int VerticalSpacing
		{
			get { return _formattedText.VerticalSpacing; }
			set
			{
				_formattedText.VerticalSpacing = value;
				FireMeasureChanged();
			}
		}

		public string Text
		{
			get { return _formattedText.Text; }
			set
			{
				if (value == _formattedText.Text)
				{
					return;
				}

				_formattedText.Text = value;
				FireMeasureChanged();

				var ev = TextChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public BitmapFont Font
		{
			get { return _formattedText.Font; }
			set
			{
				_formattedText.Font = value;
				FireMeasureChanged();
			}
		}

		public BitmapFont MessageFont { get; set; }

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
				FireMeasureChanged();
			}
		}

		public Color TextColor { get; set; }
		public Color DisabledTextColor { get; set; }
		public Color FocusedTextColor { get; set; }
		public Color MessageTextColor { get; set; }

		public Drawable FocusedBackground { get; set; }
		public Drawable Cursor { get; set; }
		public Drawable Selection { get; set; }

		public int BlinkIntervalInMs { get; set; }

		public override bool CanFocus
		{
			get { return true; }
		}


		public event EventHandler TextChanged;

		public TextField(TextFieldStyle style)
		{
			_formattedText.IsColored = false;

			if (style != null)
			{
				ApplyTextFieldStyle(style);
			}

			BlinkIntervalInMs = 450;
		}

		public TextField() : this(DefaultAssets.UIStylesheet.TextFieldStyle)
		{
		}

		private void ProcessChar(char ch)
		{
			var sb = new StringBuilder();
			sb.Append(Text.Substring(0, _cursorIndex));
			sb.Append(ch);
			sb.Append(Text.Substring(_cursorIndex));

			Text = sb.ToString();

			_cursorIndex++;
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.Back:
				{
					var sb = new StringBuilder();
					if (_cursorIndex > 0)
					{
						sb.Append(Text.Substring(0, _cursorIndex - 1));
					}
					sb.Append(Text.Substring(_cursorIndex));

					Text = sb.ToString();

					if (_cursorIndex > 0)
					{
						--_cursorIndex;
					}
				}
					break;

				case Keys.Delete:
				{
					var sb = new StringBuilder();
					sb.Append(Text.Substring(0, _cursorIndex));
					sb.Append(Text.Substring(_cursorIndex + 1));

					Text = sb.ToString();
				}
					break;

				case Keys.Left:
					if (_cursorIndex > 0)
					{
						--_cursorIndex;
					}
					break;

				case Keys.Right:
					if (_cursorIndex < Text.Length)
					{
						++_cursorIndex;
					}
					break;

				case Keys.Up:
				{
					int lineIndex, glyphIndex;
					if (!_formattedText.GetPositionByCharIndex(_cursorIndex, out lineIndex, out glyphIndex))
					{
						break;
					}

					if (lineIndex <= 0)
					{
						break;
					}

					// Move up
					--lineIndex;

					if (glyphIndex >= _formattedText.Strings[lineIndex].Count)
					{
						glyphIndex = _formattedText.Strings[lineIndex].Count - 1;
					}

					var pos = _formattedText.GetCharIndexByPosition(lineIndex, glyphIndex);
					_cursorIndex = pos ?? 0;
				}
					break;

				case Keys.Down:
				{
					int lineIndex, glyphIndex;
					if (!_formattedText.GetPositionByCharIndex(_cursorIndex, out lineIndex, out glyphIndex))
					{
						break;
					}

					if (lineIndex >= _formattedText.Strings.Length - 1)
					{
						break;
					}


					// Move down
					++lineIndex;

					if (glyphIndex >= _formattedText.Strings[lineIndex].Count)
					{
						glyphIndex = _formattedText.Strings[lineIndex].Count - 1;
					}

					var pos = _formattedText.GetCharIndexByPosition(lineIndex, glyphIndex);
					_cursorIndex = pos ?? 0;
				}
					break;

				case Keys.Home:
					_cursorIndex = 0;
					break;

				case Keys.Enter:
				{
					// Insert line break
					var sb = new StringBuilder();
					sb.Append(Text.Substring(0, _cursorIndex));
					sb.Append('\n');
					sb.Append(Text.Substring(_cursorIndex));

					Text = sb.ToString();

					++_cursorIndex;
				}
					break;

				default:
					var ch =
						k.ToChar(Desktop.KeyboardState.IsKeyDown(Keys.LeftShift) || Desktop.KeyboardState.IsKeyDown(Keys.RightShift));

					if (ch.HasValue)
					{
						ProcessChar(ch.Value);
					}
					break;
			}
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			var mousePos = Desktop.MousePosition;
			var glyphRender = _formattedText.Hit(mousePos);

			if (glyphRender == null)
			{
				return;
			}

			var index = _formattedText.GetIndexByGlyphRender(glyphRender);

			if (index.HasValue)
			{
				_cursorIndex = index.Value;
			}

			if (glyphRender.RenderedBounds.HasValue)
			{
				if (mousePos.X >= glyphRender.RenderedBounds.Value.X + glyphRender.RenderedBounds.Value.X/2)
				{
					_cursorIndex = _cursorIndex + 1;
				}
			}
		}

		public override void InternalRender(SpriteBatch batch, Rectangle bounds)
		{
			if (_formattedText.Font == null)
			{
				return;
			}

			_formattedText.Draw(batch, bounds, TextColor);

			if (!IsFocused)
			{
				// Skip cursor rendering if the widget doesnt have the focus
				return;
			}

			var now = DateTime.Now;
			if ((now - _lastBlinkStamp).TotalMilliseconds >= BlinkIntervalInMs)
			{
				_cursorOn = !_cursorOn;
				_lastBlinkStamp = now;
			}

			if (_cursorOn && Cursor != null)
			{
				var x = bounds.X;
				var y = bounds.Y;
				var glyphRender = _formattedText.GetGlyphRenderByIndex(_cursorIndex - 1);
				if (glyphRender != null && 
					glyphRender.RenderedBounds.HasValue && 
					glyphRender.Run.RenderedBounds.HasValue)
				{
					x = glyphRender.RenderedBounds.Value.Right;
					y = glyphRender.Run.RenderedBounds.Value.Top;
				}

				Cursor.Draw(batch, new Rectangle(x,
					y,
					Cursor.Size.X,
					_formattedText.Font.LineHeight));
			}
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			if (Font == null)
			{
				return Point.Zero;
			}

			var width = availableSize.X;
			if (WidthHint != null && WidthHint.Value < width)
			{
				width = WidthHint.Value;
			}

			var result = Point.Zero;
			if (Font != null)
			{
				var formattedText = _formattedText.Clone();
				formattedText.Width = _wrap ? width : default(int?);

				result = formattedText.Size;
			}

			if (result.Y < Font.LineHeight)
			{
				result.Y = Font.LineHeight;
			}

			return result;
		}

		public override void Arrange()
		{
			base.Arrange();

			_formattedText.Width = _wrap ? LayoutBounds.Width : default(int?);
		}

		public void ApplyTextFieldStyle(TextFieldStyle style)
		{
			ApplyWidgetStyle(style);

			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			FocusedBackground = style.FocusedBackground;
			MessageTextColor = style.MessageTextColor;

			FocusedBackground = style.FocusedBackground;
			Cursor = style.Cursor;
			Selection = style.Selection;

			Font = style.Font;
			MessageFont = style.MessageFont;
		}
	}
}