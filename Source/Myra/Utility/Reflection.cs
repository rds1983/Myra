using System;
using System.Reflection;
using System.Linq;

namespace Myra.Utility
{
	public static class Reflection
	{
		public static T FindAttribute<T>(this MemberInfo property) where T : Attribute
		{
			var result = (from T a in property.GetCustomAttributes<T>(true) select a).FirstOrDefault();

			return result;
		}

		public static T FindAttribute<T>(this Type type) where T : Attribute
		{
			var result = (from T a in type.GetTypeInfo().GetCustomAttributes<T>(true) select a).FirstOrDefault();

			return result;
		}

		public static bool IsNullablePrimitive(this Type type)
		{
			return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
					type.GenericTypeArguments[0].GetTypeInfo().IsPrimitive;
		}

		public static Type GetNullableType(this Type type)
		{
			return type.GenericTypeArguments[0];
		}
	}
}