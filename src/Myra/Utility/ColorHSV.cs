namespace Myra.Utility
{
	internal struct ColorHSV
	{
		public int H { get; set; }
		public int S { get; set; }
		public int V { get; set; }

		public static bool operator ==(ColorHSV a, ColorHSV b)
		{
			return Equals(a, b);
		}

		public static bool operator !=(ColorHSV a, ColorHSV b)
		{
			return !Equals(a, b);
		}

		public override bool Equals(object obj)
		{
			return obj is ColorHSV && Equals((ColorHSV)obj, this);
		}

		private static bool Equals(ColorHSV a, ColorHSV b)
		{
			return a.H == b.H && a.V == b.V && a.S == b.V;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = H.GetHashCode();
				hashCode = (hashCode * 397) ^ S.GetHashCode();
				hashCode = (hashCode * 397) ^ V.GetHashCode();
				return hashCode;
			}
		}
	}
}