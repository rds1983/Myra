namespace Myra.Utility
{
	public struct ColorHSV
	{
		public int H { get; set; }
		public int S { get; set; }
		public int V { get; set; }

		public static bool operator ==(ColorHSV a, ColorHSV b)
		{
			return a.H == b.H && a.V == b.V && a.S == b.V;
		}

		public static bool operator !=(ColorHSV a, ColorHSV b)
		{
			return !(a == b);
		}

	}
}
