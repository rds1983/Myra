#if NET7_0_OR_GREATER
#define MATH_IFACES
#else
#undef MATH_IFACES
#endif

using System;

#if MATH_IFACES
using System.Numerics;
#else
using Generic.Math;
#endif

namespace Myra.Utility.Types
{
    /// <summary>
    /// Generic math methods for <typeparamref name="TNum"/>.<para/>
    /// If project is less than .net7, uses Generic.Math library which relies on Reflection.Emit and codegen.
    /// </summary>
    internal static class MathHelper<TNum> where TNum : struct
    {
        // TODO Use generic math interfaces instead of GenericMath<TNum> if the dotnet version supports it.
        // The Generic.Math lib requires runtime codegen!

        private static readonly bool NumIsSigned;
        
        /// <summary>Value that represents 0 for <typeparamref name="TNum"/></summary>
        public static readonly TNum Zero;
        /// <summary>Value that represents 1 for <typeparamref name="TNum"/></summary>
        public static readonly TNum One;
        
        static MathHelper()
        {
            Type arg = typeof(TNum);
            TypeInfo info = TypeHelper<TNum>.Info;
            
            if(info.IsNullable)
                throw new ArgumentException($"Invalid Generic-Type Argument: '{arg}', Nullable types are not supported");
            if(!info.IsNumber)
                throw new ArgumentException($"Invalid Generic-Type Argument: '{arg}', Only numeric types are supported");
            if(arg == typeof(byte) || arg == typeof(sbyte))
                throw new ArgumentException($"Invalid Generic-Type Argument: '{arg}' does not have full math support. Convert to another type first");

            NumIsSigned = info.IsSignedNumber;
            Zero = GenericMath<TNum>.Zero;
            One = GenericMath<int, TNum>.Convert( 1 );
        }
        
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> + <paramref name="rhs"/>
        /// </summary>
        public static TNum Add(TNum lhs, TNum rhs) => Add_Internal(lhs, rhs);
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> += <paramref name="rhs"/>
        /// </summary>
        public static void Add(ref TNum lhs, TNum rhs) => lhs = Add_Internal(lhs, rhs);
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> - <paramref name="rhs"/>
        /// </summary>
        public static TNum Subtract(TNum lhs, TNum rhs) => Subtract_Internal(lhs, rhs);
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> -= <paramref name="rhs"/>
        /// </summary>
        public static void Subtract(ref TNum lhs, TNum rhs) => lhs = Subtract_Internal(lhs, rhs);
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> * <paramref name="rhs"/>
        /// </summary>
        public static TNum Multiply(TNum lhs, TNum rhs) => Multiply_Internal(lhs, rhs);
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> *= <paramref name="rhs"/>
        /// </summary>
        public static void Multiply(ref TNum lhs, TNum rhs) => lhs = Multiply_Internal(lhs, rhs);
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> / <paramref name="rhs"/>
        /// </summary>
        public static TNum Divide(TNum lhs, TNum rhs) => Divide_Internal(lhs, rhs);
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> /= <paramref name="rhs"/>
        /// </summary>
        public static void Divide(ref TNum lhs, TNum rhs) => lhs = Divide_Internal(lhs, rhs);

        private static TNum Add_Internal(TNum lhs, TNum rhs)
        {
            return GenericMath<TNum>.Add(lhs, rhs);
        }
        private static TNum Subtract_Internal(TNum lhs, TNum rhs)
        {
            return GenericMath<TNum>.Subtract(lhs, rhs);
        }
        private static TNum Multiply_Internal(TNum lhs, TNum rhs)
        {
            return GenericMath<TNum>.Multiply(lhs, rhs);
        }
        private static TNum Divide_Internal(TNum lhs, TNum rhs)
        {
            return GenericMath<TNum>.Divide(lhs, rhs);
        }
        
#region Compare
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> == <paramref name="rhs"/>
        /// </summary>
        public static bool Equal(TNum lhs, TNum rhs)
        {
            return GenericMath<TNum>.Equal(lhs, rhs);
        }
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> != <paramref name="rhs"/>
        /// </summary>
        public static bool NotEqual(TNum lhs, TNum rhs)
        {
            return GenericMath<TNum>.NotEqual(lhs, rhs);
        }
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> &lt; <paramref name="rhs"/>
        /// </summary>
        public static bool LessThan(TNum lhs, TNum rhs)
        {
            return GenericMath<TNum>.LessThan(lhs, rhs);
        }
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> &lt;= <paramref name="rhs"/>
        /// </summary>
        public static bool LessThanOrEqual(TNum lhs, TNum rhs)
        {
            return GenericMath<TNum>.LessThanOrEqual(lhs, rhs);
        }
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> &gt; <paramref name="rhs"/>
        /// </summary>
        public static bool GreaterThan(TNum lhs, TNum rhs)
        {
            return GenericMath<TNum>.GreaterThan(lhs, rhs);
        }
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> &gt;= <paramref name="rhs"/>
        /// </summary>
        public static bool GreaterThanOrEqual(TNum lhs, TNum rhs)
        {
            return GenericMath<TNum>.GreaterThanOrEqual(lhs, rhs);
        }
#endregion

        /// <summary>
        /// Returns the absolute positive value of <paramref name="value"/>.<para/>
        /// Returns <paramref name="value"/> unchanged if <typeparamref name="TNum"/> does not support negatives.
        /// </summary>
        public static TNum Abs(TNum value)
        {
            if (NumIsSigned && LessThan(value, Zero))
                value = Negate_Internal(value);
            return value;
        }
        /// <summary>
        /// Equivalent to operator: -<paramref name="value"/><para/>
        /// Returns <paramref name="value"/> unchanged if <typeparamref name="TNum"/> does not support negatives.
        /// </summary>
        public static TNum Negate(TNum value)
        {
            if(NumIsSigned)
                return Negate_Internal(value);
            return value;
        }
        private static TNum Negate_Internal(TNum value)
        {
            return GenericMath<TNum>.Negate(value);
        }
        
        /// <summary>
        /// Returns the smallest of two values.
        /// </summary>
        public static TNum Min(TNum lhs, TNum rhs)
            => LessThan(lhs, rhs) ? lhs : rhs;
        /// <summary>
        /// Returns the largest of two values.
        /// </summary>
        public static TNum Max(TNum lhs, TNum rhs)
            => GreaterThan(lhs, rhs) ? lhs : rhs;
        
        /// <summary>
        /// Clamp <paramref name="value"/> between <paramref name="minValue"/> and <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="value">The value to limit.</param>
        /// <param name="minValue">The minimum range. (inclusive) If null, there will be no lower limit applied.</param>
        /// <param name="maxValue">The maximum range. (inclusive) If null, there will be no upper limit applied.</param>
        public static TNum Clamp(TNum value, TNum? minValue, TNum? maxValue)
        {
            bool limitMin = minValue.HasValue, limitMax = maxValue.HasValue;
            if (limitMin & limitMax)
                return Clamp(value, minValue.Value, maxValue.Value);
            if (!limitMin & !limitMax)
                return value;
            
            // limitMin != limitMax...
            if (limitMin && LessThanOrEqual(value, minValue.Value))
                return minValue.Value;
            if (limitMax && GreaterThanOrEqual(value, maxValue.Value))
                return maxValue.Value;
            return value;
        }
        /// <summary>
        /// Clamp <paramref name="value"/> between <paramref name="minValue"/> and <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="value">The value to limit.</param>
        /// <param name="minValue">The minimum range. (inclusive)</param>
        /// <param name="maxValue">The maximum range. (inclusive)</param>
        public static TNum Clamp(TNum value, TNum minValue, TNum maxValue)
        {
            if (Equal(minValue, maxValue))
                return minValue;
            
            if(GreaterThan(minValue, maxValue))
                SwapValues(ref minValue, ref maxValue);
            
            if (LessThanOrEqual(value, minValue))
                return minValue;
            if (GreaterThanOrEqual(value, maxValue))
                return maxValue;
            return value;
        }
        private static void SwapValues(ref TNum a, ref TNum b)
        {
            TNum c = a;
            TNum d = b;
            b = c;
            a = d;
        }
    }

    internal static class MathHelper<TNum, TResult>
    {
        public static TResult Convert(TNum value)
        {
#if MATH_IFACES
            throw new NotImplementedException();
#else
            return GenericMath<TNum, TResult>.Convert(value);
#endif
        }
    }
}