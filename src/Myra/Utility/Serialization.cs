using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Utility
{
	public static class Serialization
	{
		public static bool GetStyle(this Dictionary<string, object> styles, string name, out string result)
		{
			result = null;

			object obj;
			if (!styles.TryGetValue(name, out obj))
			{
				return false;
			}

			result = obj.ToString();

			return true;
		}

		public static bool GetStyle(this Dictionary<string, object> styles, string name, out Dictionary<string, object> result)
		{
			result = null;

			object obj;
			if (!styles.TryGetValue(name, out obj))
			{
				return false;
			}

			result = (Dictionary<string, object>)obj;

			return true;
		}

		public static bool GetStyle(this Dictionary<string, object> styles, string name, out int result)
		{
			result = 0;

			string s;
			if (!GetStyle(styles, name, out s))
			{
				return false;
			}

			double i;
			if (!double.TryParse(s, out i))
			{
				return false;
			}

			result = (int)i;

			return true;
		}

		public static bool GetStyle(this Dictionary<string, object> styles, string name, out bool result)
		{
			result = false;

			string s;
			if (!GetStyle(styles, name, out s))
			{
				return false;
			}

			bool i;
			if (!bool.TryParse(s, out i))
			{
				return false;
			}

			result = i;

			return true;
		}

		public static bool GetStyle(this Dictionary<string, object> styles, string name, out float result)
		{
			result = 0.0f;

			string s;
			if (!GetStyle(styles, name, out s))
			{
				return false;
			}

			float f;
			if (!float.TryParse(s, out f))
			{
				return false;
			}

			result = f;

			return true;
		}

		public static bool HasDefaultValue(this PropertyInfo property, object value)
		{
			var defaultAttribute = property.FindAttribute<DefaultValueAttribute>();

			object defaultAttributeValue = null;
			if (defaultAttribute != null)
			{
				defaultAttributeValue = defaultAttribute.Value;
				// If property is of Color type, than DefaultValueAttribute should contain its name or hex
				if (property.PropertyType == typeof(Color))
				{
					defaultAttributeValue = defaultAttributeValue.ToString().FromName().Value;
				}
			}

			if ((defaultAttribute != null && Equals(value, defaultAttributeValue)))
			{
				// Skip default
				return true;
			}

			return false;
		}
	}
}