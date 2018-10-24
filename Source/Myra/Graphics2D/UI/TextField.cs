using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Attributes;
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
			set { SetText(value, false); }
		}

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public SpriteFont Font
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
		[Obsolete]
		public SpriteFont MessageFont { get; set; }

		[EditCategory("Appearance")]
		[DefaultValue(true)]
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
		public Color? DisabledTextColor { get; set; }

		[EditCategory("Appearance")]
		public Color? FocusedTextColor { get; set; }

		[EditCategory("Appearance")]
		[Obsolete]
		public Color? MessageTextColor { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public Drawable Cursor { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public Drawable Selection { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(450)]
		public int BlinkIntervalInMs { get; set; }

		[EditCategory("Behavior")]
		public bool Multiline { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public bool Readonly { get; set; }

		private bool AcceptsInput
		{
			get
			{
				return Enabled && !Readonly;
			}
		}

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
		public Func<string, string> InputFilter { get; set; }

		/// <summary>
		/// Fires every time when the text had been changed
		/// </summary>
		public event EventHandler TextChanged;

		/// <summary>
		/// Fires every time when the text had been changed by user(doesnt fire if it had been assigned through code)
		/// </summary>
		public event EventHandler TextChangedByUser;

		public TextField(TextFieldStyle style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;

			CanFocus = true;
			ClipToBounds = true;
			_formattedText.IsColored = false;

			if (style != null)
			{
				ApplyTextFieldStyle(style);
			}

			BlinkIntervalInMs = 450;
		}

		public TextField(string style)
			: this(Stylesheet.Current.TextFieldStyles[style])
		{
		}

		public TextField() : this(Stylesheet.Current.TextFieldStyle)
		{
		}

		private bool SetText(string value, bool byUser)
		{
			if (value == _formattedText.Text)
			{
				return false;
			}

			// Filter check
			var f = InputFilter;
			if (f != null)
			{
				value = f(value);
				if (value == null)
				{
					return false;
				}
			}

			_formattedText.Text = value;
			InvalidateMeasure();

			var ev = TextChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}

			if (byUser)
			{
				ev = TextChangedByUser;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}

			return true;
		}

		private void ProcessChar(char ch)
		{
			if (!AcceptsInput)
			{
				return;
			}

			EnsureIndexInRange();

			var sb = new StringBuilder();

			var text = Text ?? "";

			sb.Append(text.Substring(0, _cursorIndex));
			sb.Append(ch);
			sb.Append(text.Substring(_cursorIndex));

			var nextText = sb.ToString();
			if (SetText(nextText, true))
			{
				_cursorIndex++;
			}
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (!AcceptsInput)
			{
				return;
			}

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

						if (SetText(sb.ToString(), true) && _cursorIndex > 0)
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

				/*				case Keys.Up:
								{
									if (!Multiline)
									{
										break;
									}

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
									if (!Multiline)
									{
										break;
									}

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
									break;*/

				case Keys.Home:
					_cursorIndex = 0;
					break;

				case Keys.Enter:
					{
						if (!Multiline)
						{
							break;
						}

						// Insert line break
						var sb = new StringBuilder();
						sb.Append(Text.Substring(0, _cursorIndex));
						sb.Append('\n');
						sb.Append(Text.Substring(_cursorIndex));

						if (SetText(sb.ToString(), true))
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

					var ch = k.ToChar(Desktop.KeyboardState.IsKeyDown(Keys.LeftShift) || Desktop.KeyboardState.IsKeyDown(Keys.RightShift));

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

			_cursorIndex = glyphRender.Index;

			if (glyphRender.TextRun.RenderedPosition != null &&
				mousePos.X >= glyphRender.TextRun.RenderedPosition.Value.X + glyphRender.Bounds.X + glyphRender.Bounds.Width / 2)
			{
				++_cursorIndex;
			}
		}

		public override void InternalRender(RenderContext context)
		{
			if (_formattedText.Font == null)
			{
				return;
			}

			var bounds = ActualBounds;

			var textColor = TextColor;
			if (!Enabled && DisabledTextColor != null)
			{
				textColor = DisabledTextColor.Value;
			} else if (IsFocused && FocusedTextColor != null)
			{
				textColor = FocusedTextColor.Value;
			}

			var centeredBounds = LayoutUtils.Align(bounds.Size, _formattedText.Size, HorizontalAlignment.Left, VerticalAlignment.Center);
			centeredBounds.Offset(bounds.Location);
			_formattedText.Draw(context.Batch, centeredBounds, textColor, context.Opacity);

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
				var glyphRender = _formattedText.GetGlyphInfoByIndex(_cursorIndex - 1);
				if (glyphRender != null && glyphRender.TextRun.RenderedPosition != null)
				{
					x = glyphRender.TextRun.RenderedPosition.Value.X + glyphRender.Bounds.Right;
					y = glyphRender.TextRun.RenderedPosition.Value.Y;
				}

				context.Draw(Cursor, new Rectangle(x,
					y,
					Cursor.Size.X,
					_formattedText.Font.LineSpacing));
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

			if (result.Y < Font.LineSpacing)
			{
				result.Y = Font.LineSpacing;
			}

			return result;
		}

		public override void Arrange()
		{
			base.Arrange();

			_formattedText.Width = _wrap ? Bounds.Width : default(int?);
		}

		public void ApplyTextFieldStyle(TextFieldStyle style)
		{
			ApplyWidgetStyle(style);

			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			FocusedTextColor = style.FocusedTextColor;

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

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyTextFieldStyle(stylesheet.TextFieldStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.TextFieldStyles.Keys.ToArray();
		}
	}
}