using System;

namespace Myra.Utility
{
	internal static class Reflection
	{
		public static bool IsNullableEnum(this Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
					type.GenericTypeArguments[0].IsEnum;
		}
	}
}
