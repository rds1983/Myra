using Myra.Graphics2D;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using FontStashSharp;
using Myra.Utility;
using Myra.Attributes;
using Myra.Graphics2D.TextureAtlases;

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

		public static bool IsTypeExternalAsset(Type type)
		{
			return typeof(IBrush).IsAssignableFrom(type) ||
					typeof(SpriteFontBase).IsAssignableFrom(type) ||
					type == typeof(TextureRegionAtlas);
		}

		public static ITypeSerializer FindSerializer(Type type)
		{
			if (type.IsNullable())
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

		protected static void ParseProperties(Type type, bool isSave,
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

				if (property.HasAttribute<XmlIgnoreAttribute>() ||
					(isSave && (property.HasAttribute<SkipSaveAttribute>() || property.HasAttribute<ObsoleteAttribute>())) ||
					(!isSave && property.HasAttribute<SkipLoadAttribute>()))
				{
					continue;
				}

				var propertyType = property.PropertyType;
				if (propertyType.IsPrimitive ||
					propertyType.IsNullablePrimitive() ||
					propertyType.IsEnum ||
					propertyType.IsNullableEnum() ||
					propertyType == typeof(string) ||
					FindSerializer(propertyType) != null ||
					IsTypeExternalAsset(propertyType))
				{
					simpleProperties.Add(property);
				}
				else
				{
					complexProperties.Add(property);
				}
			}
		}
	}
}
