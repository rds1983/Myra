using System.Collections.Generic;

namespace StbTextEditSharp
{
	internal class UndoRedoStack
	{
		private readonly Stack<UndoRedoRecord> _stack = new Stack<UndoRedoRecord>();

		public Stack<UndoRedoRecord> Stack
		{
			get
			{
				return _stack;
			}
		}

		public void Reset()
		{
			_stack.Clear();
		}

		public void MakeInsert(int where, int length)
		{
			if (length <= 0)
			{
				return;
			}

			var record = new UndoRedoRecord
			{
				OperationType = OperationType.Insert,
				Where = where,
				Length = length
			};

			_stack.Push(record);
		}

		public void MakeDelete(string text, int where, int length)
		{
			if (length <= 0)
			{
				return;
			}

			var record = new UndoRedoRecord
			{
				OperationType = OperationType.Delete,
				Where = where,
				Length = length,
				Data = text.Substring(where, length)
			};

			_stack.Push(record);
		}

		public void MakeReplace(string text, int where, int length, int newLength)
		{
			if (length <= 0)
			{
				MakeInsert(where, newLength);
				return;
			}

			if (newLength <= 0)
			{
				MakeDelete(text, where, length);
				return;
			}

			var record = new UndoRedoRecord
			{
				OperationType = OperationType.Replace,
				Where = where,
				Length = newLength,
				Data = text.Substring(where, length)
			};

			_stack.Push(record);
		}
	}
}