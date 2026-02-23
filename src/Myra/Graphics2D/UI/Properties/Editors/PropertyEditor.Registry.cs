using System;
using System.Collections.Generic;
using System.Linq;
using Myra.Utility;

namespace Myra.Graphics2D.UI.Properties.Editors
{
    public partial class PropertyEditor
    {
        /// <summary>
        /// Relationship data for an editor and its supported types
        /// </summary>
        internal sealed class Registry
        {
            private readonly Type _editorType;
            private readonly Type[] _types;
            private readonly string[] _typeNames;
            private readonly Func<Type, bool> _extraTypeChecks;
            public Type EditorType => _editorType;

            /// <summary>
            /// Return true if the editor type is Generic and not yet complete.
            /// </summary>
            public bool IsOpenGenericType => _editorType.IsGenericTypeDefinition;

            public Registry(Type editorType, params Type[] propertyTypes)
            {
                _editorType = editorType;
                _types = propertyTypes;
                _typeNames = TypeToString(propertyTypes);

                if (propertyTypes.Length > 0 && propertyTypes[0] == typeof(Enum))
                {
                    _extraTypeChecks = _enumCheck;
                }
                else
                {
                    _extraTypeChecks = _falseCheck;
                }
            }

            /// <summary>
            /// Returns true if this editor type can support <paramref name="type"/>.
            /// </summary>
            /// <param name="allowCasts">Allow casting <paramref name="type"/> to an intermediate type? (Like interfaces)</param>
            public bool CanEditType(Type type, bool allowCasts = true)
            {
                if (!allowCasts)
                {
                    for (int i = 0; i < _types.Length; i++)
                    {
                        Type supported = _types[i];
                        if (object.ReferenceEquals(supported, type))
                            return true;
                    }

                    return CanEditType(TypeToString(type));
                }
                else
                {
                    for (int i = 0; i < _types.Length; i++)
                    {
                        Type supported = _types[i];
                        if (object.ReferenceEquals(supported, type))
                            return true;
                        if (supported.IsInterface && supported.IsAssignableFrom(type))
                            return true;
                        if (_extraTypeChecks.Invoke(type))
                            return true;
                    }

                    return CanEditType(TypeToString(type));
                }
            }

            private bool CanEditType(string value)
            {
                foreach (string supported in _typeNames)
                {
                    if (StringComparer.InvariantCultureIgnoreCase.Equals(supported, value))
                        return true;
                }

                return false;
            }

            //TODO use a less expensive conversion here.
            //We only use string for the purpose of testing type/type equality across assembly
            private static string TypeToString(Type value) => value.GetFriendlyName();

            private static string[] TypeToString(params Type[] args)
            {
                if (args == null || args.Length == 0)
                    return null;
                HashSet<string> result = new HashSet<string>();
                for (int i = 0; i < args.Length; i++)
                {
                    result.Add(TypeToString(args[i]));
                }

                return result.ToArray();
            }

            private static bool _falseCheck(Type t) => false;
            private static bool _enumCheck(Type t) => t.IsEnum;
        }
    }
}