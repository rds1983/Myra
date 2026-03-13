using System;

namespace Myra.Utility.Types
{
    public readonly struct TypeInfo
    {
        public readonly TypeCode Code;
        
        /// <inheritdoc cref="Type.IsClass"/>
        public readonly bool IsClass;
        /// <inheritdoc cref="Type.IsInterface"/>
        public readonly bool IsInterface;
        /// <inheritdoc cref="Type.IsValueType"/>
        public readonly bool IsValue;
        /// <summary>
        /// If true, the type is a basic .Net primitive type.
        /// </summary>
        public readonly bool IsPrimitive;
        /// <summary>
        /// True if: <para/>
        /// The type <see cref="Type.IsValueType"/> contained inside a <see cref="System.Nullable{}"/>.<para/>
        /// The type is any other complex class or object.
        /// </summary>
        public readonly bool IsNullable;
        
        /// <summary>
        /// If true, the type represents a numerical .Net value. This value is based off <see cref="TypeCode"/>.
        /// </summary>
        public readonly bool IsNumber;
        /// <summary>
        /// If true, the type represents a numerical .Net value that can be negative. This value is based off <see cref="TypeCode"/>.
        /// </summary>
        public readonly bool IsSignedNumber;
        /// <summary>
        /// If true, the type represents an integer-based numerical .Net value. This value is based off <see cref="TypeCode"/>.
        /// </summary>
        public readonly bool IsWholeNumber;
        /// <summary>
        /// If true, the type represents a floating-point numerical .Net value. This value is based off <see cref="TypeCode"/>.
        /// </summary>
        public readonly bool IsFractionalNumber;
        
        internal TypeInfo(Type type)
        {
            Code = Type.GetTypeCode(type);

            IsNumber = false;
            IsSignedNumber = false;
            IsWholeNumber = false;
            IsFractionalNumber = false;
            
            IsPrimitive = type.IsPrimitive;
            IsValue = type.IsValueType;
            IsInterface = type.IsInterface;
            IsClass = type.IsClass;
            
            if (IsClass)
            {
                // The type is a class or delegate 
                IsNullable = true;
            }
            else
            {
                // The type is a value or interface
                if (IsInterface)
                {
                    IsNullable = true;
                }
                else
                {
                    IsNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
                    
                    IsNumber = Code.IsNumericType();
                    IsSignedNumber = Code.IsSigned();
                    IsWholeNumber = Code.IsNumericInteger();
                    IsFractionalNumber = Code.IsNumericFractional();
                }
            }
        }
    }
}