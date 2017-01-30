using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Myra.Editor.Utils
{
	public static class Reflection
	{
		public static T FindAttribute<T>(this PropertyInfo property) where T : Attribute
		{
			var result = (from T a in Attribute.GetCustomAttributes(property, typeof(T), true) select a).FirstOrDefault();

			return result;
		}

		public static T[] FindAttributes<T>(this PropertyInfo property) where T : Attribute
		{
			var result = (from T a in Attribute.GetCustomAttributes(property, typeof(T), true) select a).ToArray();

			return result;
		}

		public static T FindAttribute<T>(this Type type) where T : Attribute
		{
			var result = (from T a in Attribute.GetCustomAttributes(type, typeof(T), true) select a).FirstOrDefault();

			return result;
		}

		public static Type FindGenericType(this Type givenType, Type genericType)
		{
			var interfaceTypes = givenType.GetInterfaces();

			foreach (var it in interfaceTypes)
			{
				if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
					return it;
			}

			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
				return givenType;

			Type baseType = givenType.BaseType;
			if (baseType == null) return null;

			return FindGenericType(baseType, genericType);
		}
	}
}