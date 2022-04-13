using Myra.Graphics2D;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using FontStashSharp;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.MML
{
	internal class BaseContext
	{
		public static readonly Dictionary<Type, ITypeSerializer> _serializers = new Dictionary<Type, ITypeSerializer>
		{
			{typeof(Vector2), new Vector2Serializer()},
			{typeof(Thickness), new ThicknessSerializer()},
		};

		public const string IdName = "Id";

		public static ITypeSerializer FindSerializer(Type type)
		{
			if (type.IsNullablePrimitive())
			{
				type = type.GetNullableType();
			}

			ITypeSerializer result;
			if (_serializers.TryGetValue(type, out result))
			{
				return result;
			}

			return null;
		}

		protected static void ParseProperties(Type type, 
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

				var attr = property.FindAttribute<XmlIgnoreAttribute>();
				if (attr != null)
				{
					continue;
				}

				var propertyType = property.PropertyType;
				if (propertyType.IsPrimitive || 
					propertyType.IsNullablePrimitive() ||
					propertyType.IsEnum || 
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
