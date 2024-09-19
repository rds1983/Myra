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
using FontStashSharp;
using FontStashSharp.RichText;
using info.lundin.math;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.MML
{
	internal class SaveContext: BaseContext
	{
		public Func<object, PropertyInfo, bool> ShouldSerializeProperty = HasDefaultValue;

		private static string SaveSimpleProperty(BaseObject baseObject, object value, Type propertyType, string propertyName)
		{
			string str = null;

			var serializer = FindSerializer(propertyType);
			if (serializer != null)
			{
				str = serializer.Serialize(value);
			}
			else if (propertyType == typeof(Color?))
			{
				str = ((Color?)value).Value.ToHexString();
			}
			else
			if (propertyType == typeof(Color))
			{
				str = ((Color)value).ToHexString();
			}
			else if (typeof(IBrush).IsAssignableFrom(propertyType) || propertyType == typeof(SpriteFontBase))
			{
				if (baseObject != null)
				{
					baseObject.Resources.TryGetValue(propertyName, out str);
				}
			}
			else
			{
				str = Convert.ToString(value, CultureInfo.InvariantCulture);
			}

			return str;
		}

		public XElement Save(object obj, bool skipComplex = false, string tagName = null, Type parentType = null)
		{
			var type = obj.GetType();

			var baseObject = obj as BaseObject;

			List<PropertyInfo> complexProperties, simpleProperties;
			ParseProperties(type, true, out complexProperties, out simpleProperties);

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
					string str = SaveSimpleProperty(baseObject, value, property.PropertyType, property.Name);
					if (!string.IsNullOrEmpty(str))
					{
						el.Add(new XAttribute(property.Name, str));
					}
				}
			}

			if (baseObject != null && parentType != null)
			{
				var attachedProperties = AttachedPropertiesRegistry.GetPropertiesOfType(parentType);
				foreach (var property in attachedProperties)
				{
					var value = property.GetValueObject(baseObject);
					if (value != null && !value.Equals(property.DefaultValueObject))
					{
						var propertyName = property.OwnerType.Name + "." + property.Name;
						var str = SaveSimpleProperty(baseObject, value,
							property.PropertyType, propertyName);
						if (!string.IsNullOrEmpty(str))
						{
							el.Add(new XAttribute(propertyName, str));
						}
					}
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
							collectionRoot.Add(Save(comp, parentType: obj.GetType()));
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
