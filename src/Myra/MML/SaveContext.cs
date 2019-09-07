using Myra.Attributes;
using Myra.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.MML
{
	internal class SaveContext: BaseContext
	{
		public Func<object, PropertyInfo, bool> ShouldSerializeProperty = HasDefaultValue;

		public XElement Save(object obj, bool skipComplex = false)
		{
			var type = obj.GetType();

			List<PropertyInfo> complexProperties, simpleProperties;
			ParseProperties(type, out complexProperties, out simpleProperties);

			var el = new XElement(type.Name);

			foreach (var property in simpleProperties)
			{
				if (!ShouldSerializeProperty(obj, property))
				{
					continue;
				}

				// Obsolete properties ignored only on save(and not ignored on load)
				var attr = property.FindAttribute<ObsoleteAttribute>();
				if (attr != null)
				{
					continue;
				}

				var value = property.GetValue(obj);
				if (value != null)
				{
					string str;

					if (property.PropertyType == typeof(Color?))
					{
						str = ((Color?)value).Value.ToHexString();
					}
					else
					if (property.PropertyType == typeof(Color))
					{
						str = ((Color)value).ToHexString();
					}
					else
					{
						str = Convert.ToString(value, CultureInfo.InvariantCulture);
					}

					el.Add(new XAttribute(property.Name, str));
				}
			}

			if (!skipComplex)
			{
				foreach (var property in complexProperties)
				{
					var value = property.GetValue(obj);

					if (value == null)
					{
						continue;
					}

					var asList = value as IList;
					if (asList == null)
					{
						el.Add(Save(value));
					}
					else
					{
						var collectionRoot = el;

						if (property.FindAttribute<ContentAttribute>() == null)
						{
							collectionRoot = new XElement(property.Name);
							el.Add(collectionRoot);
						}

						foreach (var comp in asList)
						{
							collectionRoot.Add(Save(comp));
						}
					}
				}
			}

			return el;
		}

		public static bool HasDefaultValue(object w, PropertyInfo property)
		{
			var value = property.GetValue(w);
			if (property.HasDefaultValue(value))
			{
				return true;
			}

			return false;
		}
	}
}
