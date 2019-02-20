using System;
using System.ComponentModel;
using System.Linq;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using StbTextEditSharp;
using Myra.Graphics2D.Text;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
using Xenko.Input;
#endif

namespace Myra.Graphics2D.UI
{
	public class TextField : Widget, ITextEditHandler
	{
		private DateTime _lastBlinkStamp = DateTime.Now;
		private bool _cursorOn = true;
		private bool _wrap = false;
		private readonly TextEdit _textEdit;
		public readonly FormattedText _formattedText = new FormattedText();
		private readonly StringBuilder _stringBuilder = new StringBuilder();
		private bool _isTouchDown;

		[EditCategory("Appearance")]
		[DefaultValue(0)]
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
		[XmlIgnore]
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

		[EditCategory("Appearance")]
		[DefaultValue(false)]
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
		[DefaultValue(null)]
		[Obsolete]
		public Color? MessageTextColor { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable Cursor { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable Selection { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(450)]
		public int BlinkIntervalInMs { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public bool Multiline
		{
			get
			{
				return !_textEdit.SingleLine;
			}

			set
			{
				_textEdit.SingleLine = !value;
			}
		}

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public bool Readonly { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(VerticalAlignment.Top)]
		public VerticalAlignment TextVerticalAlignment { get; set; }

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
					_textEdit.CursorIndex = string.IsNullOrEmpty(Text) ? 0 : Text.Length;
				}
			}
		}

		[DefaultValue(true)]
		public override bool ClipToBounds
		{
			get { return base.ClipToBounds; }
			set { base.ClipToBounds = value; }
		}

		[DefaultValue(true)]
		public override bool CanFocus
		{
			get { return base.CanFocus; }
			set { base.CanFocus = value; }
		}

		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}

		[HiddenInEditor]
		[XmlIgnore]
		public Func<string, string> InputFilter { get; set; }

		public int Length
		{
			get
			{
				return Text != null ? Text.Length : 0;
			}
		}

		/// <summary>
		/// Fires every time when the text had been changed
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<string>> TextChanged;

		/// <summary>
		/// Fires every time when the text had been changed by user(doesnt fire if it had been assigned through code)
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<string>> TextChangedByUser;

		public TextField(TextFieldStyle style)
		{
			_textEdit = new TextEdit(this);

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;

			CanFocus = true;
			ClipToBounds = true;

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

			var oldValue = _formattedText.Text;
			_formattedText.Text = value;
			InvalidateMeasure();

			var ev = TextChanged;
			if (ev != null)
			{
				ev(this, new ValueChangedEventArgs<string>(oldValue, value));
			}

			if (byUser)
			{
				ev = TextChangedByUser;
				if (ev != null)
				{
					ev(this, new ValueChangedEventArgs<string>(oldValue, value));
				}
			}

			return true;
		}

		private bool IsShiftDown
		{
			get
			{
				return Desktop.DownKeys.Contains(Keys.LeftShift) || Desktop.DownKeys.Contains(Keys.RightShift);
			}
		}

		private bool IsControlDown
		{
			get
			{
				return Desktop.DownKeys.Contains(Keys.LeftControl) || Desktop.DownKeys.Contains(Keys.RightControl);
			}
		}

		private ControlKeys ApplyShiftOrNone(ControlKeys k)
		{
			if (IsShiftDown)
			{
				k |= ControlKeys.Shift;
			}

			return k;
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (!AcceptsInput)
			{
				return;
			}

			ControlKeys? controlKey = null;

			switch (k)
			{
				case Keys.Insert:
					controlKey = ControlKeys.InsertMode;
					break;

				case Keys.Z:
					if (IsControlDown)
					{
						controlKey = ControlKeys.Undo;
					}
					break;

				case Keys.Y:
					if (IsControlDown)
					{
						controlKey = ControlKeys.Redo;
					}
					break;

				case Keys.Left:
					if (IsShiftDown && IsControlDown)
					{
						controlKey = ControlKeys.Shift | ControlKeys.WordLeft;
					}
					else if (IsShiftDown)
					{
						controlKey = ControlKeys.Shift | ControlKeys.Left;
					}
					else if (IsControlDown)
					{
						controlKey = ControlKeys.WordLeft;
					}
					else
					{
						controlKey = ControlKeys.Left;
					}
					break;

				case Keys.Right:
					if (IsShiftDown && IsControlDown)
					{
						controlKey = ControlKeys.Shift | ControlKeys.WordRight;
					}
					else if (IsShiftDown)
					{
						controlKey = ControlKeys.Shift | ControlKeys.Right;
					}
					else if (IsControlDown)
					{
						controlKey = ControlKeys.WordRight;
					}
					else
					{
						controlKey = ControlKeys.Right;
					}

					break;

				case Keys.Up:
					controlKey = ApplyShiftOrNone(ControlKeys.Up);
					break;

				case Keys.Down:
					controlKey = ApplyShiftOrNone(ControlKeys.Down);
					break;

				case Keys.Back:
					controlKey = ApplyShiftOrNone(ControlKeys.BackSpace);
					break;

				case Keys.Delete:
					controlKey = ApplyShiftOrNone(ControlKeys.Delete);
					break;

				case Keys.Home:
					if (IsShiftDown && IsControlDown)
					{
						controlKey = ControlKeys.Shift | ControlKeys.TextStart;
					}
					else if (IsShiftDown)
					{
						controlKey = ControlKeys.Shift | ControlKeys.LineStart;
					}
					else if (IsControlDown)
					{
						controlKey = ControlKeys.TextStart;
					}
					else
					{
						controlKey = ControlKeys.LineStart;
					}

					break;

				case Keys.End:
					if (IsShiftDown && IsControlDown)
					{
						controlKey = ControlKeys.Shift | ControlKeys.TextEnd;
					}
					else if (IsShiftDown)
					{
						controlKey = ControlKeys.Shift | ControlKeys.LineEnd;
					}
					else if (IsControlDown)
					{
						controlKey = ControlKeys.TextEnd;
					}
					else
					{
						controlKey = ControlKeys.LineEnd;
					}

					break;

				case Keys.Enter:
					_textEdit.InputChar('\n');
					break;
			}

			if (controlKey != null)
			{
				_textEdit.Key(controlKey.Value);
			}
		}

		public override void OnChar(char c)
		{
			base.OnChar(c);

			if (!char.IsControl(c))
			{
				_textEdit.InputChar(c);
			}
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			var mousePos = Desktop.MousePosition;
			_textEdit.Click(mousePos.X, mousePos.Y);
			_isTouchDown = true;
		}

		public override void OnTouchUp()
		{
			base.OnTouchUp();

			_isTouchDown = false;
		}

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			if (!_isTouchDown)
			{
				return;
			}

			var mousePos = Desktop.MousePosition;
			_textEdit.Drag(mousePos.X, mousePos.Y);
		}

		private Point GetRenderPositionByIndex(int index)
		{
			var bounds = ActualBounds;

			var x = bounds.X;
			var y = bounds.Y;

			if (Text != null)
			{
				if (index < Text.Length)
				{
					var glyphRender = _formattedText.GetGlyphInfoByIndex(index);
					if (glyphRender != null)
					{
						x += glyphRender.Bounds.Left;
						y += glyphRender.TextLine.Top;
					}
				}
				else if (_formattedText.Strings != null && _formattedText.Strings.Length > 0)
				{
					// After last glyph
					var lastLine = _formattedText.Strings[_formattedText.Strings.Length - 1];
					if (lastLine.Count > 0)
					{
						var glyphRender = lastLine.GetGlyphInfoByIndex(lastLine.Count - 1);

						x += glyphRender.Bounds.Right;
						y += glyphRender.TextLine.Top;
					}
				}
			}

			++x;

			return new Point(x, y);
		}

		public override void InternalRender(RenderContext context)
		{
			if (_formattedText.Font == null)
			{
				return;
			}

			var bounds = ActualBounds;

			if (Selection != null)
			{
				var selectStart = Math.Min(_textEdit.SelectStart, _textEdit.SelectEnd);
				var selectEnd = Math.Max(_textEdit.SelectStart, _textEdit.SelectEnd);

				if (selectStart < selectEnd)
				{
//					Debug.WriteLine("{0} - {1}", selectStart, selectEnd);

					var startGlyph = _formattedText.GetGlyphInfoByIndex(selectStart);
					var lineIndex = startGlyph.TextLine.LineIndex;
					var i = selectStart;

					while (true)
					{
						startGlyph = _formattedText.GetGlyphInfoByIndex(i);
						var startPosition = GetRenderPositionByIndex(i);

						if (selectEnd < i + startGlyph.TextLine.Count)
						{
							var endPosition = GetRenderPositionByIndex(selectEnd);

							context.Draw(Selection,
								new Rectangle(startPosition.X,
									startPosition.Y,
									endPosition.X - startPosition.X,
									CrossEngineStuff.LineSpacing(_formattedText.Font)));

							break;
						}

						context.Draw(Selection, 
							new Rectangle(startPosition.X,
								startPosition.Y,
								bounds.Left + startGlyph.TextLine.Size.X - startPosition.X,
								CrossEngineStuff.LineSpacing(_formattedText.Font)));

						++lineIndex;
						if (lineIndex >= _formattedText.Strings.Length)
						{
							break;
						}

						i = 0;
						for (var k = 0; k < lineIndex; ++k)
						{
							i += _formattedText.Strings[k].Count;
						}
					}
				}
			}

			var textColor = TextColor;
			if (!Enabled && DisabledTextColor != null)
			{
				textColor = DisabledTextColor.Value;
			} else if (IsFocused && FocusedTextColor != null)
			{
				textColor = FocusedTextColor.Value;
			}

			var centeredBounds = LayoutUtils.Align(new Point(bounds.Width, bounds.Height), _formattedText.Size, HorizontalAlignment.Left, TextVerticalAlignment);
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
				var pos = GetRenderPositionByIndex(_textEdit.CursorIndex);
				context.Draw(Cursor, new Rectangle(pos.X,
					pos.Y,
					Cursor.Size.X,
					CrossEngineStuff.LineSpacing(_formattedText.Font)));
			}
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			if (Font == null)
			{
				return Point.Zero;
			}

			var width = availableSize.X;
			if (Width != null && Width.Value < width)
			{
				width = Width.Value;
			}

			var result = Point.Zero;
			if (Font != null)
			{
				result = _formattedText.Measure(_wrap ? width : default(int?));
			}

			if (result.Y < CrossEngineStuff.LineSpacing(Font))
			{
				result.Y = CrossEngineStuff.LineSpacing(Font);
			}

			return result;
		}

		public override void Arrange()
		{
			base.Arrange();

			_formattedText.Width = _wrap ? ActualBounds.Width : default(int?);
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
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyTextFieldStyle(stylesheet.TextFieldStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.TextFieldStyles.Keys.ToArray();
		}

		public TextEditRow LayoutRow(int startIndex)
		{
			var bounds = ActualBounds;
			var r = _formattedText.LayoutRow(startIndex, bounds.Width);

			r.x0 += bounds.X;
			r.x1 += bounds.X;
			r.ymin += bounds.Y;
			r.ymax += bounds.Y;

			return r;
		}

		public float GetWidth(int index)
		{
			var glyph = _formattedText.GetGlyphInfoByIndex(index);

			if (glyph == null)
			{
				return 0;
			}

			return glyph.Bounds.Width;
		}
	}
}