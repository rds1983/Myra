using System;

namespace Myra.Utility
{
	internal struct ColorHSV : IEquatable<ColorHSV>
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

        public override bool Equals(object obj)
        {
            return obj is ColorHSV hSV && Equals(hSV);
        }

        public bool Equals(ColorHSV other)
        {
            return H == other.H &&
                   S == other.S &&
                   V == other.V;
        }

        public override int GetHashCode()
        {
            int hashCode = -1397884734;
            hashCode *= -1521134295 + H.GetHashCode();
            hashCode *= -1521134295 + S.GetHashCode();
            hashCode *= -1521134295 + V.GetHashCode();
            return hashCode;
        }
    }
}
