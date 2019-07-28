using System.Runtime.InteropServices;

namespace StbTextEditSharp
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