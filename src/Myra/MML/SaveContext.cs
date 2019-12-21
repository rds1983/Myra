using Myra.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using Myra.Attributes;
using System.Linq;
using Myra.Graphics2D;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.MML
{
	internal class SaveContext: BaseContext
	{
		public Func<object, PropertyInfo, bool> ShouldSerializeProperty = HasDefaultValue;

		public XElement Save(object obj, bool skipComplex = false, string tagName = null)
		{
			var type = obj.GetType();

			var hasResources = obj as IHasResources;

			List<PropertyInfo> complexProperties, simpleProperties;
			ParseProperties(type, out complexProperties, out simpleProperties);

			var el = new XElement(tagName ?? type.Name);

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
					else if (property.PropertyType == typeof(IBrush) || property.PropertyType == typeof(SpriteFont))
					{
						str = string.Empty;
						if (hasResources != null)
						{
							hasResources.Resources.TryGetValue(property.Name, out str);
						}
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
				var contentProperty = (from p in complexProperties
									   where p.FindAttribute<ContentAttribute>()
									   != null
									   select p).FirstOrDefault();

				foreach (var property in complexProperties)
				{
					if (!ShouldSerializeProperty(obj, property))
					{
						continue;
					}

					var value = property.GetValue(obj);
					if (value == null)
					{
						continue;
					}

					var propertyName = type.Name + "." + property.Name;
					var isContent = property == contentProperty;
					var asList = value as IList;
					if (asList == null)
					{
						el.Add(isContent?Save(value):Save(value, false, propertyName));
					}
					else
					{
						var collectionRoot = el;

						if (property.FindAttribute<ContentAttribute>() == null && asList.Count > 0)
						{
							collectionRoot = new XElement(propertyName);
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
