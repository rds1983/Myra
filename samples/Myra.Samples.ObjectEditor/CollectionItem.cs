using System.ComponentModel;

namespace Myra.Samples.ObjectEditor
{
	public class CollectionItem
	{
		public string Text;

		[Category("Layout")]
		public int X;

		[Category("Layout")]
		public int Y;

		public override string ToString()
		{
			return string.Format("Text={0}, X={1}, Y={2}", Text, X, Y);
		}
	}
}
