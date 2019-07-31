using System;
using System.ComponentModel;
using System.Linq;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.Graphics2D.Text;
using System.Text;
using TextCopy;
using Myra.Graphics2D.UI.TextEdit;

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
	public class TextField : Widget
	{
		private DateTime _lastBlinkStamp = DateTime.Now;
		private bool _cursorOn = true;
		private bool _wrap = false;
		private readonly FormattedTextWithGlyphs _formattedText = new FormattedTextWithGlyphs();
		private readonly StringBuilder _stringBuilder = new StringBuilder();
		private bool _isTouchDown;
		private int? _lastCursorY;
		private int _cursorIndex;
		private bool _suppressRedoStackReset = false;

		private readonly UndoRedoStack UndoStack = new UndoRedoStack();
		private readonly UndoRedoStack RedoStack = new UndoRedoStack();

		[EditCategory("Appearance")]
		[DefaultValue(0)]
		public int VerticalSpacing
		{
			get
			{
				return _formattedText.VerticalSpacing;
			}
			set
			{
				_formattedText.VerticalSpacing = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Appearance")]
		public string Text
		{
			get
			{
				return _formattedText.Text;
			}
			set
			{
				SetText(value, false);
			}
		}

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public bool Multiline
		{
			get; set;
		}

		private string UserText
		{
			get
			{
				return _formattedText.Text;
			}

			set
			{
				SetText(value, true);
			}
		}

		private int Length
		{
			get
			{
				return _formattedText.Text.Length();
			}
		}

		private bool InsertMode
		{
			get; set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public SpriteFont Font
		{
			get
			{
				return _formattedText.Font;
			}
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
			get
			{
				return _wrap;
			}

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
		public Color TextColor
		{
			get; set;
		}

		[EditCategory("Appearance")]
		public Color? DisabledTextColor
		{
			get; set;
		}

		[EditCategory("Appearance")]
		public Color? FocusedTextColor
		{
			get; set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable Cursor
		{
			get; set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public IRenderable Selection
		{
			get; set;
		}

		[EditCategory("Behavior")]
		[DefaultValue(450)]
		public int BlinkIntervalInMs
		{
			get; set;
		}

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public bool Readonly
		{
			get; set;
		}

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public bool PasswordField
		{
			get
			{
				return _formattedText.IsPassword;
			}
			set
			{
				_formattedText.IsPassword = value;
			}
		}

		[EditCategory("Behavior")]
		[DefaultValue(VerticalAlignment.Top)]
		public VerticalAlignment TextVerticalAlignment
		{
			get; set;
		}

		[DefaultValue(true)]
		public override bool ClipToBounds
		{
			get
			{
				return base.ClipToBounds;
			}
			set
			{
				base.ClipToBounds = value;
			}
		}

		[DefaultValue(true)]
		public override bool AcceptsKeyboardFocus
		{
			get
			{
				return base.AcceptsKeyboardFocus;
			}
			set
			{
				base.AcceptsKeyboardFocus = value;
			}
		}

		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return base.HorizontalAlignment;
			}
			set
			{
				base.HorizontalAlignment = value;
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public Func<string, string> InputFilter
		{
			get; set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		public int CursorPosition
		{
			get
			{
				return _cursorIndex;
			}

			set
			{
				if (_cursorIndex == value)
				{
					return;
				}

				_cursorIndex = value;

				OnCursorIndexChanged();
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public Point CursorScreenPosition
		{
			get
			{
				return GetRenderPositionByIndex(CursorPosition);
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public int SelectStart
		{
			get; private set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		public int SelectEnd
		{
			get; private set;
		}

		/// <summary>
		/// Fires every time when the text had been changed
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<string>> TextChanged;

		/// <summary>
		/// Fires every time when the text had been changed by user(doesnt fire if it had been assigned through code)
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<string>> TextChangedByUser;

		public event EventHandler CursorPositionChanged;

		public TextField(TextFieldStyle style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;

			AcceptsKeyboardFocus = true;
			ClipToBounds = true;

			if (style != null)
			{
				ApplyTextFieldStyle(style);
			}

			BlinkIntervalInMs = 450;
		}

		public TextField(Stylesheet stylesheet, string style) : this(stylesheet.TextFieldStyles[style])
		{
		}

		public TextField(Stylesheet stylesheet) : this(stylesheet.TextFieldStyle)
		{
		}

		public TextField(string style) : this(Stylesheet.Current, style)
		{
		}

		public TextField() : this(Stylesheet.Current)
		{
		}

		private void DeleteChars(int pos, int l)
		{
			if (l == 0)
				return;

			UserText = UserText.Substring(0, pos) + UserText.Substring(pos + l);
		}

		private bool InsertChars(int pos, string s)
		{
			if (string.IsNullOrEmpty(s))
				return false;

			if (string.IsNullOrEmpty(Text))
				UserText = s;
			else
				UserText = UserText.Substring(0, pos) + s + UserText.Substring(pos);

			return true;
		}

		private bool InsertChar(int pos, char ch)
		{
			if (string.IsNullOrEmpty(Text))
				UserText = ch.ToString();
			else
				UserText = UserText.Substring(0, pos) + ch + UserText.Substring(pos);

			return true;
		}

		public void Insert(int where, string text)
		{
			text = Process(text);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}

			if (InsertChars(where, text))
			{
				UndoStack.MakeInsert(where, text.Length());
				CursorPosition += text.Length;
			}
		}

		public void Replace(int where, int len, string text)
		{
			if (len <= 0)
			{
				Insert(where, text);
				return;
			}

			text = Process(text);

			if (string.IsNullOrEmpty(text))
			{
				Delete(where, len);
				return;
			}

			UndoStack.MakeReplace(Text, where, len, text.Length);
			UserText = UserText.Substring(0, where) + text + UserText.Substring(where + len);
		}

		public void ReplaceAll(string text)
		{
			if (string.IsNullOrEmpty(Text))
			{
				Replace(0, 0, text);
			}
			else
			{
				Replace(0, Text.Length, text);
			}
		}

		private bool Delete(int where, int len)
		{
			if (where < 0 || where >= Length || len < 0)
			{
				return false;
			}

			UndoStack.MakeDelete(Text, where, len);
			DeleteChars(where, len);

			return true;
		}

		private void DeleteSelection()
		{
			if (SelectStart != SelectEnd)
			{
				if (SelectStart < SelectEnd)
				{
					Delete(SelectStart, SelectEnd - SelectStart);
					SelectEnd = CursorPosition = SelectStart;
				}
				else
				{
					Delete(SelectEnd, SelectStart - SelectEnd);
					SelectStart = CursorPosition = SelectEnd;
				}
			}
		}

		private bool Paste(string text)
		{
			DeleteSelection();
			if (InsertChars(CursorPosition, text))
			{
				UndoStack.MakeInsert(CursorPosition, text.Length());
				CursorPosition += text.Length;
				return true;
			}

			return false;
		}

		private void InputChar(char ch)
		{
			if (!Multiline && ch == '\n')
				return;

			if (InsertMode && !(SelectStart != SelectEnd) && CursorPosition < Length)
			{
				UndoStack.MakeReplace(Text, CursorPosition, 1, 1);
				DeleteChars(CursorPosition, 1);
				if (InsertChar(CursorPosition, ch))
				{
					UserSetCursorPosition(CursorPosition + 1, true);
				}
			}
			else
			{
				DeleteSelection();
				if (InsertChar(CursorPosition, ch))
				{
					UndoStack.MakeInsert(CursorPosition, 1);
					UserSetCursorPosition(CursorPosition + 1, true);
				}
			}
		}

		private void UndoRedo(UndoRedoStack undoStack, UndoRedoStack redoStack)
		{
			if (undoStack.Stack.Count == 0)
			{
				return;
			}

			var record = undoStack.Stack.Pop();
			try
			{
				_suppressRedoStackReset = true;
				switch (record.OperationType)
				{
					case OperationType.Insert:
						redoStack.MakeDelete(Text, record.Where, record.Length);
						DeleteChars(record.Where, record.Length);
						UserSetCursorPosition(record.Where);
						break;
					case OperationType.Delete:
						if (InsertChars(record.Where, record.Data))
						{
							redoStack.MakeInsert(record.Where, record.Data.Length);
							UserSetCursorPosition(record.Where + record.Data.Length);
						}
						break;
					case OperationType.Replace:
						redoStack.MakeReplace(Text, record.Where, record.Length, record.Data.Length());
						DeleteChars(record.Where, record.Length);
						InsertChars(record.Where, record.Data);
						break;
				}
			}
			finally
			{
				_suppressRedoStackReset = false;
			}
		}

		private void Undo()
		{
			UndoRedo(UndoStack, RedoStack);
		}

		private void Redo()
		{
			UndoRedo(RedoStack, UndoStack);
		}

		private void UserSetCursorPosition(int newPosition, bool forceChangeOnly = false)
		{
			if (newPosition > Length)
			{
				newPosition = Length;
			}

			if (newPosition < 0)
			{
				newPosition = 0;
			}

			CursorPosition = newPosition;

			if (forceChangeOnly || (!Desktop.IsShiftDown && !_isTouchDown))
			{
				SelectStart = SelectEnd = CursorPosition;
			}
			else
			{
				SelectEnd = CursorPosition;
			}
		}

		private void MoveLine(int delta)
		{
			var line = _formattedText.GetLineByCursorPosition(CursorPosition);
			if (line == null)
			{
				return;
			}

			var newLine = line.LineIndex + delta;
			if (newLine < 0 || newLine >= _formattedText.Strings.Length)
			{
				return;
			}

			var bounds = ActualBounds;
			var pos = GetRenderPositionByIndex(CursorPosition);
			var preferredX = pos.X - bounds.X;

			// Find closest glyph
			var newString = _formattedText.Strings[newLine];
			var glyphIndex = newString.GetGlyphIndexByX(preferredX);
			if (glyphIndex != null)
			{
				UserSetCursorPosition(newString.LineStart + glyphIndex.Value);
			}
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.C:
					if (Desktop.IsControlDown)
					{
						if (SelectEnd != SelectStart)
						{
							var selectStart = Math.Min(SelectStart, SelectEnd);
							var selectEnd = Math.Max(SelectStart, SelectEnd);
							Clipboard.SetText(Text.Substring(selectStart, selectEnd - selectStart));
						}
					}
					break;
				case Keys.V:
					if (!Readonly && Desktop.IsControlDown)
					{
						var clipboardText = Clipboard.GetText();
						if (!string.IsNullOrEmpty(clipboardText))
						{
							Paste(clipboardText);
						}
					}
					break;

				case Keys.Insert:
					if (!Readonly)
					{
						InsertMode = !InsertMode;
					}
					break;

				case Keys.Z:
					if (!Readonly && Desktop.IsControlDown)
					{
						Undo();
					}
					break;

				case Keys.Y:
					if (!Readonly && Desktop.IsControlDown)
					{
						Redo();
					}
					break;
				case Keys.A:
					if (Desktop.IsControlDown)
					{
						// Select all
						SelectStart = 0;
						SelectEnd = Length;
					}
					break;

				case Keys.Left:
					if (CursorPosition > 0)
					{
						UserSetCursorPosition(CursorPosition - 1);
					}
					break;

				case Keys.Right:
					if (CursorPosition < Length)
					{
						UserSetCursorPosition(CursorPosition + 1);
					}

					break;

				case Keys.Up:
					MoveLine(-1);
					break;

				case Keys.Down:
					MoveLine(1);
					break;

				case Keys.Back:
					if (!Readonly)
					{

						if (SelectStart == SelectEnd)
						{
							if (Delete(CursorPosition - 1, 1))
							{
								UserSetCursorPosition(CursorPosition - 1);
							}
						}
						else
						{
							DeleteSelection();
						}
					}
					break;

				case Keys.Delete:
					if (!Readonly)
					{
						if (SelectStart == SelectEnd)
						{
							Delete(CursorPosition, 1);
						}
						else
						{
							DeleteSelection();
						}
					}
					break;

				case Keys.Home:
				{
					if (!Desktop.IsControlDown)
					{
						var newPosition = CursorPosition;

						while (newPosition > 0 && Text[newPosition - 1] != '\n')
						{
							--newPosition;
						}

						UserSetCursorPosition(newPosition);
					}
					else
					{
						UserSetCursorPosition(0);
					}

					break;
				}

				case Keys.End:
				{
					if (!Desktop.IsControlDown)
					{
						var newPosition = CursorPosition;

						while (newPosition < Length && Text[newPosition] != '\n')
						{
							++newPosition;
						}

						UserSetCursorPosition(newPosition);
					}
					else
					{
						UserSetCursorPosition(Length);
					}

					break;
				}

				case Keys.Enter:
					if (!Readonly)
					{
						InputChar('\n');
					}
					break;
			}
		}

		private static string Process(string value)
		{
			// Remove '\r'
			if (value != null)
			{
				value = value.Replace("\r", string.Empty);
			}

			return value;
		}

		private bool SetText(string value, bool byUser)
		{
			value = Process(value);

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

			if (!byUser)
			{
				CursorPosition = SelectStart = SelectEnd = 0;
			}

			if (!_suppressRedoStackReset)
			{
				RedoStack.Reset();
			}

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

		private void UpdateScrolling()
		{
			var asScrollPane = Parent as ScrollPane;
			if (asScrollPane == null)
			{
				return;
			}

			var p = GetRenderPositionByIndex(CursorPosition);

			if (p.Y == _lastCursorY)
			{
				return;
			}

			var scrollMaximumPixels = asScrollPane.ScrollMaximumPixels;
			if (scrollMaximumPixels.Y == 0)
			{
				return;
			}

			var lineHeight = CrossEngineStuff.LineSpacing(_formattedText.Font);

			if (p.Y < asScrollPane.ActualBounds.Top)
			{
				var scrollMaximum = asScrollPane.ScrollMaximum;
				var newY = (-Top + p.Y - asScrollPane.ActualBounds.Top) * scrollMaximum.Y / scrollMaximumPixels.Y;

				var sp = asScrollPane.ScrollPosition;
				asScrollPane.ScrollPosition = new Point(sp.X, newY);
			}
			else if (p.Y + lineHeight > asScrollPane.ActualBounds.Bottom)
			{
				var scrollMaximum = asScrollPane.ScrollMaximum;
				var newY = (-Top + p.Y + lineHeight - asScrollPane.ActualBounds.Bottom) * scrollMaximum.Y / scrollMaximumPixels.Y;

				var sp = asScrollPane.ScrollPosition;
				asScrollPane.ScrollPosition = new Point(sp.X, newY);
			}

			_lastCursorY = p.Y;

			asScrollPane.UpdateLayout();
		}

		private void OnCursorIndexChanged()
		{
			UpdateScrolling();

			CursorPositionChanged?.Invoke(this, EventArgs.Empty);
		}

		public override void OnChar(char c)
		{
			base.OnChar(c);

			if (!Readonly && !char.IsControl(c))
			{
				InputChar(c);
			}
		}

		private void SetCursorByTouch()
		{
			var bounds = ActualBounds;
			var mousePos = Desktop.MousePosition;

			mousePos.X -= bounds.X;
			mousePos.Y -= bounds.Y;

			var line = _formattedText.GetLineByY(mousePos.Y);
			if (line != null)
			{
				var glyphIndex = line.GetGlyphIndexByX(mousePos.X);
				if (glyphIndex != null)
				{
					UserSetCursorPosition(line.LineStart + glyphIndex.Value);
				}
			}
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			if (Length == 0)
			{
				return;
			}

			SetCursorByTouch();

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

			SetCursorByTouch();
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

			if (Selection != null && !string.IsNullOrEmpty(Text))
			{
				var selectStart = Math.Min(SelectStart, SelectEnd);
				var selectEnd = Math.Max(SelectStart, SelectEnd);

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
			}
			else if (IsKeyboardFocused && FocusedTextColor != null)
			{
				textColor = FocusedTextColor.Value;
			}

			var centeredBounds = LayoutUtils.Align(new Point(bounds.Width, bounds.Height), _formattedText.Size, HorizontalAlignment.Left, TextVerticalAlignment);
			centeredBounds.Offset(bounds.Location);

			_formattedText.Draw(context.Batch, centeredBounds.Location, context.View, textColor, context.Opacity);

			if (!IsKeyboardFocused)
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
				var pos = GetRenderPositionByIndex(CursorPosition);
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

		public float GetWidth(int index)
		{
			var glyph = _formattedText.GetGlyphInfoByIndex(index);

			if (glyph == null)
			{
				return 0;
			}

			if (glyph.Character == '\n')
			{
				return FormattedText.NewLineWidth;
			}

			return glyph.Bounds.Width;
		}
	}
}