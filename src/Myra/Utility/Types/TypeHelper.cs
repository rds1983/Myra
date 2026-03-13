using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Myra.Utility.Types
{
    public static class TypeHelper
    {
        private static readonly ReadOnlyCollection<KeyValuePair<string, string>> _typeKeywordPairs =
            new ReadOnlyCollection<KeyValuePair<string, string>>(new List<KeyValuePair<string, string>>
            {   //Key is DotNet, Value is C# keyword
                new KeyValuePair<string, string>("Single",  "float"),
                new KeyValuePair<string, string>("Double",  "double"),
                new KeyValuePair<string, string>("Decimal", "decimal"),
                
                new KeyValuePair<string, string>("SByte",   "sbyte"),
                new KeyValuePair<string, string>("Byte",    "byte"),
                new KeyValuePair<string, string>("Int16",   "short"),
                new KeyValuePair<string, string>("UInt16",  "ushort"),
                new KeyValuePair<string, string>("Int32",   "int"),
                new KeyValuePair<string, string>("UInt32",  "uint"),
                new KeyValuePair<string, string>("Int64",   "long"),
                new KeyValuePair<string, string>("UInt64",  "ulong"),
                
                new KeyValuePair<string, string>("Boolean", "bool"),
                new KeyValuePair<string, string>("Char",    "char"),
                new KeyValuePair<string, string>("Object",  "object"),
                new KeyValuePair<string, string>("String",  "string"),
            });
        private static readonly ReadOnlyDictionary<Type, string> _typeSuffixLiterals = 
            new ReadOnlyDictionary<Type, string>(new Dictionary<Type, string>
            {
                {typeof(float),   "f"},
                {typeof(double),  "d"},
                {typeof(decimal), "m"},
                {typeof(byte),    string.Empty},
                {typeof(sbyte),   string.Empty},
                {typeof(short),   string.Empty},
                {typeof(ushort),  string.Empty},
                {typeof(int),     string.Empty},
                {typeof(uint),    "u"},
                {typeof(long),    "L"},
                {typeof(ulong),   "uL"},
            });
        private static Dictionary<Type, Func<TypeInfo>> _lookup;
        
        internal static void RegisterHelperForType<T>()
        {
            if(_lookup == null)
                _lookup = new Dictionary<Type, Func<TypeInfo>>(16);

            Type helpingType = typeof(T);
            _lookup.Add(helpingType, () => TypeHelper<T>.Info);
        }
        public static bool TryGetInfoForType(Type type, out TypeInfo result)
        {
            if (_lookup != null)
            {
                if (_lookup.TryGetValue(type, out var getterFunc))
                {
                    result = getterFunc.Invoke();
                    return (int)result.Code > 0;
                }
            }
            result = default;
            return false;
        }
        
        /// <summary>
        /// If type is <see cref="System.Nullable{}"/>, return the generic type the nullable holds (T), else return type unchanged. Also returns boolean for if type was changed
        /// </summary>
        public static bool GetNullableTypeOrPassThrough(ref Type type)
        {
            bool changed = false;
            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(Nullable<>))
                {
                    type = type.GenericTypeArguments[0];
                    changed = true;
                }
            }
            return changed;
        }
        
#region Strings
        private const string FrontendGeneric = "<>";
        private const string BackendGeneric = "`1";
        public static bool IsGenericTypeName(string str) => IsGenericTypeName_FrontEnd(str) || IsGenericTypeName_BackEnd(str);
        /// <summary>
        /// Returns if the string ends with a front-end C# generic pattern
        /// </summary>
        public static bool IsGenericTypeName_FrontEnd(string str)
        {
            return str.Contains(FrontendGeneric);
        }
        /// <summary>
        /// Returns if the string ends with a back-end C# generic pattern
        /// </summary>
        public static bool IsGenericTypeName_BackEnd(string str)
        {
            return str.Contains(BackendGeneric);
        }
        public static void SwapGenericTypeNameFormat(ref string str)
        {
            if (IsGenericTypeName_FrontEnd(str))
            {
                str = str.Replace(FrontendGeneric, BackendGeneric);
            }
            else if (IsGenericTypeName_BackEnd(str))
            {
                str = str.Replace(BackendGeneric, FrontendGeneric);
            }
        }
        public static bool MakeFancyGenericTypeName(Type forType, out string result, bool avoidInternalTypeNames = true)
        {
            if (forType.IsGenericType)
            {
                string genericArgName;
                try
                {
                    genericArgName = forType.GetGenericArguments()[0].Name;
                }
                catch
                {
                    genericArgName = "?";
                }
                
                // Assume that starting uppercase is an internal type name
                if (avoidInternalTypeNames && char.IsUpper(genericArgName[0])) 
                {
                    NameSwap_DotNetToKeyword(ref genericArgName);
                }
                
                result = forType.Name;
                MakeFancyGenericTypeName(ref result, genericArgName);
                return true;
            }
            result = null;
            return false;
        }
        public static void MakeFancyGenericTypeName(ref string str, string genericTypeName)
        {
            if(!IsGenericTypeName_BackEnd(str))
                return;
            if (string.IsNullOrWhiteSpace(genericTypeName))
                genericTypeName = string.Empty;
            else if (genericTypeName.Contains(".")) //remove namespace
                genericTypeName = genericTypeName.Split('.').Last();
            
            str = str.Replace(BackendGeneric, $"<{genericTypeName}>");
        }
        
        public static void StripGenericFromString(ref string str, string replace = null)
        {
            if (string.IsNullOrEmpty(replace))
            {
                replace = string.Empty;
            }

            if (IsGenericTypeName_BackEnd(str))
            {
                str = str.Replace(BackendGeneric, replace);
            }
            else if (IsGenericTypeName_FrontEnd(str))
            {
                str = str.Replace(FrontendGeneric, replace);
            }
        }

        public static void NameSwap_DotNetToKeyword(ref string str)
        {
            foreach (var pair in _typeKeywordPairs)
            {
                if (pair.Key == str)
                {
                    str = pair.Value;
                    return;
                }
            }
        }
        public static void NameSwap_KeywordToDotNet(ref string str)
        {
            foreach (var pair in _typeKeywordPairs)
            {
                if (pair.Value == str)
                {
                    str = pair.Key;
                    return;
                }
            }
        }
        public static bool TryGetValueSuffixLiteral(Type forType, out string result)
        {
            if (_typeSuffixLiterals.TryGetValue(forType, out string str))
            {
                result = str;
                return true;
            }
            result = string.Empty;
            return false;
        }
#endregion Strings
    }
    
    /// <summary>
    /// Provides static helpers and cached info about a generic type <typeparamref name="T"/>.
    /// Accessing a type via this class will load a <see cref="TypeInfo"/> associated with that type.
    /// </summary>
    internal static class TypeHelper<T>
    {
        static TypeHelper()
        {
            _type = typeof(T);
            _info = new TypeInfo(_type);
            TypeHelper.RegisterHelperForType<T>();

            FindMethodTryParse();
        }

        private static readonly Type _type;
        private static readonly TypeInfo _info;
        
        /// <summary>
        /// Cached information about generic type <typeparamref name="T"/>.
        /// </summary>
        public static TypeInfo Info => _info;

        /// <summary>
        /// If <typeparamref name="T"/> is <see cref="System.Nullable{}"/>, return the generic type the nullable holds, else return type <typeparamref name="T"/>.
        /// </summary>
        public static Type GetNullableTypeOrPassThrough()
        {
            if (!_info.IsNullable)
                return _type;
            return _type.GenericTypeArguments[0];
        }
        
        public static bool CanAssign(Type other) => _type.IsAssignableFrom(other);

        private static MethodInfo _tryParse;
        private static void FindMethodTryParse()
        {
            const string METHOD_NAME = "TryParse";
            Type type = typeof(T);
            try
            {
                // public static bool TryParse(string str, out TData value)
                _tryParse = type.GetMethod(METHOD_NAME,
                    BindingFlags.Static | BindingFlags.Public, null,
                    new[] { typeof(string), type.MakeByRefType() }, null);
                
                if (_tryParse == null)
                {
                    throw new Exception($"Reflection Error: '{type.Name}' does not contain a public static method '{METHOD_NAME}'");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Unhandled Reflection Error: {e}");
            }
        }
        
        /// <summary>
        /// Attempts find and invoke <typeparamref name="T"/>'s static TryParse() method using Reflection.
        /// </summary>
        public static bool TryParse(string str, out T data)
        {
            if (_tryParse == null)
                throw new NotSupportedException($"Not supported: {typeof(T)}.TryParse()");
            
            // public static bool TryParse(string str, out TData value)
            // The method's "out TData" gets written to object[] array.
            object[] param = new object[] { str, default(T) }; 
            object result = _tryParse.Invoke(null, param); 
                
            if (result is bool didConvert)
            {
                // Read what the method wrote as "out T"
                data = didConvert ? (T)param[1] : default;
                return didConvert;
            }
            
            // We really might want to throw an exception here instead
            data = default;
            return false;
        }
    }
}