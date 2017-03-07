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

		[EditCategory("Appearance")]
		public int VerticalSpacing
		{
			get { return _formattedText.VerticalSpacing; }
			set
			{
				_formattedText.VerticalSpacing = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Appearance")]
		public string Text
		{
			get { return _formattedText.Text; }
			set { SetText(value); }
		}

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public BitmapFont Font
		{
			get { return _formattedText.Font; }
			set
			{
				_formattedText.Font = value;
				InvalidateMeasure();
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public BitmapFont MessageFont { get; set; }

		[EditCategory("Appearance")]
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
				InvalidateMeasure();
			}
		}

		[EditCategory("Appearance")]
		public Color TextColor { get; set; }

		[EditCategory("Appearance")]
		public Color DisabledTextColor { get; set; }

		[EditCategory("Appearance")]
		public Color FocusedTextColor { get; set; }

		[EditCategory("Appearance")]
		public Color MessageTextColor { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public Drawable FocusedBackground { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public Drawable Cursor { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public Drawable Selection { get; set; }

		[EditCategory("Behavior")]
		public int BlinkIntervalInMs { get; set; }

		public override bool IsFocused
		{
			get { return base.IsFocused; }
			internal set
			{
				base.IsFocused = value;

				if (base.IsFocused)
				{
					_cursorIndex = string.IsNullOrEmpty(Text) ? 0 : Text.Length;
				}
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Func<string, bool> InputFilter { get; set; }

		public event EventHandler TextChanged;

		public TextField(TextFieldStyle style)
		{
			CanFocus = true;
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

		private bool SetText(string value)
		{
			if (value == _formattedText.Text)
			{
				return false;
			}

			// Filter check
			var f = InputFilter;
			if (f != null && !f(value))
			{
				return false;
			}

			_formattedText.Text = value;
			InvalidateMeasure();

			var ev = TextChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}

			return true;
		}

		private void ProcessChar(char ch)
		{
			EnsureIndexInRange();

			var sb = new StringBuilder();
			sb.Append(Text.Substring(0, _cursorIndex));
			sb.Append(ch);
			sb.Append(Text.Substring(_cursorIndex));

			var nextText = sb.ToString();
			if (SetText(nextText))
			{
				_cursorIndex++;
			}
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.Back:
				{
					EnsureIndexInRange();
					var sb = new StringBuilder();

					if (_cursorIndex > 0)
					{
						sb.Append(Text.Substring(0, _cursorIndex - 1));
					}
					sb.Append(Text.Substring(_cursorIndex));

					if (SetText(sb.ToString()) && _cursorIndex > 0)
					{
						--_cursorIndex;
					}
				}
					break;

				case Keys.Delete:
				{

					EnsureIndexInRange();

					var sb = new StringBuilder();
					sb.Append(Text.Substring(0, _cursorIndex));

					if (_cursorIndex + 1 < Text.Length)
					{
						sb.Append(Text.Substring(_cursorIndex + 1));
					}

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

					if (SetText(sb.ToString()))
					{
						++_cursorIndex;
					}
				}
					break;

				default:
					if (Desktop == null)
					{
						break;
					}

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
				if (mousePos.X >= glyphRender.RenderedBounds.Value.X + glyphRender.RenderedBounds.Value.Width/2)
				{
					_cursorIndex = _cursorIndex + 1;
				}
			}
		}

		public override void InternalRender(SpriteBatch batch)
		{
			if (_formattedText.Font == null)
			{
				return;
			}

			var bounds = RenderBounds;
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

		public void EnsureIndexInRange()
		{
			if (_cursorIndex < 0)
			{
				_cursorIndex = 0;
			}

			if (Text != null)
			{
				if (_cursorIndex > Text.Length)
				{
					_cursorIndex = Text.Length;
				}
			}
			else
			{
				_cursorIndex = 0;
			}
		}
	}
}