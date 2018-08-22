using Myra.Attributes;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;

namespace Myra.Utility
{
	public static class Serialization
	{
		public static bool GetStyle(this JObject styles, string name, out string result)
		{
			result = null;

			JToken obj;
			if (!styles.TryGetValue(name, out obj))
			{
				return false;
			}

			result = obj.ToString();

			return true;
		}

		public static bool GetStyle(this JObject styles, string name, out JObject result)
		{
			result = null;

			JToken obj;
			if (!styles.TryGetValue(name, out obj))
			{
				return false;
			}

			result = obj.ToObject<JObject>();

			return true;
		}

		public static bool GetStyle(this JObject styles, string name, out int result)
		{
			result = 0;

			string s;
			if (!GetStyle(styles, name, out s))
			{
				return false;
			}

			int i;
			if (!int.TryParse(s, out i))
			{
				return false;
			}

			result = i;

			return true;
		}
		
		public static bool GetStyle(this JObject styles, string name, out bool result)
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
				
		public static bool GetStyle(this JObject styles, string name, out char result)
		{
			result = '\0';

			string s;
			if (!GetStyle(styles, name, out s))
			{
				return false;
			}

			char i;
			if (!char.TryParse(s, out i))
			{
				return false;
			}

			result = i;

			return true;
		}

		public static bool GetStyle(this JObject styles, string name, out float result)
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

		public static bool HasStylesheetValue(Stylesheet stylesheet, Widget w, PropertyInfo property, string styleName)
		{
			if (string.IsNullOrEmpty(styleName))
			{
				styleName = Stylesheet.DefaultStyleName;
			}

			// Find styles dict of that widget
			var stylesDictPropertyName = w.GetType().Name + "Styles";
			var stylesDictProperty = stylesheet.GetType().GetRuntimeProperty(stylesDictPropertyName);
			if (stylesDictProperty == null)
			{
				return false;
			}

			var stylesDict = (IDictionary)stylesDictProperty.GetValue(stylesheet);
			if (stylesDict == null)
			{
				return false;
			}

			// Fetch style from the dict
			var style = stylesDict[styleName];

			// Now find corresponding property
			PropertyInfo styleProperty = null;

			var stylePropertyPathAttribute = property.FindAttribute<StylePropertyPathAttribute>();
			if (stylePropertyPathAttribute != null)
			{
				var parts = stylePropertyPathAttribute.Name.Split('.');

				for (var i = 0; i < parts.Length; ++i)
				{
					styleProperty = style.GetType().GetRuntimeProperty(parts[i]);

					if (i < parts.Length - 1)
					{
						style = styleProperty.GetValue(style);
					}
				}
			}
			else
			{
				styleProperty = style.GetType().GetRuntimeProperty(property.Name);
			}

			if (styleProperty == null)
			{
				return false;
			}

			// Compare values
			var styleValue = styleProperty.GetValue(style);
			var value = property.GetValue(w);
			if (!Equals(styleValue, value))
			{
				return false;
			}

			return true;
		}
	}
}