using System;

namespace Myra.Samples.Inspector
{
    public class SomeNumerics
    {
        public byte @byte = 250;
        public sbyte @sbyte = -43;
        public short @short = short.MaxValue - 4;
        public ushort @ushort = 333;
        public int @int = -30;
        public uint @uint = 30;
        public long @long = 0;
        public ulong @ulong = ulong.MaxValue - 4;
        public float @float = 1.0f;
        public double @double = Math.PI;
        public decimal @decimal = decimal.MinusOne;
    }
}