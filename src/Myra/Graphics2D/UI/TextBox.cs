using System;
using System.ComponentModel;
using System.Linq;
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
	public class TextBox : Widget
	{
		private DateTime _lastBlinkStamp = DateTime.Now;
		private bool _cursorOn = true;
		private bool _wrap = false;
		private readonly FormattedTextWithGlyphs _formattedText = new FormattedTextWithGlyphs();
		private readonly StringBuilder _stringBuilder = new StringBuilder();
		private bool _isTouchDown;
		private Point? _lastCursorPosition;
		private int _cursorIndex;
		private Point _internalScrolling = Point.Zero;
		private bool _suppressRedoStackReset = false;

		private readonly UndoRedoStack UndoStack = new UndoRedoStack();
		private readonly UndoRedoStack RedoStack = new UndoRedoStack();

		[Category("Appearance")]
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

		[Category("Appearance")]
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

		[Category("Behavior")]
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

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
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

		[Category("Appearance")]
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

		[Category("Appearance")]
		public Color TextColor
		{
			get; set;
		}

		[Category("Appearance")]
		public Color? DisabledTextColor
		{
			get; set;
		}

		[Category("Appearance")]
		public Color? FocusedTextColor
		{
			get; set;
		}

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public IRenderable Cursor
		{
			get; set;
		}

		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public IRenderable Selection
		{
			get; set;
		}

		[Category("Behavior")]
		[DefaultValue(450)]
		public int BlinkIntervalInMs
		{
			get; set;
		}

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool Readonly
		{
			get; set;
		}

		[Category("Behavior")]
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

		[Category("Behavior")]
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

		[Browsable(false)]
		[XmlIgnore]
		public Func<string, string> InputFilter
		{
			get; set;
		}

		[Browsable(false)]
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

		[Browsable(false)]
		[XmlIgnore]
		public Point CursorScreenPosition
		{
			get
			{
				return GetRenderPositionByIndex(CursorPosition);
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public int SelectStart
		{
			get; private set;
		}

		[Browsable(false)]
		[XmlIgnore]
		public int SelectEnd
		{
			get; private set;
		}

		private int CursorWidth
		{
			get
			{
				return 1 + (Cursor != null ? Cursor.Size.X : 0);
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

		public event EventHandler CursorPositionChanged;

		public TextBox(TextBoxStyle style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;

			AcceptsKeyboardFocus = true;
			ClipToBounds = true;

			if (style != null)
			{
				ApplyTextBoxStyle(style);
			}

			BlinkIntervalInMs = 450;
		}

		public TextBox(Stylesheet stylesheet, string style) : this(stylesheet.TextBoxStyles[style])
		{
		}

		public TextBox(Stylesheet stylesheet) : this(stylesheet.TextBoxStyle)
		{
		}

		public TextBox(string style) : this(Stylesheet.Current, style)
		{
		}

		public TextBox() : this(Stylesheet.Current)
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
				DeleteSelection();
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
			if (!Enabled)
			{
				return;
			}

			switch (k)
			{
				case Keys.C:
					if (Desktop.IsControlDown)
					{
						if (SelectEnd != SelectStart)
						{
							var selectStart = Math.Min(SelectStart, SelectEnd);
							var selectEnd = Math.Max(SelectStart, SelectEnd);

							var clipboardText = Text.Substring(selectStart, selectEnd - selectStart);
							try
							{
								Clipboard.SetText(clipboardText);
							}
							catch (Exception)
							{
								MyraEnvironment.InternalClipboard = clipboardText;
							}
						}
					}
					break;
				case Keys.V:
					if (!Readonly && Desktop.IsControlDown)
					{
						string clipboardText;
						try
						{
							clipboardText = Clipboard.GetText();
						}
						catch (Exception)
						{
							clipboardText = MyraEnvironment.InternalClipboard;
						}

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
								UserSetCursorPosition(CursorPosition - 1, true);
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
					if (!Desktop.IsControlDown && !string.IsNullOrEmpty(Text))
					{
						var newPosition = CursorPosition;

						while (newPosition > 0 &&
							(newPosition - 1 >= Text.Length ||
							Text[newPosition - 1] != '\n'))
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
			var p = GetRenderPositionByIndex(CursorPosition);
			if (p == _lastCursorPosition)
			{
				return;
			}

			var asScrollViewer = Parent as ScrollViewer;

			Point sz, maximum;
			var bounds = ActualBounds;
			if (asScrollViewer != null)
			{
				asScrollViewer.UpdateLayout();
				sz = new Point(asScrollViewer.Bounds.Width, asScrollViewer.Bounds.Height);
				maximum = asScrollViewer.ScrollMaximum;
			}
			else
			{
				sz = new Point(Bounds.Width, Bounds.Height);
				maximum = new Point(_formattedText.Size.X + CursorWidth - sz.X,
					_formattedText.Size.Y - sz.Y);

				if (maximum.X < 0)
				{
					maximum.X = 0;
				}

				if (maximum.Y < 0)
				{
					maximum.Y = 0;
				}
			}

			if (maximum == Point.Zero)
			{
				_internalScrolling = Point.Zero;
				_lastCursorPosition = p;
				return;
			}

			p.X -= bounds.X;
			p.Y -= bounds.Y;

			var lineHeight = CrossEngineStuff.LineSpacing(_formattedText.Font);

			Point sp;
			if (asScrollViewer != null)
			{
				sp = asScrollViewer.ScrollPosition;
			}
			else
			{
				sp = _internalScrolling;
			}

			if (p.Y < sp.Y)
			{
				sp.Y = p.Y;
			}
			else if (p.Y + lineHeight > sp.Y + sz.Y)
			{
				sp.Y = p.Y + lineHeight - sz.Y;
			}

			if (p.X < sp.X)
			{
				sp.X = p.X;
			}
			else if (p.X + CursorWidth > sp.X + sz.X)
			{
				sp.X = p.X + CursorWidth - sz.X;
			}

			if (asScrollViewer != null)
			{
				asScrollViewer.ScrollPosition = sp;
			}
			else
			{
				if (sp.X < 0)
				{
					sp.X = 0;
				}

				if (sp.X > maximum.X)
				{
					sp.X = maximum.X;
				}

				if (sp.Y < 0)
				{
					sp.Y = 0;
				}

				if (sp.Y > maximum.Y)
				{
					sp.Y = maximum.Y;
				}

				_internalScrolling = sp;
			}

			_lastCursorPosition = p;
		}

		private void OnCursorIndexChanged()
		{
			UpdateScrolling();

			CursorPositionChanged?.Invoke(this, EventArgs.Empty);
		}

		public override void OnChar(char c)
		{
			base.OnChar(c);

			if (!Enabled)
			{
				return;
			}

			if (!Readonly && !char.IsControl(c))
			{
				InputChar(c);
			}
		}

		private void SetCursorByTouch()
		{
			var bounds = ActualBounds;
			var mousePos = Desktop.TouchPosition;

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

			if (!Enabled)
			{
				return;
			}

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

		public override void OnTouchMoved()
		{
			base.OnTouchMoved();
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

			return new Point(x, y);
		}

		private void RenderSelection(RenderContext context)
		{
			var bounds = ActualBounds;

			if (string.IsNullOrEmpty(Text) || Selection == null)
			{
				return;
			}

			var selectStart = Math.Min(SelectStart, SelectEnd);
			var selectEnd = Math.Max(SelectStart, SelectEnd);

			if (selectStart >= selectEnd)
			{
				return;
			}

			var startGlyph = _formattedText.GetGlyphInfoByIndex(selectStart);
			if (startGlyph == null)
			{
				return;
			}

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
						new Rectangle(startPosition.X - _internalScrolling.X,
							startPosition.Y - _internalScrolling.Y,
							endPosition.X - startPosition.X,
							CrossEngineStuff.LineSpacing(_formattedText.Font)));

					break;
				}

				context.Draw(Selection,
					new Rectangle(startPosition.X - _internalScrolling.X,
						startPosition.Y - _internalScrolling.Y,
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

		public override void InternalRender(RenderContext context)
		{
			if (_formattedText.Font == null)
			{
				return;
			}

			var bounds = ActualBounds;
			RenderSelection(context);

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

			var p = new Point(centeredBounds.Location.X - _internalScrolling.X,
				centeredBounds.Location.Y - _internalScrolling.Y);
			_formattedText.Draw(context.Batch, 
				p,
				context.View, textColor, context.Opacity);

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

			if (Enabled && _cursorOn && Cursor != null)
			{
				p = GetRenderPositionByIndex(CursorPosition);
				p.X -= _internalScrolling.X;
				p.Y -= _internalScrolling.Y;
				context.Draw(Cursor,
					new Rectangle(p.X, p.Y,
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

			width -= CursorWidth;

			var result = Point.Zero;
			if (Font != null)
			{
				result = _formattedText.Measure(_wrap ? width : default(int?));
			}

			if (result.Y < CrossEngineStuff.LineSpacing(Font))
			{
				result.Y = CrossEngineStuff.LineSpacing(Font);
			}

			if (Cursor != null)
			{
				result.X += CursorWidth;
				result.Y = Math.Max(result.Y, Cursor.Size.Y);
			}

			return result;
		}

		public override void Arrange()
		{
			base.Arrange();

			_formattedText.Width = _wrap ? ActualBounds.Width : default(int?);
		}

		public void ApplyTextBoxStyle(TextBoxStyle style)
		{
			ApplyWidgetStyle(style);

			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			FocusedTextColor = style.FocusedTextColor;

			Cursor = style.Cursor;
			Selection = style.Selection;

			Font = style.Font;
		}

		public override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyTextBoxStyle(stylesheet.TextBoxStyles[name]);
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