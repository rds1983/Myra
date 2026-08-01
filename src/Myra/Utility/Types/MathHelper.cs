using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BigInt = System.Numerics.BigInteger;

#if MATH_IFACES
using System.Numerics;
#else
using Generic.Math;
#endif

namespace Myra.Utility.Types
{
    /// <summary>
    /// Generic math methods and constants for <typeparamref name="TNum"/>.<para/>
    /// If project is less than .net7, uses Generic.Math library which relies on Reflection.Emit and codegen.
    /// </summary>
    public static class MathHelper<TNum>
#if MATH_IFACES
        where TNum : struct, INumber<TNum>, IMinMaxValue<TNum>
#else
        where TNum : struct
#endif
    {
        public static readonly TypeInfo Info;
        
        /// <summary>Value that represents 0 for <typeparamref name="TNum"/></summary>
        public static readonly TNum Zero;
        /// <summary>Value that represents 1 for <typeparamref name="TNum"/></summary>
        public static readonly TNum One;
        /// <summary>
        /// Value that represents the smallest value for TNum. This excludes <see cref="NegInfinity"/>
        /// </summary>
        public static readonly TNum Minimum;
        /// <summary>
        /// Value that represents the largest value for TNum. This excludes <see cref="Infinity"/>
        /// </summary>
        public static readonly TNum Maximum;
        /// <summary>
        /// Value that represents negative infinity. Undefined for integer types.
        /// </summary>
        public static readonly TNum? NegInfinity;
        /// <summary>
        /// Value that represents infinity. Undefined for integer types.
        /// </summary>
        public static readonly TNum? Infinity;
        
        static MathHelper()
        {
            Type arg = typeof(TNum);
            Info = TypeHelper<TNum>.Info;
            
            if(Info.IsNullable)
                throw new ArgumentException($"Invalid Generic-Type Argument: '{arg}', Nullable types are not supported");
            if(!Info.IsNumber)
                throw new ArgumentException($"Invalid Generic-Type Argument: '{arg}', Only numeric types are supported");
//            if(arg == typeof(byte) || arg == typeof(sbyte))
//                throw new ArgumentException($"Invalid Generic-Type Argument: '{arg}' does not have full math support. Convert to another type first");
            
#if MATH_IFACES
            Zero = TNum.Zero;
            One = TNum.One;
            Minimum = TNum.MinValue;
            Maximum = TNum.MaxValue;
#else
            Zero = GenericMath<TNum>.Zero;
            One = MathHelper<int>.ConvertTo<TNum>( 1 );
            
            if (Info.IsWholeNumber)
            {
                SquashFoundLimitsIntoIntType(out TNum min, out TNum max);
                Minimum = min;
                Maximum = max;
            }
#endif
            if (Info.IsWholeNumber)
            {
                Infinity = null;
                NegInfinity = null;
            }
            else if(Info.IsFractionalNumber)
            {
                //TODO find minimum and maximums
            }

#if !MATH_IFACES
            void SquashFoundLimitsIntoIntType(out TNum min, out TNum max)
            {
                ulong permutations;
                int steps;
                if (!FindMaxBitValue(out permutations, out steps))
                {
                    min = Zero;
                    max = One;
                }
                
                if (Info.IsSignedNumber)
                {
                    // Signed integer value type
                    long half;
                    if (steps == 4) // 64 bits?
                    {
                        half = (long)((permutations / 2uL) + 1);
                    }
                    else
                    {
                        half = (long)(permutations / 2uL);
                    }
                    min = MathHelper<long>.ConvertTo<TNum>(-half);
                    max = MathHelper<long>.ConvertTo<TNum>(half - 1);
                }
                else 
                {
                    // Unsigned integer value type
                    min = Zero;
                    if (steps == 4) // 64 bits?
                    {
                        max = MathHelper<ulong>.ConvertTo<TNum>(permutations);
                    }
                    else
                    {
                        max = MathHelper<ulong>.ConvertTo<TNum>(permutations - 1);
                    }
                }
            }
            
            bool FindMaxBitValue(out ulong permutations, out int steps)
            {
                permutations = 0uL;
                int? size = Info.Code.GetTypeSize();
                if (!size.HasValue)
                {
                    steps = -1;
                    return false;
                }
                
                bool found = false;
                int bitsInType = size.Value * 8;
                int n = 8;
                steps = 1;
                
                do
                {
                    if (n == bitsInType)
                    {
                        permutations = MathHelper<ulong>.Pow(2, n);
                        found = true;
                        break;
                    }

                    steps++;
                    n *= 2;
                    
                } while (n < 128); //Only check up to 128 bits

                if (steps >= 4)
                    permutations = ulong.MaxValue;
                return found;
            }
#endif
        }
/*
        private static void TestPow(int value, int exp, float expected)
        {
            TNum num = MathHelper<int, TNum>.Convert(value);
            TestPow(num, exp, out TNum result);
            float compare = MathHelper<TNum, float>.Convert(result);
            if(compare != expected)
                Console.WriteLine($"POW FAILURE! Got {compare}, Expected {expected}");
        }
        private static void TestPow(TNum value, int exp, out TNum result)
        {
            try
            {
                result = Pow(value, exp);
                Console.WriteLine($"MathHelper<{typeof(TNum).Name}>.Pow( {value}, {exp} ) = {result}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = Zero;
            }
        }*/
        
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
        /// <summary>
        /// Returns <paramref name="value"/> raised to the <paramref name="exponent"/>-th power.
        /// </summary>
        /// <exception cref="ArithmeticException">
        /// Thrown if the result is too small to fit into the integer-based value type <typeparamref name="TNum"/>.
        /// Without this exception, this method would return zero for all negative <paramref name="exponent"/> inputs.
        /// </exception>
        /// <exception cref="DivideByZeroException">
        /// Thrown if <paramref name="value"/> is zero and <paramref name="exponent"/> is negative.
        /// </exception>
        public static TNum Pow(TNum value, int exponent)
        {
            if (Info.IsWholeNumber & exponent < 0)
                throw new ArithmeticException($"Pow result will always be less than one and cannot be properly respresented as {typeof(TNum)}");
            
            if (Equal(value, Zero))
            {
                if (exponent < 0)
                    throw new DivideByZeroException(nameof(exponent));
                return Zero;
            }
            
            if (exponent == 0)
                return One;
            
            TNum result = One;
            if (exponent > 0)
            {
                // Do multiply op
                do
                {
                    result = Multiply_Internal(result, value);
                    exponent--;
                }
                while (exponent > 0);
            }
            else
            {
                // Do divide op
                exponent = -exponent;
                do
                {
                    result = Divide_Internal(result, value);
                    exponent--;
                }
                while (exponent > 0);
            }
            return result;
        }
        /// <summary>
        /// Returns <paramref name="value"/> raised to the second power.
        /// </summary>
        public static TNum Pow2(TNum value) => Pow(value, 2);
        
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
            if (Info.IsSignedNumber && LessThan(value, Zero))
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
        
        /// <summary>
        /// Convert number type '<typeparamref name="TNum"/>' to another number type '<typeparamref name="TResult"/>'.
        /// </summary>
        /// <typeparam name="TNum">The type to convert from.</typeparam>
        /// <typeparam name="TResult">The type to convert to.</typeparam>
        public static TResult ConvertTo<TResult>(TNum value)
#if MATH_IFACES
            where TResult : struct, INumber<TResult>
        {
            return TResult.CreateTruncating<TNum>(value);
        }
#else
            where TResult : struct
        {
            return GenericMath<TNum, TResult>.Convert(value);
        }
#endif
        
        private static void SwapValues(ref TNum a, ref TNum b)
        {
            TNum c = a;
            TNum d = b;
            b = c;
            a = d;
        }
    }
}