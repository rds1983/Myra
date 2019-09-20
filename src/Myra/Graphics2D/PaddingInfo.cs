using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D
{
	public struct PaddingInfo
	{
		public static readonly PaddingInfo Zero = new PaddingInfo();

		public int Left { get; set; }
		public int Right { get; set; }
		public int Top { get; set; }
		public int Bottom { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public int Width
		{
			get { return Left + Right; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public int Height
		{
			get { return Top + Bottom; }
		}

		public static bool operator ==(PaddingInfo a, PaddingInfo b)
		{
			return a.Left == b.Left &&
			       a.Right == b.Right &&
			       a.Top == b.Top &&
			       a.Bottom == b.Bottom;
		}

		public static bool operator !=(PaddingInfo a, PaddingInfo b)
		{
			return !(a == b);
		}

		public override string ToString()
		{
			return string.Format("{{Left: {0}, Right: {1}, Top: {2}, Bottom: {3}}}",
				Left, Right, Top, Bottom);
		}
	}
}