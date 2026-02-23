using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
    internal static class MathHelper<TNum>
#if MATH_IFACES
        where TNum : struct, INumber<TNum>, IMinMaxValue<TNum>
#else
        where TNum : struct
#endif
    {
        private static readonly bool NumIsSigned;
        
        /// <summary>Value that represents 0 for <typeparamref name="TNum"/></summary>
        public static readonly TNum Zero;
        /// <summary>Value that represents 1 for <typeparamref name="TNum"/></summary>
        public static readonly TNum One;
        
#if MATH_IFACES //TODO find a way to get these values without interfaces
        public static readonly TNum MinValue;
        public static readonly TNum MaxValue;
#endif
        
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

#if MATH_IFACES
            Zero = TNum.Zero;
            One = TNum.One;
            MinValue = TNum.MinValue;
            MaxValue = TNum.MaxValue;
#else
            Zero = GenericMath<TNum>.Zero;
            One = MathHelper<int, TNum>.Convert( 1 );
#endif
        }
        
#region Internals
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TNum Add_Internal(TNum lhs, TNum rhs)
        {
#if MATH_IFACES
            return lhs + rhs;
#else
            return GenericMath<TNum>.Add(lhs, rhs);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TNum Subtract_Internal(TNum lhs, TNum rhs)
        {
#if MATH_IFACES
            return lhs - rhs;
#else
            return GenericMath<TNum>.Subtract(lhs, rhs);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TNum Multiply_Internal(TNum lhs, TNum rhs)
        {
#if MATH_IFACES
            return lhs * rhs;
#else
            return GenericMath<TNum>.Multiply(lhs, rhs);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TNum Divide_Internal(TNum lhs, TNum rhs)
        {
#if MATH_IFACES
            return lhs / rhs;
#else
            return GenericMath<TNum>.Divide(lhs, rhs);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TNum Negate_Internal(TNum value)
        {
#if MATH_IFACES
            return -value;
#else
            return GenericMath<TNum>.Negate(value);
#endif
        }
        
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> == <paramref name="rhs"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal_Internal(TNum lhs, TNum rhs)
        {
#if MATH_IFACES
            return lhs.Equals(rhs);
#else
            return GenericMath<TNum>.Equal(lhs, rhs);
#endif
        }
        
#endregion Internals

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

#region Compare
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> == <paramref name="rhs"/>
        /// </summary>
        public static bool Equal(TNum lhs, TNum rhs) => Equal_Internal(lhs, rhs);
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> != <paramref name="rhs"/>
        /// </summary>
        public static bool UnEqual(TNum lhs, TNum rhs) => !Equal_Internal(lhs, rhs);
        
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> &lt; <paramref name="rhs"/>
        /// </summary>
        public static bool LessThan(TNum lhs, TNum rhs)
        {
#if MATH_IFACES
            return lhs < rhs;
#else
            return GenericMath<TNum>.LessThan(lhs, rhs);
#endif
        }
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> &lt;= <paramref name="rhs"/>
        /// </summary>
        public static bool LessThanOrEqual(TNum lhs, TNum rhs)
        {
#if MATH_IFACES
            return lhs <= rhs;
#else
            return GenericMath<TNum>.LessThanOrEqual(lhs, rhs);
#endif
        }
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> &gt; <paramref name="rhs"/>
        /// </summary>
        public static bool GreaterThan(TNum lhs, TNum rhs)
        {
#if MATH_IFACES
            return lhs > rhs;
#else
            return GenericMath<TNum>.GreaterThan(lhs, rhs);
#endif
        }
        /// <summary>
        /// Equivalent to operator: <paramref name="lhs"/> &gt;= <paramref name="rhs"/>
        /// </summary>
        public static bool GreaterThanOrEqual(TNum lhs, TNum rhs)
        {
#if MATH_IFACES
            return lhs >= rhs;
#else
            return GenericMath<TNum>.GreaterThanOrEqual(lhs, rhs);
#endif
        }
#endregion Compare

        /// <summary>
        /// Returns the absolute positive value of <paramref name="value"/>.<para/>
        /// Returns <paramref name="value"/> unchanged if <typeparamref name="TNum"/> does not support negatives.
        /// </summary>
        public static TNum Abs(TNum value)
        {
#if MATH_IFACES
            return TNum.Abs(value);
#else
            if (NumIsSigned && LessThan(value, Zero))
                value = Negate_Internal(value);
            return value;
#endif
        }

        /// <summary>
        /// Equivalent to operator: -<paramref name="value"/><para/>
        /// Returns <paramref name="value"/> unchanged if <typeparamref name="TNum"/> does not support negatives.
        /// </summary>
        public static TNum Negate(TNum value) => Negate_Internal(value);
        
        /// <summary>
        /// Returns the smallest of two values.
        /// </summary>
        public static TNum Min(TNum lhs, TNum rhs)
        {
            return LessThan(lhs, rhs) ? lhs : rhs;
        }
        /// <summary>
        /// Returns the smallest element in an array of values.
        /// </summary>
        public static TNum Min(params TNum[] values)
        {
            if (values == null || values.Length <= 0)
                throw new ArgumentException("Values array must have at least one element.");
            if (values.Length == 1)
                return values[0];
            
            TNum value = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (LessThan(values[i], value))
                {
                    value = values[i];
                }
            }
            return value;
        }
        /// <summary>
        /// Returns the smallest element in an array of values.
        /// </summary>
        public static TNum Min(IReadOnlyList<TNum> values)
        {
            if (values == null || values.Count <= 0)
                throw new ArgumentException("Values array must have at least one element.");
            if (values.Count == 1)
                return values[0];
            
            TNum value = values[0];
            for (int i = 1; i < values.Count; i++)
            {
                if (LessThan(values[i], value))
                {
                    value = values[i];
                }
            }
            return value;
        }

        /// <summary>
        /// Returns the largest of two values.
        /// </summary>
        public static TNum Max(TNum lhs, TNum rhs)
        {
            return GreaterThan(lhs, rhs) ? lhs : rhs;
        }
        /// <summary>
        /// Returns the largest element in an array of values.
        /// </summary>
        public static TNum Max(params TNum[] values)
        {
            if (values == null || values.Length <= 0)
                throw new ArgumentException("Values array must have at least one element.");
            if (values.Length == 1)
                return values[0];
            
            TNum value = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (GreaterThan(values[i], value))
                {
                    value = values[i];
                }
            }
            return value;
        }
        /// <summary>
        /// Returns the largest element in an array of values.
        /// </summary>
        public static TNum Max(IReadOnlyList<TNum> values)
        {
            if (values == null || values.Count <= 0)
                throw new ArgumentException("Values array must have at least one element.");
            if (values.Count == 1)
                return values[0];
            
            TNum value = values[0];
            for (int i = 1; i < values.Count; i++)
            {
                if (GreaterThan(values[i], value))
                {
                    value = values[i];
                }
            }
            return value;
        }

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
            if (Equal_Internal(minValue, maxValue))
                return minValue;
            if (GreaterThan(minValue, maxValue))
                SwapValues(ref minValue, ref maxValue);
#if MATH_IFACES
            return TNum.Clamp(value, minValue, maxValue);
#else
            if (LessThanOrEqual(value, minValue))
                return minValue;
            if (GreaterThanOrEqual(value, maxValue))
                return maxValue;
            return value;
#endif
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
#if MATH_IFACES
        where TNum    : struct, INumber<TNum>
        where TResult : struct, INumber<TResult>
#else
        where TNum    : struct
        where TResult : struct
#endif
    {
        public static TResult Convert(TNum value)
        {
#if MATH_IFACES
            return TResult.CreateTruncating<TNum>(value);
#else
            return GenericMath<TNum, TResult>.Convert(value);
#endif
        }
    }
}