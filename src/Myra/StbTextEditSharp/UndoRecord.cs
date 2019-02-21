using System.Runtime.InteropServices;

namespace StbTextEditSharp
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct UndoRecord
	{
		public int where;
		public int insert_length;
		public int delete_length;
		public int char_storage;
	}
}