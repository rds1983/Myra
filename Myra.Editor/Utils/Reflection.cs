using System;
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
	}
}