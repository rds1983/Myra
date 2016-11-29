using Newtonsoft.Json;

namespace Myra.Graphics2D
{
	public struct PaddingInfo
	{
		public static readonly PaddingInfo Zero = new PaddingInfo();

		public int Left, Right, Top, Bottom;

		[JsonIgnore]
		public int Width
		{
			get { return Left + Right; }
		}

		[JsonIgnore]
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