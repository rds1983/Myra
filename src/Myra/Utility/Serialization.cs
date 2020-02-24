using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using XNAssets.Utility;
using Myra.Graphics2D;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Utility
{
	public static class Serialization
	{
		internal static bool GetStyle(this Dictionary<string, object> styles, string name, out string result)
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

		internal static bool GetStyle(this Dictionary<string, object> styles, string name, out Dictionary<string, object> result)
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
			if (property.PropertyType == typeof(Thickness) &&
				value.Equals(Thickness.Zero))
			{
				// Skip empty Thickness
				return true;
			}

			var defaultAttribute = property.FindAttribute<DefaultValueAttribute>();

			object defaultAttributeValue = null;
			if (defaultAttribute != null)
			{
				defaultAttributeValue = defaultAttribute.Value;
				// If property is of Color type, than DefaultValueAttribute should contain its name or hex
				if (property.PropertyType == typeof(Color))
				{
					defaultAttributeValue = ColorStorage.FromName(defaultAttributeValue.ToString()).Value;
				}

				if (property.PropertyType == typeof(string) && 
					string.IsNullOrEmpty((string)defaultAttributeValue) && 
					string.IsNullOrEmpty((string)value))
				{
					// Skip empty/null string
					return true;
				}

				if (Equals(value, defaultAttributeValue))
				{
					// Skip default
					return true;
				}
			}

			return false;
		}
	}
}