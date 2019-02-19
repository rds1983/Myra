using System;

namespace StbTextEditSharp
{
	public class TextEdit
	{
		internal readonly ITextEditHandler Handler;

		public int cursor;
		public int select_start;
		public int select_end;
		public bool insert_mode;
		public bool cursor_at_end_of_line;
		public bool initialized;
		public bool has_preferred_x;
		public bool single_line;
		public byte padding1;
		public byte padding2;
		public byte padding3;
		public float preferred_x;
		public UndoState undostate;

		public int Length
		{
			get
			{
				return Handler.Length;
			}
		}

		public string text
		{
			get
			{
				return Handler.Text;
			}

			set
			{
				Handler.Text = value;
			}
		}

		public TextEdit(ITextEditHandler handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}

			Handler = handler;

			undostate = new UndoState();
			ClearState(false);
		}

		public void SortSelection()
		{
			if ((select_end) < (select_start))
			{
				int temp = (int)(select_end);
				select_end = (int)(select_start);
				select_start = (int)(temp);
			}
		}

		public void MoveToFirst()
		{
			if ((select_start != select_end))
			{
				SortSelection();
				cursor = (int)(select_start);
				select_end = (int)(select_start);
				has_preferred_x = false;
			}
		}

		public void PrepareSelectionAtCursor()
		{
			if (!(select_start != select_end))
				select_start = (int)(select_end = (int)(cursor));
			else
				cursor = (int)(select_end);
		}

		public void MakeUndoInsert(int where, int length)
		{
			undostate.CreateUndo(where, 0, length);
		}

		public void ClearState(bool is_single_line)
		{
			undostate.undo_point = (short)(0);
			undostate.undo_char_point = (int)(0);
			undostate.redo_point = (short)(99);
			undostate.redo_char_point = (int)(999);
			select_end = (int)(select_start = (int)(0));
			cursor = (int)(0);
			has_preferred_x = false;
			preferred_x = (float)(0);
			cursor_at_end_of_line = false;
			initialized = true;
			single_line = is_single_line;
			insert_mode = false;
		}

		public void DeleteChars(int pos, int l)
		{
			if (l == 0)
				return;

			text = text.Substring(0, pos) + text.Substring(pos + l);
		}

		public int InsertChars(int pos, int[] codepoints, int start, int legth)
		{
			// TODO
			return 0;
		}

		public int InsertChars(int pos, string s)
		{
			if (string.IsNullOrEmpty(s))
				return 0;

			if (text == null)
				text = s;
			else
				text = text.Substring(0, pos) + s + text.Substring(pos);

			return s.Length;
		}

		public int InsertChar(int pos, int codepoint)
		{
			var s = char.ConvertFromUtf32(codepoint);
			return InsertChars(pos, s);
		}

		public int LocateCoord(float x, float y)
		{
			TextEditRow r = new TextEditRow();
			int n = (int)(Length);
			float base_y = (float)(0);
			float prev_x = 0;
			int i = (int)(0);
			int k = 0;
			r.x0 = (float)(r.x1 = (float)(0));
			r.ymin = (float)(r.ymax = (float)(0));
			r.num_chars = (int)(0);
			while ((i) < (n))
			{
				r = Handler.LayoutRow((int)(i));
				if (r.num_chars <= 0)
					return (int)(n);
				if (((i) == (0)) && ((y) < (base_y + r.ymin)))
					return (int)(0);
				if ((y) < (base_y + r.ymax))
					break;
				i += (int)(r.num_chars);
				base_y += (float)(r.baseline_y_delta);
			}
			if ((i) >= (n))
				return (int)(n);
			if ((x) < (r.x0))
				return (int)(i);
			if ((x) < (r.x1))
			{
				prev_x = (float)(r.x0);
				for (k = (int)(0); (k) < (r.num_chars); ++k)
				{
					float w = Handler.GetWidth(i + k);
					if ((x) < (prev_x + w))
					{
						if ((x) < (prev_x + w / 2))
							return (int)(k + i);
						else
							return (int)(k + i + 1);
					}
					prev_x += (float)(w);
				}
			}

			if ((text[i + r.num_chars - 1]) == ('\n'))
				return (int)(i + r.num_chars - 1);
			else
				return (int)(i + r.num_chars);
		}

		public void Click(float x, float y)
		{
			if (single_line)
			{
				TextEditRow r = new TextEditRow();
				r = Handler.LayoutRow((int)(0));
				y = (float)(r.ymin);
			}

			cursor = (int)(LocateCoord((float)(x), (float)(y)));
			select_start = (int)(cursor);
			select_end = (int)(cursor);
			has_preferred_x = false;
		}

		public void Drag(float x, float y)
		{
			int p = (int)(0);
			if (single_line)
			{
				TextEditRow r = new TextEditRow();
				r = Handler.LayoutRow((int)(0));
				y = (float)(r.ymin);
			}

			if ((select_start) == (select_end))
				select_start = (int)(cursor);
			p = (int)(LocateCoord((float)(x), (float)(y)));
			cursor = (int)(select_end = (int)(p));
		}

		public void Clamp()
		{
			int n = (int)(Length);
			if ((select_start != select_end))
			{
				if ((select_start) > (n))
					select_start = (int)(n);
				if ((select_end) > (n))
					select_end = (int)(n);
				if ((select_start) == (select_end))
					cursor = (int)(select_start);
			}

			if ((cursor) > (n))
				cursor = (int)(n);
		}

		public void Delete(int where, int len)
		{
			MakeUndoDelete((int)(where), (int)(len));
			DeleteChars((int)(where), (int)(len));
			has_preferred_x = false;
		}

		public void DeleteSelection()
		{
			Clamp();
			if ((select_start != select_end))
			{
				if ((select_start) < (select_end))
				{
					Delete((int)(select_start), (int)(select_end - select_start));
					select_end = (int)(cursor = (int)(select_start));
				}
				else
				{
					Delete((int)(select_end), (int)(select_start - select_end));
					select_start = (int)(cursor = (int)(select_end));
				}
				has_preferred_x = false;
			}

		}

		public void MoveToLast()
		{
			if ((select_start != select_end))
			{
				SortSelection();
				Clamp();
				cursor = (int)(select_end);
				select_start = (int)(select_end);
				has_preferred_x = false;
			}
		}

		private static bool IsSpace(int codepoint)
		{
			return char.IsWhiteSpace((char)codepoint);
		}

		public bool IsWordBoundary(int idx)
		{
			return (idx) > (0) ? (((IsSpace((int)(text[idx - 1])))) && (!IsSpace((int)(text[idx])))) : true;
		}

		public int MoveToPreviousWord(int c)
		{
			--c;
			while (((c) >= (0)) && (!IsWordBoundary((int)(c))))
			{
				--c;
			}
			if ((c) < (0))
				c = (int)(0);
			return (int)(c);
		}

		public int MoveToNextWord(int c)
		{
			int len = (int)(Length);
			++c;
			while (((c) < (len)) && (!IsWordBoundary((int)(c))))
			{
				++c;
			}
			if ((c) > (len))
				c = (int)(len);
			return (int)(c);
		}

		public int Cut()
		{
			if ((select_start != select_end))
			{
				DeleteSelection();
				has_preferred_x = false;
				return (int)(1);
			}

			return (int)(0);
		}

		private int PasteInternal(string text)
		{
			Clamp();
			DeleteSelection();
			if ((InsertChars((int)(cursor), text)) != 0)
			{
				MakeUndoInsert((int)(cursor), text.Length);
				cursor += text.Length;
				has_preferred_x = false;
				return (int)(1);
			}

			if ((undostate.undo_point) != 0)
				--undostate.undo_point;
			return (int)(0);
		}

		public void InputChar(char ch)
		{
			if (((ch) == ('\n')) && (single_line))
				return;
			if (((insert_mode) && (!(select_start != select_end))) && ((cursor) < (Length)))
			{
				MakeUndoReplace((int)(cursor), (int)(1), (int)(1));
				DeleteChars((int)(cursor), (int)(1));
				if ((InsertChar((int)(cursor), ch)) != 0)
				{
					++cursor;
					has_preferred_x = false;
				}
			}
			else
			{
				DeleteSelection();
				if ((InsertChar((int)(cursor), ch)) != 0)
				{
					MakeUndoInsert((int)(cursor), (int)(1));
					++cursor;
					has_preferred_x = false;
				}
			}
		}

		public void Key(ControlKeys key)
		{
		retry:
			switch (key)
			{
				case ControlKeys.InsertMode:
					insert_mode = !insert_mode;
					break;
				case ControlKeys.Undo:
					Undo();
					has_preferred_x = false;
					break;
				case ControlKeys.Redo:
					Redo();
					has_preferred_x = false;
					break;
				case ControlKeys.Left:
					if ((select_start != select_end))
						MoveToFirst();
					else if ((cursor) > (0))
						--cursor;
					has_preferred_x = false;
					break;
				case ControlKeys.Right:
					if ((select_start != select_end))
						MoveToLast();
					else
						++cursor;
					Clamp();
					has_preferred_x = false;
					break;
				case ControlKeys.Left | ControlKeys.Shift:
					Clamp();
					PrepareSelectionAtCursor();
					if ((select_end) > (0))
						--select_end;
					cursor = (int)(select_end);
					has_preferred_x = false;
					break;
				case ControlKeys.WordLeft:
					if ((select_start != select_end))
						MoveToFirst();
					else
					{
						cursor = (int)(MoveToPreviousWord((int)(cursor)));
						Clamp();
					}
					break;
				case ControlKeys.WordLeft | ControlKeys.Shift:
					if (!(select_start != select_end))
						PrepareSelectionAtCursor();
					cursor = (int)(MoveToPreviousWord((int)(cursor)));
					select_end = (int)(cursor);
					Clamp();
					break;
				case ControlKeys.WordRight:
					if ((select_start != select_end))
						MoveToLast();
					else
					{
						cursor = (int)(MoveToNextWord((int)(cursor)));
						Clamp();
					}
					break;
				case ControlKeys.WordRight | ControlKeys.Shift:
					if (!(select_start != select_end))
						PrepareSelectionAtCursor();
					cursor = (int)(MoveToNextWord((int)(cursor)));
					select_end = (int)(cursor);
					Clamp();
					break;
				case ControlKeys.Right | ControlKeys.Shift:
					PrepareSelectionAtCursor();
					++select_end;
					Clamp();
					cursor = (int)(select_end);
					has_preferred_x = false;
					break;
				case ControlKeys.Down:
				case ControlKeys.Down | ControlKeys.Shift:
				{
					FindState find = new FindState();
					TextEditRow row = new TextEditRow();
					int i = 0;
					bool sel = ((key & ControlKeys.Shift) != 0);
					if (single_line)
					{
						key = (ControlKeys.Right | (key & ControlKeys.Shift));
						goto retry;
					}
					if ((sel))
						PrepareSelectionAtCursor();
					else if ((select_start != select_end))
						MoveToLast();
					Clamp();
					find.FindCharPosition(this, (int)(cursor), single_line);
					if ((find.length) != 0)
					{
						float goal_x = (float)(has_preferred_x ? preferred_x : find.x);
						float x = 0;
						int start = (int)(find.first_char + find.length);
						cursor = (int)(start);
						row = Handler.LayoutRow((int)(cursor));
						x = (float)(row.x0);
						for (i = (int)(0); (i) < (row.num_chars); ++i)
						{
							float dx = (float)(1);
							x += (float)(dx);
							if ((x) > (goal_x))
								break;
							++cursor;
						}
						Clamp();
						has_preferred_x = true;
						preferred_x = (float)(goal_x);
						if ((sel))
							select_end = (int)(cursor);
					}
					break;
				}
				case ControlKeys.Up:
				case ControlKeys.Up | ControlKeys.Shift:
				{
					FindState find = new FindState();
					TextEditRow row = new TextEditRow();
					int i = 0;
					bool sel = ((key & ControlKeys.Shift) != 0);
					if (single_line)
					{
						key = (ControlKeys.Left | (key & ControlKeys.Shift));
						goto retry;
					}
					if (sel)
						PrepareSelectionAtCursor();
					else if ((select_start != select_end))
						MoveToFirst();
					Clamp();
					find.FindCharPosition(this, (int)(cursor), single_line);
					if (find.prev_first != find.first_char)
					{
						float goal_x = (float)(has_preferred_x ? preferred_x : find.x);
						float x = 0;
						cursor = (int)(find.prev_first);
						row = Handler.LayoutRow((int)(cursor));
						x = (float)(row.x0);
						for (i = (int)(0); (i) < (row.num_chars); ++i)
						{
							float dx = (float)(1);
							x += (float)(dx);
							if ((x) > (goal_x))
								break;
							++cursor;
						}
						Clamp();
						has_preferred_x = true;
						preferred_x = (float)(goal_x);
						if (sel)
							select_end = (int)(cursor);
					}
					break;
				}
				case ControlKeys.Delete:
				case ControlKeys.Delete | ControlKeys.Shift:
					if ((select_start != select_end))
						DeleteSelection();
					else
					{
						int n = (int)(Length);
						if ((cursor) < (n))
							Delete((int)(cursor), (int)(1));
					}
					has_preferred_x = false;
					break;
				case ControlKeys.BackSpace:
				case ControlKeys.BackSpace | ControlKeys.Shift:
					if ((select_start != select_end))
						DeleteSelection();
					else
					{
						Clamp();
						if ((cursor) > (0))
						{
							Delete((int)(cursor - 1), (int)(1));
							--cursor;
						}
					}
					has_preferred_x = false;
					break;
				case ControlKeys.TextStart:
					cursor = (int)(select_start = (int)(select_end = (int)(0)));
					has_preferred_x = false;
					break;
				case ControlKeys.TextEnd:
					cursor = (int)(Length);
					select_start = (int)(select_end = (int)(0));
					has_preferred_x = false;
					break;
				case ControlKeys.TextStart | ControlKeys.Shift:
					PrepareSelectionAtCursor();
					cursor = (int)(select_end = (int)(0));
					has_preferred_x = false;
					break;
				case ControlKeys.TextEnd | ControlKeys.Shift:
					PrepareSelectionAtCursor();
					cursor = (int)(select_end = (int)(Length));
					has_preferred_x = false;
					break;
				case ControlKeys.LineStart:
					Clamp();
					MoveToFirst();
					if (single_line)
						cursor = (int)(0);
					else
						while (((cursor) > (0)) && ((text[cursor - 1]) != '\n'))
						{
							--cursor;
						}
					has_preferred_x = false;
					break;
				case ControlKeys.LineEnd:
				{
					int n = (int)(Length);
					Clamp();
					MoveToFirst();
					if (single_line)
						cursor = (int)(n);
					else
						while (((cursor) < (n)) && ((text[cursor]) != '\n'))
						{
							++cursor;
						}
					has_preferred_x = false;
					break;
				}
				case ControlKeys.LineStart | ControlKeys.Shift:
					Clamp();
					PrepareSelectionAtCursor();
					if (single_line)
						cursor = (int)(0);
					else
						while (((cursor) > (0)) && ((text[cursor - 1]) != '\n'))
						{
							--cursor;
						}
					select_end = (int)(cursor);
					has_preferred_x = false;
					break;
				case ControlKeys.LineEnd | ControlKeys.Shift:
				{
					int n = (int)(Length);
					Clamp();
					PrepareSelectionAtCursor();
					if (single_line)
						cursor = (int)(n);
					else
						while (((cursor) < (n)) && ((text[cursor]) != '\n'))
						{
							++cursor;
						}
					select_end = (int)(cursor);
					has_preferred_x = false;
					break;
				}
			}
		}

		public void Undo()
		{
			UndoState s = undostate;
			UndoRecord u = new UndoRecord();
			if ((s.undo_point) == (0))
				return;
			u = (UndoRecord)(s.undo_rec[s.undo_point - 1]);
			int rpos = s.redo_point - 1;
			s.undo_rec[rpos].char_storage = (int)(-1);
			s.undo_rec[rpos].insert_length = (int)(u.delete_length);
			s.undo_rec[rpos].delete_length = (int)(u.insert_length);
			s.undo_rec[rpos].where = (int)(u.where);
			if ((u.delete_length) != 0)
			{
				if ((s.undo_char_point + u.delete_length) >= (999))
				{
					s.undo_rec[rpos].insert_length = (int)(0);
				}
				else
				{
					int i = 0;
					while ((s.undo_char_point + u.delete_length) > (s.redo_char_point))
					{
						if ((s.redo_point) == (99))
							return;
						s.DiscardRedo();
					}
					rpos = s.redo_point - 1;
					s.undo_rec[rpos].char_storage = (int)(s.redo_char_point - u.delete_length);
					s.redo_char_point = (int)(s.redo_char_point - u.delete_length);
					for (i = (int)(0); (i) < (u.delete_length); ++i)
					{
						s.undo_char[s.undo_rec[rpos].char_storage + i] = (sbyte)(text[u.where + i]);
					}
				}
				DeleteChars((int)(u.where), (int)(u.delete_length));
			}

			if ((u.insert_length) != 0)
			{
				InsertChars((int)(u.where), s.undo_char, u.char_storage, (int)(u.insert_length));
				s.undo_char_point -= (int)(u.insert_length);
			}

			cursor = (int)(u.where + u.insert_length);
			s.undo_point--;
			s.redo_point--;
		}

		public void Redo()
		{
			UndoState s = undostate;
			UndoRecord r = new UndoRecord();
			if ((s.redo_point) == (99))
				return;
			int upos = s.undo_point;
			r = (UndoRecord)(s.undo_rec[s.redo_point]);
			s.undo_rec[upos].delete_length = (int)(r.insert_length);
			s.undo_rec[upos].insert_length = (int)(r.delete_length);
			s.undo_rec[upos].where = (int)(r.where);
			s.undo_rec[upos].char_storage = (int)(-1);

			var u = s.undo_rec[upos];
			if ((r.delete_length) != 0)
			{
				if ((s.undo_char_point + u.insert_length) > (s.redo_char_point))
				{
					s.undo_rec[upos].insert_length = (int)(0);
					s.undo_rec[upos].delete_length = (int)(0);
				}
				else
				{
					int i = 0;
					s.undo_rec[upos].char_storage = (int)(s.undo_char_point);
					s.undo_char_point = (int)(s.undo_char_point + u.insert_length);
					for (i = (int)(0); (i) < (u.insert_length); ++i)
					{
						s.undo_char[u.char_storage + i] = (sbyte)(text[u.where + i]);
					}
				}
				DeleteChars((int)(r.where), (int)(r.delete_length));
			}

			if ((r.insert_length) != 0)
			{
				InsertChars((int)(r.where), s.undo_char, r.char_storage, (int)(r.insert_length));
				s.redo_char_point += (int)(r.insert_length);
			}

			cursor = (int)(r.where + r.insert_length);
			s.undo_point++;
			s.redo_point++;
		}

		public void MakeUndoDelete(int where, int length)
		{
			int i;
			int? p = undostate.CreateUndo((int)(where), (int)(length), (int)(0));
			if ((p) != null)
			{
				for (i = (int)(0); (i) < (length); ++i)
				{
					undostate.undo_char[p.Value + i] = (char)(text[(int)(where + i)]);
				}
			}
		}

		public void MakeUndoReplace(int where, int old_length, int new_length)
		{
			int i;
			int? p = undostate.CreateUndo((int)(where), (int)(old_length), (int)(new_length));
			if ((p) != null)
			{
				for (i = (int)(0); (i) < (old_length); ++i)
				{
					undostate.undo_char[p.Value + i] = (char)(text[(int)(where + i)]);
				}
			}
		}

		public int Paste(string ctext)
		{
			return (int)(PasteInternal(ctext));
		}
	}
}