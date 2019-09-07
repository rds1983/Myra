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