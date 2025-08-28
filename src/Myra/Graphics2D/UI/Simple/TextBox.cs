using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using TextCopy;
using Myra.Graphics2D.UI.TextEdit;
using FontStashSharp;
using FontStashSharp.RichText;
using Myra.Events;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Numerics;
using System.Drawing;
using Myra.Platform;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	public class TextBox : Widget
	{
		private const int CursorUpdateDelayInMs = 30;

		private DateTime _lastCursorUpdate;
		private DateTime _lastBlinkStamp = DateTime.Now;
		private bool _cursorOn = true;
		private bool _wrap = false;
		private readonly RichTextLayout _richTextLayout = new RichTextLayout
		{
			CalculateGlyphs = true,
			SupportsCommands = false
		};

		private Point? _lastCursorPosition;
		private int _cursorIndex;
		private Point _internalScrolling = Mathematics.PointZero;
		private bool _suppressRedoStackReset = false;
		private string _text;
		private string _hintText;
		private bool _passwordField;
		private bool _isTouchDown;

		private readonly UndoRedoStack UndoStack = new UndoRedoStack();
		private readonly UndoRedoStack RedoStack = new UndoRedoStack();

		[Category("Appearance")]
		[DefaultValue(0)]
		public int VerticalSpacing
		{
			get
			{
				return _richTextLayout.VerticalSpacing;
			}
			set
			{
				_richTextLayout.VerticalSpacing = value;
				InvalidateMeasure();
			}
		}

		[Category("Appearance")]
		[DefaultValue(null)]
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				SetText(value, false);
				DisableHintText();
			}
		}

		[Category("Appearance")]
		[DefaultValue(null)]
		public string HintText
		{
			get
			{
				return _hintText;
			}
			set
			{
				_hintText = value;

				if (_text == null)
				{
					EnableHintText();
				}
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool HintTextEnabled { get; set; }

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool Multiline { get; set; }

		private string UserText
		{
			get
			{
				return _text;
			}

			set
			{
				SetText(value, true);
			}
		}

		private int Length => _text.Length();

		private bool InsertMode { get; set; }

		[Category("Appearance")]
		public SpriteFontBase Font
		{
			get
			{
				return _richTextLayout.Font;
			}
			set
			{
				_richTextLayout.Font = value;
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
		public Color TextColor { get; set; }

		[Category("Appearance")]
		public Color? DisabledTextColor { get; set; }

		[Category("Appearance")]
		public Color? FocusedTextColor { get; set; }

		[Category("Appearance")]
		public IImage Cursor { get; set; }

		[Category("Appearance")]
		public IBrush Selection { get; set; }

		[Category("Behavior")]
		[DefaultValue(450)]
		public int BlinkIntervalInMs { get; set; }

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool Readonly { get; set; }

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool PasswordField
		{
			get
			{
				return _passwordField;
			}
			set
			{
				_passwordField = value;
				UpdateRichTextLayout();
			}
		}

		[Category("Behavior")]
		[DefaultValue(VerticalAlignment.Top)]
		public VerticalAlignment TextVerticalAlignment { get; set; }

		[Category("Behavior")]
		[DefaultValue(MouseCursorType.IBeam)]
		public override MouseCursorType? MouseCursor
		{
			get => base.MouseCursor;
			set => base.MouseCursor = value;
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

		/// <summary>
		/// Cursor position in local coordinates
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Point CursorCoords => GetRenderPositionByIndex(CursorPosition);

		[Browsable(false)]
		[XmlIgnore]
		public int SelectStart { get; private set; }

		[Browsable(false)]
		[XmlIgnore]
		public int SelectEnd { get; private set; }

		private int CursorWidth => 1 + (Cursor != null ? Cursor.Size.X : 0);

		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}

			internal set
			{
				if (Desktop != null)
				{
					Desktop.TouchUp -= DesktopTouchUp;
					Desktop.TouchDown -= DesktopTouchDown;
				}

				base.Desktop = value;

				if (Desktop != null)
				{
					Desktop.TouchUp += DesktopTouchUp;
					Desktop.TouchDown += DesktopTouchDown;
				}
			}
		}

		/// <summary>
		/// Fires when the value is about to be changed
		/// Set Cancel to true if you want to cancel the change
		/// </summary>
		public event MyraEventHandler<ValueChangingEventArgs<string>> ValueChanging;

		/// <summary>
		/// Fires every time when the text had been changed
		/// </summary>
		public event MyraEventHandler<ValueChangedEventArgs<string>> TextChanged;

		/// <summary>
		/// Fires every time when the text had been changed by user(doesnt fire if it had been assigned through code)
		/// </summary>
		public event MyraEventHandler<ValueChangedEventArgs<string>> TextChangedByUser;
		
		/// <summary>
		/// Fires every time when the text had been deleted
		/// </summary>
		public event MyraEventHandler<TextDeletedEventArgs> TextDeleted;
		
		public event MyraEventHandler CursorPositionChanged;

		public TextBox(string styleName = Stylesheet.DefaultStyleName)
		{
			AcceptsKeyboardFocus = true;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;

			ClipToBounds = true;

			SetStyle(styleName);

			BlinkIntervalInMs = 450;

			MouseCursor = MouseCursorType.IBeam;
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

		private int Delete(int where, int len)
		{
			if (where < 0 || where >= Length || len < 0)
			{
				return 0;
			}

			// If we're trying to delete one part
			// of a surrogate pair, delete both.
			if (len == 1)
			{
				if (char.IsSurrogate(Text[where]))
				{
					len++;
				}

				if (char.IsLowSurrogate(Text[where]))
				{
					where--;
				}
			}

			UndoStack.MakeDelete(Text, where, len);
			var stringToDelete = Text.Substring(where, len);
			DeleteChars(where, len);

			TextDeleted?.Invoke(this, new TextDeletedEventArgs(where, stringToDelete));
			return len;
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
			text = Process(text);

			DeleteSelection();
			if (InsertChars(CursorPosition, text))
			{
				UndoStack.MakeInsert(CursorPosition, text.Length());
				CursorPosition += text.Length;
				ResetSelection();
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
					UserSetCursorPosition(CursorPosition + 1);

				}
			}
			else
			{
				DeleteSelection();
				if (InsertChar(CursorPosition, ch))
				{
					UndoStack.MakeInsert(CursorPosition, 1);
					UserSetCursorPosition(CursorPosition + 1);
				}
			}

			ResetSelection();
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

			ResetSelection();
		}

		private void Undo()
		{
			UndoRedo(UndoStack, RedoStack);
		}

		private void Redo()
		{
			UndoRedo(RedoStack, UndoStack);
		}

		private void UserSetCursorPosition(int newPosition)
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
		}

		private void ResetSelection()
		{
			SelectStart = SelectEnd = CursorPosition;
		}

		private void UpdateSelection()
		{
			SelectEnd = CursorPosition;
		}

		private void UpdateSelectionIfShiftDown()
		{
			if (Desktop.IsShiftDown)
			{
				UpdateSelection();
			}
			else
			{
				ResetSelection();
			}
		}

		private void MoveLine(int delta)
		{
			var line = _richTextLayout.GetLineByCursorPosition(CursorPosition);
			if (line == null)
			{
				return;
			}

			var newLine = line.LineIndex + delta;
			if (newLine < 0 || newLine >= _richTextLayout.Lines.Count)
			{
				return;
			}

			var bounds = ActualBounds;
			var pos = GetRenderPositionByIndex(CursorPosition);
			var preferredX = pos.X - bounds.X;

			// Find closest glyph
			var newString = _richTextLayout.Lines[newLine];
			var cursorPosition = newString.TextStartIndex;
			var glyphIndex = newString.GetGlyphIndexByX(preferredX);
			if (glyphIndex != null)
			{
				cursorPosition += glyphIndex.Value;
			}

			UserSetCursorPosition(cursorPosition);
			UpdateSelectionIfShiftDown();
		}

		public void SelectAll()
		{
			// Select all
			SelectStart = 0;
			SelectEnd = Length;
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
						Copy();
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

				case Keys.X:
					if (Desktop.IsControlDown)
					{
						Copy();
						if (!Readonly && SelectStart != SelectEnd)
						{
							DeleteSelection();
						}
					}
					break;

				case Keys.D:
					if (!Readonly && Desktop.IsControlDown)
					{
						// nothing selected -> duplicate current line
						if (SelectStart == SelectEnd)
						{
							// get start of line
							var searchStart = Math.Max(0, SelectStart - 1);
							var lineStart = Text.LastIndexOf("\n", searchStart);
							// special case: cursor is in first line
							if (lineStart == -1) lineStart = 0;

							// get end of line
							var lineEnd = Text.IndexOf("\n", SelectEnd);
							// special case: cursor is in last line
							if (lineEnd == -1) lineEnd = Text.Length;

							var line = Text.Substring(lineStart, lineEnd - lineStart);
							if (lineStart == 0)
								line = "\n" + line;
							Insert(lineEnd, line);
						}
						// duplicate selection
						else
						{
							var start = Math.Min(SelectStart, SelectEnd);
							var end = Math.Max(SelectStart, SelectEnd);
							var text = Text.Substring(start, end - start);
							Insert(end, text);
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
						SelectAll();
					}
					break;

				case Keys.Left:
					if (CursorPosition > 0)
					{
						UserSetCursorPosition(CursorPosition - 1);
						UpdateSelectionIfShiftDown();
					}
					break;

				case Keys.Right:
					if (CursorPosition < Length)
					{
						UserSetCursorPosition(CursorPosition + 1);
						UpdateSelectionIfShiftDown();
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
							int deleted = Delete(CursorPosition - 1, 1);
							if (deleted > 0)
							{
								UserSetCursorPosition(CursorPosition - deleted);
								ResetSelection();
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

						UpdateSelectionIfShiftDown();

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

						UpdateSelectionIfShiftDown();

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

		private void Copy()
		{
			if (SelectEnd != SelectStart)
			{
				var selectStart = Math.Min(SelectStart, SelectEnd);
				var selectEnd = Math.Max(SelectStart, SelectEnd);

				var clipboardText = _richTextLayout.Text.Substring(selectStart, selectEnd - selectStart);
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
			if (value == _text)
			{
				return false;
			}

			var oldValue = _text;
			if (ValueChanging != null)
			{
				var args = new ValueChangingEventArgs<string>(oldValue, value);
				ValueChanging(this, args);
				if (args.Cancel)
				{
					return false;
				}

				value = args.NewValue;
			}

			_text = value;

			UpdateRichTextLayout();

			if (!byUser)
			{
				CursorPosition = SelectStart = SelectEnd = 0;
			}

			if (!_suppressRedoStackReset)
			{
				RedoStack.Reset();
			}

			InvalidateMeasure();

			TextChanged?.Invoke(this, new ValueChangedEventArgs<string>(oldValue, value));

			if (byUser)
			{
				TextChangedByUser?.Invoke(this, new ValueChangedEventArgs<string>(oldValue, value));
			}

			return true;
		}

		private void UpdateRichTextLayout()
		{
			if (string.IsNullOrEmpty(_text))
			{
				_richTextLayout.Text = _text;
				EnableHintText();
				return;
			}

			DisableHintText();
			_richTextLayout.Text = PasswordField ? new string('*', _text.Length) : _text;
		}

		private void DisableHintText()
		{
			if (_hintText == null)
			{
				return;
			}

			_richTextLayout.Text = _text;
			HintTextEnabled = false;
		}

		private void EnableHintText()
		{
			if (ShouldEnableHintText())
			{
				_richTextLayout.Text = _hintText;
				HintTextEnabled = true;
			}
		}

		private bool ShouldEnableHintText()
		{
			return _hintText != null &&
				   string.IsNullOrEmpty(_text)
				   && !IsKeyboardFocused;
		}

		private void UpdateScrolling()
		{
			var p = GetRenderPositionByIndex(CursorPosition);
			if (p == _lastCursorPosition || Desktop == null)
			{
				return;
			}

			Desktop.UpdateLayout();

			var asScrollViewer = Parent as ScrollViewer;

			Point sz, maximum;
			var bounds = ActualBounds;
			if (asScrollViewer != null)
			{
				sz = new Point(asScrollViewer.Bounds.Width, asScrollViewer.Bounds.Height);
				sz.X -= asScrollViewer.VerticalThumbWidth;
				sz.Y -= asScrollViewer.HorizontalThumbHeight;

				maximum = asScrollViewer.ScrollMaximum;
			}
			else
			{
				sz = new Point(Bounds.Width, Bounds.Height);
				maximum = new Point(_richTextLayout.Size.X + CursorWidth - sz.X,
					_richTextLayout.Size.Y - sz.Y);

				if (maximum.X < 0)
				{
					maximum.X = 0;
				}

				if (maximum.Y < 0)
				{
					maximum.Y = 0;
				}
			}

			if (maximum == Mathematics.PointZero)
			{
				_internalScrolling = Mathematics.PointZero;
				_lastCursorPosition = p;
				return;
			}

			p.X -= bounds.X;
			p.Y -= bounds.Y;

			var lineHeight = _richTextLayout.Font.LineHeight;

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
			_lastCursorUpdate = DateTime.Now;
			
			UpdateScrolling();

			CursorPositionChanged.Invoke(this, InputEventType.CursorPositionChanged);
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
			if (Desktop == null)
			{
				return;
			}

			var mousePos = ToLocal(Desktop.TouchPosition.Value);
			mousePos.X += _internalScrolling.X;
			mousePos.Y += _internalScrolling.Y;

			if (mousePos.X < 0)
			{
				mousePos.X = 0;
			}

			if (mousePos.Y < 0)
			{
				mousePos.Y = 0;
			}

			var line = _richTextLayout.GetLineByY(mousePos.Y);
			if (line != null)
			{
				var glyphIndex = line.GetGlyphIndexByX(mousePos.X);
				if (glyphIndex != null)
				{
					UserSetCursorPosition(line.TextStartIndex + glyphIndex.Value);
					if (_isTouchDown || Desktop.IsShiftDown)
					{
						UpdateSelection();
					}
					else
					{
						ResetSelection();
					}
				}
			}
		}

		private void DesktopTouchUp(object sender, MyraEventArgs args)
		{
			_isTouchDown = false;
		}

		private void DesktopTouchDown(object sender, MyraEventArgs e)
		{
			if (!Enabled || !IsTouchInside || Length == 0)
			{
				return;
			}

			SetCursorByTouch();

			_lastCursorUpdate = DateTime.Now;
			_isTouchDown = true;
		}


		public override void OnTouchDoubleClick()
		{
			base.OnTouchDoubleClick();

			var position = CursorPosition;
			if (string.IsNullOrEmpty(Text) || position < 0 || position >= Text.Length || Desktop.IsShiftDown)
			{
				return;
			}

			if (char.IsWhiteSpace(Text[position]))
			{
				if (position == 0)
				{
					return;
				}

				--position;
				if (char.IsWhiteSpace(Text[position]))
				{
					return;
				}
			}

			int start, end;
			start = end = position;

			while (start > 0)
			{
				if (char.IsWhiteSpace(Text[start]))
				{
					++start;
					break;
				}

				--start;
			}

			while (end < Text.Length)
			{
				if (char.IsWhiteSpace(Text[end]))
				{
					break;
				}

				++end;
			}

			if (start == end)
			{
				return;
			}

			SelectStart = start;
			SelectEnd = end;
		}

		public override void OnGotKeyboardFocus()
		{
			base.OnGotKeyboardFocus();

			_lastBlinkStamp = DateTime.Now;
			_cursorOn = true;

			DisableHintText();
		}

		public override void OnLostKeyboardFocus()
		{
			base.OnLostKeyboardFocus();

			EnableHintText();
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
					var glyphRender = _richTextLayout.GetGlyphInfoByIndex(index);
					if (glyphRender != null)
					{
						x += glyphRender.Value.Bounds.Left;
						y += glyphRender.Value.LineTop;
					}
				}
				else if (_richTextLayout.Lines != null && _richTextLayout.Lines.Count > 0)
				{
					// After last glyph
					var lastLine = _richTextLayout.Lines[_richTextLayout.Lines.Count - 1];
					if (lastLine.Count > 0)
					{
						var glyphRender = lastLine.GetGlyphInfoByIndex(lastLine.Count - 1);

						x += glyphRender.Value.Bounds.Left + glyphRender.Value.XAdvance;
						y += glyphRender.Value.LineTop;
					}
					else if (_richTextLayout.Lines.Count > 1)
					{
						var previousLine = _richTextLayout.Lines[_richTextLayout.Lines.Count - 2];
						if (previousLine.Count > 0)
						{
							var glyphRender = previousLine.GetGlyphInfoByIndex(0);
							y += glyphRender.Value.LineTop + lastLine.Size.Y + _richTextLayout.VerticalSpacing;
						}
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

			var startGlyph = _richTextLayout.GetGlyphInfoByIndex(selectStart);
			if (startGlyph == null)
			{
				return;
			}

			var lineIndex = startGlyph.Value.TextChunk.LineIndex;
			var i = selectStart;

			var lineHeight = _richTextLayout.Font.LineHeight;
			while (true)
			{
				startGlyph = _richTextLayout.GetGlyphInfoByIndex(i);
				if (startGlyph == null)
				{
					break;
				}

				var startPosition = GetRenderPositionByIndex(i);

				var line = _richTextLayout.Lines[startGlyph.Value.TextChunk.LineIndex];

				if (selectEnd < line.TextStartIndex + line.Count)
				{
					var endPosition = GetRenderPositionByIndex(selectEnd);

					Selection.Draw(context,
						new Rectangle(startPosition.X - _internalScrolling.X,
							startPosition.Y - _internalScrolling.Y,
							endPosition.X - startPosition.X,
							lineHeight));

					break;
				}

				Selection.Draw(context,
					new Rectangle(startPosition.X - _internalScrolling.X,
						startPosition.Y - _internalScrolling.Y,
						bounds.Left + startGlyph.Value.TextChunk.Size.X - startPosition.X,
						lineHeight));

				++lineIndex;
				if (lineIndex >= _richTextLayout.Lines.Count)
				{
					break;
				}

				i = _richTextLayout.Lines[lineIndex].TextStartIndex;
			}
		}

		public override void InternalRender(RenderContext context)
		{
			if (_richTextLayout.Font == null)
			{
				return;
			}

			if (_isTouchDown)
			{
				// This makes the text to scroll if the touch is outside of the TextBox bounds
				var passed = DateTime.Now - _lastCursorUpdate;
				if (passed.TotalMilliseconds > CursorUpdateDelayInMs)
				{
					SetCursorByTouch();
					_lastCursorUpdate = DateTime.Now;
				}
			}

			var bounds = ActualBounds;
			RenderSelection(context);

			var textColor = TextColor;
			var oldOpacity = context.Opacity;

			if (HintTextEnabled)
			{
				context.Opacity *= 0.5f;
			}
			else if (!Enabled && DisabledTextColor != null)
			{
				textColor = DisabledTextColor.Value;
			}
			else if (IsKeyboardFocused && FocusedTextColor != null)
			{
				textColor = FocusedTextColor.Value;
			}

			var centeredBounds = LayoutUtils.Align(new Point(bounds.Width, bounds.Height), _richTextLayout.Size, HorizontalAlignment.Left, TextVerticalAlignment);
			centeredBounds.Offset(bounds.Location);

			var p = new Point(centeredBounds.Location.X - _internalScrolling.X,
				centeredBounds.Location.Y - _internalScrolling.Y);

			if (MyraEnvironment.DrawTextGlyphsFrames)
			{
				foreach (var line in _richTextLayout.Lines)
				{
					foreach (TextChunk chunk in line.Chunks)
					{
						foreach (var glyph in chunk.Glyphs)
						{
							var glyphBounds = glyph.Bounds;
							glyphBounds.Offset(p);
							context.DrawRectangle(glyphBounds, Color.White);
						}
					}
				}
			}

			context.DrawRichText(_richTextLayout, new Vector2(p.X, p.Y), textColor);

			if (!IsKeyboardFocused)
			{
				// Skip cursor rendering if the widget doesnt have the focus
				return;
			}

			var now = DateTime.Now;
			
			if (_lastCursorUpdate > _lastBlinkStamp)
			{
				_lastBlinkStamp = _lastCursorUpdate;
				_cursorOn = true;
			}
			
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

				var rect = new Rectangle(p.X, p.Y, Cursor.Size.X, _richTextLayout.Font.LineHeight);
				Cursor.Draw(context, rect);
			}

			context.Opacity = oldOpacity;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			if (Font == null)
			{
				return Mathematics.PointZero;
			}

			var width = availableSize.X;
			width -= CursorWidth;

			var result = Mathematics.PointZero;
			if (Font != null)
			{
				result = _richTextLayout.Measure(_wrap ? width : default(int?));
			}

			if (result.Y < Font.LineHeight)
			{
				result.Y = Font.LineHeight;
			}

			if (Cursor != null)
			{
				result.X += CursorWidth;
				result.Y = Math.Max(result.Y, Cursor.Size.Y);
			}

			return result;
		}

		protected override void InternalArrange()
		{
			base.InternalArrange();

			var width = ActualBounds.Width;
			width -= CursorWidth;

			_richTextLayout.Width = _wrap ? width : default(int?);
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

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyTextBoxStyle(stylesheet.TextBoxStyles.SafelyGetStyle(name));
		}

		public float GetWidth(int index)
		{
			var glyph = _richTextLayout.GetGlyphInfoByIndex(index);
			if (glyph == null)
			{
				return 0;
			}

			if (glyph.Value.Codepoint == '\n')
			{
				return 0;
			}

			return glyph.Value.Bounds.Width;
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var textBox = (TextBox)w;
			VerticalSpacing = textBox.VerticalSpacing;
			Text = textBox.Text;
			HintText = textBox.HintText;
			HintTextEnabled = textBox.HintTextEnabled;
			Multiline = textBox.Multiline;
			Font = textBox.Font;
			Wrap = textBox.Wrap;
			TextColor = textBox.TextColor;
			DisabledTextColor = textBox.DisabledTextColor;
			FocusedTextColor = textBox.FocusedTextColor;
			Cursor = textBox.Cursor;
			Selection = textBox.Selection;
			BlinkIntervalInMs = textBox.BlinkIntervalInMs;
			Readonly = textBox.Readonly;
			PasswordField = textBox.PasswordField;
			TextVerticalAlignment = textBox.TextVerticalAlignment;
		}
	}
}