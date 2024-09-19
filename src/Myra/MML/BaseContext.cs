using Myra.Graphics2D;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using FontStashSharp;
using Myra.Utility;
using Myra.Attributes;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Numerics;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.MML
{
	internal class BaseContext
	{
		public const string IdName = "Id";

		public static ITypeSerializer FindSerializer(Type type)
		{
			if (type.IsNullablePrimitive())
			{
				type = type.GetNullableType();
			}

			ITypeSerializer result;
			if (Serialization._serializers.TryGetValue(type, out result))
			{
				return result;
			}

			return null;
		}

		protected static void ParseProperties(Type type, bool checkSkipSave,
			out List<PropertyInfo> complexProperties, 
			out List<PropertyInfo> simpleProperties)
		{
			complexProperties = new List<PropertyInfo>();
			simpleProperties = new List<PropertyInfo>();

			var allProperties = type.GetRuntimeProperties();
			foreach (var property in allProperties)
			{
				if (property.GetMethod == null ||
					!property.GetMethod.IsPublic ||
					property.GetMethod.IsStatic)
				{
					continue;
				}

				Attribute attr = property.FindAttribute<XmlIgnoreAttribute>();
				if (attr != null)
				{
					continue;
				}

				if (checkSkipSave)
				{
					attr = property.FindAttribute<SkipSaveAttribute>();
					if (attr != null)
					{
						continue;
					}
				}

				var propertyType = property.PropertyType;
				if (propertyType.IsPrimitive || 
					propertyType.IsNullablePrimitive() ||
					propertyType.IsEnum || 
					propertyType.IsNullableEnum() ||
					propertyType == typeof(string) ||
					propertyType == typeof(Vector2) ||
					propertyType == typeof(Color) ||
					propertyType == typeof(Color?) ||
					typeof(IBrush).IsAssignableFrom(propertyType) ||
					propertyType == typeof(SpriteFontBase) ||
					propertyType == typeof(Thickness))
				{
					simpleProperties.Add(property);
				} else
				{
					complexProperties.Add(property);
				}
			}
		}
	}
}
