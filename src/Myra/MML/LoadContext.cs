using Myra.Attributes;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.MML
{
	internal class LoadContext: BaseContext
	{
		public Dictionary<string, string> LegacyNames = new Dictionary<string, string>();
		public Dictionary<string, Color> Colors;
		public HashSet<string> NodesToIgnore = null;
		public Func<Type, object> ObjectCreator = (type) => Activator.CreateInstance(type);
		public string Namespace;
		public Assembly Assembly = typeof(Widget).Assembly;
		public Func<string, TextureRegion> TextureGetter = null;
		public Func<string, SpriteFont> FontGetter = null;

		public void Load(object obj, XElement el)
		{
			var type = obj.GetType();

			List<PropertyInfo> complexProperties, simpleProperties;
			ParseProperties(type, out complexProperties, out simpleProperties);

			foreach (var attr in el.Attributes())
			{
				var property = (from p in simpleProperties where p.Name == attr.Name select p).FirstOrDefault();

				if (property != null)
				{
					object value = null;

					var propertyType = property.PropertyType;
					if (propertyType.IsEnum)
					{
						value = Enum.Parse(propertyType, attr.Value);
					}
					else if (propertyType == typeof(Color) || propertyType == typeof(Color?))
					{
						Color color;
						if (Colors != null && Colors.TryGetValue(attr.Value, out color))
						{
							value = color;
						}
						else
						{
							value = attr.Value.FromName();
						}
					}
					else if (propertyType == typeof(IRenderable) && TextureGetter != null)
					{
						var texture = TextureGetter(attr.Value);
						if (texture == null)
						{
							throw new Exception(string.Format("Could not find renderable '{0}'", attr.Value));
						}
						value = texture;
					}
					else if (propertyType == typeof(SpriteFont) && FontGetter != null)
					{
						var font = FontGetter(attr.Value);
						if (font == null)
						{
							throw new Exception(string.Format("Could not find font '{0}'", attr.Value));
						}
						value = font;
					}
					else
					{
						if (propertyType.IsNullablePrimitive())
						{
							propertyType = propertyType.GetNullableType();
						}

						value = Convert.ChangeType(attr.Value, propertyType, CultureInfo.InvariantCulture);
					}
					property.SetValue(obj, value);
				}
			}

			var contentProperty = (from p in complexProperties
								   where p.FindAttribute<ContentAttribute>() 
								   != null select p).FirstOrDefault();

			foreach (var child in el.Elements())
			{
				var childName = child.Name.ToString();
				if (NodesToIgnore != null && NodesToIgnore.Contains(childName))
				{
					continue;
				}

				// Find property
				var property = (from p in complexProperties where p.Name == childName select p).FirstOrDefault();
				if (property != null)
				{
					do
					{
						var value = property.GetValue(obj);
						var asList = value as IList;
						if (asList != null)
						{
							// List
							foreach (var child2 in child.Elements())
							{
								var item = ObjectCreator(property.PropertyType.GenericTypeArguments[0]);
								Load(item, child2);
								asList.Add(item);
							}

							break;
						}

						var asDict = value as IDictionary;
						if (asDict != null)
						{
							// Dict
							foreach (var child2 in child.Elements())
							{
								var item = ObjectCreator(property.PropertyType.GenericTypeArguments[1]);
								Load(item, child2);

								var id = string.Empty;
								if (child2.Attribute(IdName) != null)
								{
									id = child2.Attribute(IdName).Value;
								}

								asDict[id] = item;
							}

							break;
						}

						if (property.SetMethod == null)
						{
							// Readonly
							Load(value, child);
						}
						else
						{
							var newValue = ObjectCreator(property.PropertyType);
							Load(newValue, child);
							property.SetValue(obj, newValue);
						}
						break;
					} while (true);
				}
				else
				{
					// Property not found
					// Should be widget class name then
					var widgetName = childName;
					string newName;
					if (LegacyNames.TryGetValue(widgetName, out newName))
					{
						widgetName = newName;
					}

					var itemType = Assembly.GetType(Namespace + "." + widgetName);
					if (itemType != null)
					{
						var item = ObjectCreator(itemType);
						Load(item, child);

						if (contentProperty == null)
						{
							throw new Exception(string.Format("Class {0} lacks property marked with ContentAttribute", type.Name));
						}

						var containerValue = contentProperty.GetValue(obj);
						var asList = containerValue as IList;
						if (asList != null)
						{
							// List
							asList.Add(item);
						} else
						{
							// Simple
							contentProperty.SetValue(obj, item);
						}
					}
					else
					{
						throw new Exception(string.Format("Could not resolve tag '{0}'", widgetName));
					}
				}
			}
		}
	}
}
