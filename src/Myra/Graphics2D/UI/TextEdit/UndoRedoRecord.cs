using System.Runtime.InteropServices;

namespace Myra.Graphics2D.UI.TextEdit
{
	internal enum OperationType
	{
		Insert,
		Delete,
		Replace
	}

	[StructLayout(LayoutKind.Sequential)]
	internal class UndoRedoRecord
	{
		public OperationType OperationType;
		public string Data;
		public int Where;
		public int Length;
	}
}