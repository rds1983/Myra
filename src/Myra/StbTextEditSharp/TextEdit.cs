using Myra.Utility;
using System;
using System.Text;

namespace StbTextEditSharp
{
	internal class TextEdit
	{
		internal readonly ITextEditHandler Handler;

		private int _cursorIndex;
		private bool _suppressRedoStackReset = false;

		public int CursorIndex
		{
			get
			{
				return _cursorIndex;
			}

			set
			{
				if (value == _cursorIndex)
				{
					return;
				}

				_cursorIndex = value;
				var ev = CursorIndexChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		public int Length => Handler.Length;

		public string Text
		{
			get
			{
				return Handler.Text;
			}

			set
			{
				Handler.Text = value;

				if (!_suppressRedoStackReset)
				{
					RedoStack.Reset();
				}
			}
		}


		public bool CursorAtEndOfLine;
		public bool HasPreferredX;
		public bool InsertMode;
		public float PreferredX;
		public int SelectEnd;
		public int SelectStart;
		public bool SingleLine = true;
		public readonly UndoRedoStack UndoStack = new UndoRedoStack();
		public readonly UndoRedoStack RedoStack = new UndoRedoStack();

		public event EventHandler CursorIndexChanged;

		public TextEdit(ITextEditHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException("handler");

			Handler = handler;

			ClearState();
		}

		public void SortSelection()
		{
			if (SelectEnd < SelectStart)
			{
				var temp = SelectEnd;
				SelectEnd = SelectStart;
				SelectStart = temp;
			}
		}

		public void MoveToFirst()
		{
			if (SelectStart != SelectEnd)
			{
				SortSelection();
				CursorIndex = SelectStart;
				SelectEnd = SelectStart;
				HasPreferredX = false;
			}
		}

		public void PrepareSelectionAtCursor()
		{
			if (!(SelectStart != SelectEnd))
				SelectStart = SelectEnd = CursorIndex;
			else
				CursorIndex = SelectEnd;
		}

		public void ClearState()
		{
			UndoStack.Reset();
			RedoStack.Reset();
			SelectEnd = SelectStart = 0;
			CursorIndex = 0;
			HasPreferredX = false;
			PreferredX = 0;
			CursorAtEndOfLine = false;
			InsertMode = false;
		}

		public void DeleteChars(int pos, int l)
		{
			if (l == 0)
				return;

			Text = Text.Substring(0, pos) + Text.Substring(pos + l);
		}

		public int InsertChars(int pos, int[] codepoints, int start, int length)
		{
			var sb = new StringBuilder();
			for (var i = start; i < start + length; ++i)
			{
				sb.Append(char.ConvertFromUtf32(codepoints[i]));
			}

			InsertChars(pos, sb.ToString());

			return length;
		}

		public int InsertChars(int pos, string s)
		{
			if (string.IsNullOrEmpty(s))
				return 0;

			if (string.IsNullOrEmpty(Text))
				Text = s;
			else
				Text = Text.Substring(0, pos) + s + Text.Substring(pos);

			return s.Length;
		}

		public int InsertChar(int pos, int codepoint)
		{
			var s = char.ConvertFromUtf32(codepoint);
			return InsertChars(pos, s);
		}

		public int LocateCoord(float x, float y)
		{
			var r = new TextEditRow();
			var n = Length;
			var base_y = (float)0;
			var i = 0;
			var k = 0;
			r.x0 = r.x1 = 0;
			r.ymin = r.ymax = 0;
			r.num_chars = 0;
			while (i < n)
			{
				r = Handler.LayoutRow(i);
				if (r.num_chars <= 0)
					return n;
				if (i == 0 && y < base_y + r.ymin)
					return 0;
				if (y < base_y + r.ymax)
					break;
				i += r.num_chars;
				base_y += r.baseline_y_delta;
			}

			if (i >= n)
				return n;
			if (x < r.x0)
				return i;
			if (x < r.x1)
			{
				var prev_x = r.x0;
				for (k = 0; k < r.num_chars; ++k)
				{
					var w = Handler.GetWidth(i + k);
					if (x < prev_x + w)
					{
						if (x < prev_x + w / 2)
							return k + i;
						return k + i + 1;
					}

					prev_x += w;
				}
			}

			if (Text[i + r.num_chars - 1] == '\n')
				return i + r.num_chars - 1;
			return i + r.num_chars;
		}

		public void Click(float x, float y, bool isShiftDown = false)
		{
			if (SingleLine)
			{
				var r = new TextEditRow();
				r = Handler.LayoutRow(0);
				y = r.ymin;
			}

			CursorIndex = LocateCoord(x, y);

			if (!isShiftDown)
			{
				SelectStart = CursorIndex;
			}

			SelectEnd = CursorIndex;
			HasPreferredX = false;
		}

		public void Drag(float x, float y)
		{
			var p = 0;
			if (SingleLine)
			{
				var r = new TextEditRow();
				r = Handler.LayoutRow(0);
				y = r.ymin;
			}

			if (SelectStart == SelectEnd)
				SelectStart = CursorIndex;
			p = LocateCoord(x, y);
			CursorIndex = SelectEnd = p;
		}

		public void Clamp()
		{
			var n = Length;
			if (SelectStart != SelectEnd)
			{
				if (SelectStart > n)
					SelectStart = n;
				if (SelectEnd > n)
					SelectEnd = n;
				if (SelectStart == SelectEnd)
					CursorIndex = SelectStart;
			}

			if (CursorIndex > n)
				CursorIndex = n;
		}

		public void Delete(int where, int len)
		{
			UndoStack.MakeDelete(Text, where, len);
			DeleteChars(where, len);
			HasPreferredX = false;
		}

		public void DeleteSelection()
		{
			Clamp();
			if (SelectStart != SelectEnd)
			{
				if (SelectStart < SelectEnd)
				{
					Delete(SelectStart, SelectEnd - SelectStart);
					SelectEnd = CursorIndex = SelectStart;
				}
				else
				{
					Delete(SelectEnd, SelectStart - SelectEnd);
					SelectStart = CursorIndex = SelectEnd;
				}

				HasPreferredX = false;
			}
		}

		public void MoveToLast()
		{
			if (SelectStart != SelectEnd)
			{
				SortSelection();
				Clamp();
				CursorIndex = SelectEnd;
				SelectStart = SelectEnd;
				HasPreferredX = false;
			}
		}

		private static bool IsSpace(int codepoint)
		{
			return char.IsWhiteSpace((char)codepoint);
		}

		public bool IsWordBoundary(int idx)
		{
			return idx > 0 ? IsSpace(Text[idx - 1]) && !IsSpace(Text[idx]) : true;
		}

		public int MoveToPreviousWord(int c)
		{
			--c;
			while (c >= 0 && !IsWordBoundary(c))
				--c;
			if (c < 0)
				c = 0;
			return c;
		}

		public int MoveToNextWord(int c)
		{
			var len = Length;
			++c;
			while (c < len && !IsWordBoundary(c))
				++c;
			if (c > len)
				c = len;
			return c;
		}

		public bool Cut()
		{
			if (SelectStart != SelectEnd)
			{
				DeleteSelection();
				HasPreferredX = false;
				return true;
			}

			return false;
		}

		private bool PasteInternal(string text)
		{
			Clamp();
			DeleteSelection();
			if (InsertChars(CursorIndex, text) != 0)
			{
				UndoStack.MakeInsert(CursorIndex, string.IsNullOrEmpty(text) ? 0 : text.Length);
				CursorIndex += text.Length;
				HasPreferredX = false;
				return true;
			}

			return false;
		}

		public void InputChar(char ch)
		{
			if (ch == '\n' && SingleLine)
				return;
			if (InsertMode && !(SelectStart != SelectEnd) && CursorIndex < Length)
			{
				UndoStack.MakeReplace(Text, CursorIndex, 1, 1);
				DeleteChars(CursorIndex, 1);
				if (InsertChar(CursorIndex, ch) != 0)
				{
					++CursorIndex;
					HasPreferredX = false;
				}
			}
			else
			{
				DeleteSelection();
				if (InsertChar(CursorIndex, ch) != 0)
				{
					UndoStack.MakeInsert(CursorIndex, 1);
					++CursorIndex;
					HasPreferredX = false;
				}
			}
		}

		public void Replace(int where, int len, string text)
		{
			UndoStack.MakeReplace(Text, where, len, string.IsNullOrEmpty(text) ? 0 : text.Length);
			DeleteChars(where, len);
			if (InsertChars(where, text) != 0)
			{
				HasPreferredX = false;
			}
		}

		public void Key(ControlKeys key)
		{
		retry:
			switch (key)
			{
				case ControlKeys.InsertMode:
					InsertMode = !InsertMode;
					break;
				case ControlKeys.Undo:
					Undo();
					HasPreferredX = false;
					break;
				case ControlKeys.Redo:
					Redo();
					HasPreferredX = false;
					break;
				case ControlKeys.Left:
					if (SelectStart != SelectEnd)
						MoveToFirst();
					else if (CursorIndex > 0)
						--CursorIndex;
					HasPreferredX = false;
					break;
				case ControlKeys.Right:
					if (SelectStart != SelectEnd)
						MoveToLast();
					else
						++CursorIndex;
					Clamp();
					HasPreferredX = false;
					break;
				case ControlKeys.Left | ControlKeys.Shift:
					Clamp();
					PrepareSelectionAtCursor();
					if (SelectEnd > 0)
						--SelectEnd;
					CursorIndex = SelectEnd;
					HasPreferredX = false;
					break;
				case ControlKeys.WordLeft:
					if (SelectStart != SelectEnd)
					{
						MoveToFirst();
					}
					else
					{
						CursorIndex = MoveToPreviousWord(CursorIndex);
						Clamp();
					}

					break;
				case ControlKeys.WordLeft | ControlKeys.Shift:
					if (SelectStart == SelectEnd)
						PrepareSelectionAtCursor();
					CursorIndex = MoveToPreviousWord(CursorIndex);
					SelectEnd = CursorIndex;
					Clamp();
					break;
				case ControlKeys.WordRight:
					if (SelectStart != SelectEnd)
					{
						MoveToLast();
					}
					else
					{
						CursorIndex = MoveToNextWord(CursorIndex);
						Clamp();
					}

					break;
				case ControlKeys.WordRight | ControlKeys.Shift:
					if (SelectStart == SelectEnd)
						PrepareSelectionAtCursor();
					CursorIndex = MoveToNextWord(CursorIndex);
					SelectEnd = CursorIndex;
					Clamp();
					break;
				case ControlKeys.Right | ControlKeys.Shift:
					PrepareSelectionAtCursor();
					++SelectEnd;
					Clamp();
					CursorIndex = SelectEnd;
					HasPreferredX = false;
					break;
				case ControlKeys.Down:
				case ControlKeys.Down | ControlKeys.Shift:
				{
					var find = new FindState();
					var row = new TextEditRow();
					var sel = (key & ControlKeys.Shift) != 0;
					if (SingleLine)
					{
						key = ControlKeys.Right | (key & ControlKeys.Shift);
						goto retry;
					}

					if (sel)
						PrepareSelectionAtCursor();
					else if (SelectStart != SelectEnd)
						MoveToLast();
					Clamp();
					find.FindCharPosition(this, CursorIndex, SingleLine);
					if (find.length != 0)
					{
						var goal_x = HasPreferredX ? PreferredX : find.x;
						float x = 0;
						var start = find.first_char + find.length;
						CursorIndex = start;
						row = Handler.LayoutRow(CursorIndex);
						x = row.x0;
						for (var i = 0; i < row.num_chars; ++i)
						{
							var dx = Handler.GetWidth(start + i);
							if (dx == Handler.NewLineWidth)
								break;

							x += dx;
							if (x > goal_x)
								break;
							++CursorIndex;
						}

						Clamp();
						HasPreferredX = true;
						PreferredX = goal_x;
						if (sel)
							SelectEnd = CursorIndex;
					}

					break;
				}
				case ControlKeys.Up:
				case ControlKeys.Up | ControlKeys.Shift:
				{
					var find = new FindState();
					var row = new TextEditRow();
					var i = 0;
					var sel = (key & ControlKeys.Shift) != 0;
					if (SingleLine)
					{
						key = ControlKeys.Left | (key & ControlKeys.Shift);
						goto retry;
					}

					if (sel)
						PrepareSelectionAtCursor();
					else if (SelectStart != SelectEnd)
						MoveToFirst();
					Clamp();
					find.FindCharPosition(this, CursorIndex, SingleLine);
					if (find.prev_first != find.first_char)
					{
						var goal_x = HasPreferredX ? PreferredX : find.x;
						float x = 0;
						CursorIndex = find.prev_first;
						row = Handler.LayoutRow(CursorIndex);
						x = row.x0;
						for (i = 0; i < row.num_chars; ++i)
						{
							var dx = Handler.GetWidth(find.prev_first + i);
							if (dx == Handler.NewLineWidth)
								break;
							x += dx;
							if (x > goal_x)
								break;
							++CursorIndex;
						}

						Clamp();
						HasPreferredX = true;
						PreferredX = goal_x;
						if (sel)
							SelectEnd = CursorIndex;
					}

					break;
				}
				case ControlKeys.Delete:
				case ControlKeys.Delete | ControlKeys.Shift:
					if (SelectStart != SelectEnd)
					{
						DeleteSelection();
					}
					else
					{
						var n = Length;
						if (CursorIndex < n)
							Delete(CursorIndex, 1);
					}

					HasPreferredX = false;
					break;
				case ControlKeys.BackSpace:
				case ControlKeys.BackSpace | ControlKeys.Shift:
					if (SelectStart != SelectEnd)
					{
						DeleteSelection();
					}
					else
					{
						Clamp();
						if (CursorIndex > 0)
						{
							Delete(CursorIndex - 1, 1);
							--CursorIndex;
						}
					}

					HasPreferredX = false;
					break;
				case ControlKeys.TextStart:
					CursorIndex = SelectStart = SelectEnd = 0;
					HasPreferredX = false;
					break;
				case ControlKeys.TextEnd:
					CursorIndex = Length;
					SelectStart = SelectEnd = 0;
					HasPreferredX = false;
					break;
				case ControlKeys.TextStart | ControlKeys.Shift:
					PrepareSelectionAtCursor();
					CursorIndex = SelectEnd = 0;
					HasPreferredX = false;
					break;
				case ControlKeys.TextEnd | ControlKeys.Shift:
					PrepareSelectionAtCursor();
					CursorIndex = SelectEnd = Length;
					HasPreferredX = false;
					break;
				case ControlKeys.LineStart:
					Clamp();
					MoveToFirst();
					if (SingleLine)
						CursorIndex = 0;
					else
						while (CursorIndex > 0 && Text[CursorIndex - 1] != '\n')
							--CursorIndex;
					HasPreferredX = false;
					break;
				case ControlKeys.LineEnd:
				{
					var n = Length;
					Clamp();
					MoveToFirst();
					if (SingleLine)
						CursorIndex = n;
					else
						while (CursorIndex < n && Text[CursorIndex] != '\n')
							++CursorIndex;
					HasPreferredX = false;
					break;
				}
				case ControlKeys.LineStart | ControlKeys.Shift:
					Clamp();
					PrepareSelectionAtCursor();
					if (SingleLine)
						CursorIndex = 0;
					else
						while (CursorIndex > 0 && Text[CursorIndex - 1] != '\n')
							--CursorIndex;
					SelectEnd = CursorIndex;
					HasPreferredX = false;
					break;
				case ControlKeys.LineEnd | ControlKeys.Shift:
				{
					var n = Length;
					Clamp();
					PrepareSelectionAtCursor();
					if (SingleLine)
						CursorIndex = n;
					else
						while (CursorIndex < n && Text[CursorIndex] != '\n')
							++CursorIndex;
					SelectEnd = CursorIndex;
					HasPreferredX = false;
					break;
				}
			}
		}

		public void Undo()
		{
			if (UndoStack.Stack.Count == 0)
			{
				return;
			}

			var record = UndoStack.Stack.Pop();
			try
			{
				_suppressRedoStackReset = true;
				switch (record.OperationType)
				{
					case OperationType.Insert:
						RedoStack.MakeDelete(Text, record.Where, record.Length);
						DeleteChars(record.Where, record.Length);
						CursorIndex = record.Where;
						break;
					case OperationType.Delete:
						var length = InsertChars(record.Where, record.Data);
						RedoStack.MakeInsert(record.Where, length);
						CursorIndex = record.Where + length;
						break;
					case OperationType.Replace:
						RedoStack.MakeReplace(Text, record.Where, record.Length, record.Data.Length());
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

		public void Redo()
		{
			if (RedoStack.Stack.Count == 0)
			{
				return;
			}

			var record = RedoStack.Stack.Pop();

			try
			{
				_suppressRedoStackReset = true;

				switch (record.OperationType)
				{
					case OperationType.Insert:
						UndoStack.MakeDelete(Text, record.Where, record.Length);
						DeleteChars(record.Where, record.Length);
						CursorIndex = record.Where;
						break;
					case OperationType.Delete:
						var length = InsertChars(record.Where, record.Data);
						UndoStack.MakeInsert(record.Where, length);
						CursorIndex = record.Where + length;
						break;
					case OperationType.Replace:
						UndoStack.MakeReplace(Text, record.Where, record.Length, record.Data.Length());
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

		public bool Paste(string ctext)
		{
			return PasteInternal(ctext);
		}
	}
}