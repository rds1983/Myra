using System;

namespace Myra.Samples.Inspector
{
    public class SomeNullableNumerics
    {
        public byte? @byte = null;
        public sbyte? @sbyte = 100;
        public short? @short = short.MaxValue;
        public ushort? @ushort = 13;
        public int? @int = null;
        public uint? @uint = 353;
        public long? @long = null;
        public ulong? @ulong = ulong.MaxValue;
        public float? @float = 1.14f;
        public double? @double = Math.PI * 40;
        public decimal? @decimal = decimal.MinusOne;
    }
}