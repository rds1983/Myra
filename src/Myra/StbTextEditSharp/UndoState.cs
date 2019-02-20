using System;

namespace StbTextEditSharp
{
	public class UndoState
	{
		public int redo_char_point;
		public short redo_point;
		public int[] undo_char = new int[999];
		public int undo_char_point;
		public short undo_point;
		public UndoRecord[] undo_rec = new UndoRecord[99];

		public void FlushRedo()
		{
			redo_point = 99;
			redo_char_point = 999;
		}

		public void DiscardUndo()
		{
			if (undo_point > 0)
			{
				if (undo_rec[0].char_storage >= 0)
				{
					var n = undo_rec[0].insert_length;
					undo_char_point -= n;

					Array.Copy(undo_char, n, undo_char, 0, undo_char_point);
					for (var i = 0; i < undo_point; ++i)
						if (undo_rec[i].char_storage >= 0)
							undo_rec[i].char_storage -= n;
				}

				--undo_point;

				Array.Copy(undo_rec, 1, undo_rec, 0, undo_point);
			}
		}

		public void DiscardRedo()
		{
			int num;
			var k = 99 - 1;
			if (redo_point <= k)
			{
				if (undo_rec[k].char_storage >= 0)
				{
					var n = undo_rec[k].insert_length;
					int i;
					redo_char_point += n;
					num = 999 - redo_char_point;

					Array.Copy(undo_char, redo_char_point - n, undo_char, redo_char_point, num);
					for (i = (int)redo_point; i < k; ++i)
						if (undo_rec[i].char_storage >= 0)
							undo_rec[i].char_storage += n;
				}

				++redo_point;
				num = 99 - redo_point;
				if (num != 0) Array.Copy(undo_rec, redo_point, undo_rec, redo_point - 1, num);
			}
		}

		public int? CreateUndoRecord(int numchars)
		{
			FlushRedo();
			if (undo_point == 99)
				DiscardUndo();
			if (numchars > 999)
			{
				undo_point = 0;
				undo_char_point = 0;
				return null;
			}

			while (undo_char_point + numchars > 999) DiscardUndo();
			return undo_point++;
		}

		public int? CreateUndo(int pos, int insert_len, int delete_len)
		{
			var rpos = CreateUndoRecord(insert_len);
			if (rpos == null)
				return null;

			var rposv = rpos.Value;

			undo_rec[rposv].where = pos;
			undo_rec[rposv].insert_length = (short)insert_len;
			undo_rec[rposv].delete_length = (short)delete_len;
			if (insert_len == 0)
			{
				undo_rec[rposv].char_storage = -1;
				return null;
			}

			undo_rec[rposv].char_storage = (short)undo_char_point;
			undo_char_point = (short)(undo_char_point + insert_len);
			return undo_rec[rposv].char_storage;
		}
	}
}