using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Myra.Attributes;
using FontStashSharp;
using Myra.Utility;
using FontStashSharp.RichText;
using Myra.Utility.Types;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.MML
{
	internal class LoadContext : BaseContext
	{
		struct SimplePropertyInfo
		{
			public PropertyInfo Property;
			public BaseAttachedPropertyInfo AttachedProperty;
			public string Name;
			public Type PropertyType;

			public SimplePropertyInfo(PropertyInfo property)
			{
				Property = property;
				AttachedProperty = null;

				Name = property.Name;
				PropertyType = property.PropertyType;
			}

			public SimplePropertyInfo(BaseAttachedPropertyInfo property)
			{
				Property = null;
				AttachedProperty = property;

				Name = property.Name;
				PropertyType = property.PropertyType;
			}

			public void SetValue(object obj, object value)
			{
				if (Property != null)
				{
					Property.SetValue(obj, value);
				} else if (AttachedProperty != null && obj is BaseObject)
				{
					AttachedProperty.SetValueObject((BaseObject)obj, value);
				}
			}
		}

		public Dictionary<string, string> LegacyClassNames = null;
		public Dictionary<string, string> LegacyPropertyNames = null;
		public Dictionary<string, Color> Colors;
		public HashSet<string> NodesToIgnore = null;
		public Func<Type, XElement, object> ObjectCreator = (type, el) => Activator.CreateInstance(type);
		public Dictionary<Assembly, string[]> Assemblies;
		public Func<Type, string, object> ResourceGetter = null;
		public readonly List<Tuple<object, XElement>> ObjectsNodes = new List<Tuple<object, XElement>>();

		private const string UserDataAttributePrefix = "_";
		private static readonly char[] LegacySplitChar = new char[] { Project.LegacySeparator };

		public void Load<T>(object obj, XElement el, T handler) where T : class
		{
			ObjectsNodes.Add(new Tuple<object, XElement>(obj, el));

			var type = obj.GetType();
			var handlerType = typeof(T);

			var baseObject = obj as BaseObject;

			List<PropertyInfo> complexProperties, simpleProperties;
			ParseProperties(type, false, out complexProperties, out simpleProperties);
			
			string newName;
			foreach (XAttribute attr in el.Attributes())
			{
				var propertyName = attr.Name.ToString();
				if (LegacyPropertyNames != null && LegacyPropertyNames.TryGetValue(propertyName, out newName))
				{
					propertyName = newName;
				}

				SimplePropertyInfo? simplePropertyInfo = null;
				if (propertyName.Contains("."))
				{
					// Attached property
					var parts = propertyName.Split('.');
					if (parts.Length != 2)
					{
						throw new Exception($"Couldn't parse attached property {propertyName}");
					}
					var parentType = Project.GetWidgetTypeByName(parts[0].Trim());
					if (parentType == null)
					{
						throw new Exception($"Couldn't find type {parts[0].Trim()} for attached property {propertyName}");
					}

					var properties = AttachedPropertiesRegistry.GetPropertiesOfType(parentType);
					var property = (from p in properties where p.Name == parts[1].Trim() select p).FirstOrDefault();
					if (property == null)
					{
						throw new Exception($"Type {parentType.Name} doesn't have attached property {parts[1].Trim()}");
					}

					simplePropertyInfo = new SimplePropertyInfo(property);
				} else
				{
					var property = (from p in simpleProperties where p.Name == propertyName select p).FirstOrDefault();
					if (property != null)
					{
						simplePropertyInfo = new SimplePropertyInfo(property);
					}
				}

				if (simplePropertyInfo != null)
				{
					object value = null;

					var propertyType = simplePropertyInfo.Value.PropertyType;
					if (Serialization.TryFindSerializer(propertyType, out var serializer))
					{
						value = serializer.Deserialize(attr.Value);
					}
					else if (propertyType.IsEnum || propertyType.IsNullableEnum())
					{
						if (propertyType.IsNullableEnum())
						{
							propertyType = propertyType.GetNullableType();
						}
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
							value = ColorStorage.FromName(attr.Value);
							if (value == null)
							{
								throw new Exception(string.Format("Could not find parse color '{0}'", attr.Value));
							}
						}
					}
					else if ((typeof(IBrush).IsAssignableFrom(propertyType) ||
							 propertyType == typeof(SpriteFontBase)) &&
							 !string.IsNullOrEmpty(attr.Value) &&
							 ResourceGetter != null)
					{
						try
						{
							var texture = ResourceGetter(propertyType, attr.Value);
							if (texture == null)
							{
								throw new Exception(string.Format("Could not find resource '{0}'", attr.Value));
							}
							value = texture;

							if (baseObject != null)
							{
								baseObject.Resources[simplePropertyInfo.Value.Name] = attr.Value;
							}
						}
						catch (Exception)
						{
						}
					}
					else
					{
						TypeHelper.GetNullableTypeOrPassThrough(ref propertyType);
						value = Convert.ChangeType(attr.Value, propertyType, CultureInfo.InvariantCulture);	
					}

					simplePropertyInfo.Value.SetValue(obj, value);
				}
				else if (handler != null && type.GetEvent(attr.Name.LocalName) != null)
				{
					var method = handlerType.GetMethod(attr.Value, BindingFlags.Public | BindingFlags.Instance);
					var eventHandler = type.GetEvent(attr.Name.LocalName);
					if (method == null)
					{
						throw new InvalidOperationException($"Handler of type '{handlerType}' does not contain method '{attr.Value}'. If it does, ensure the method is both public and non-static.");
					}

					var delegateMethod = method.CreateDelegate(eventHandler.EventHandlerType, handler);
					eventHandler.AddEventHandler(obj, delegateMethod);
				}
				else
				{
					// Stow away custom user attributes
					if (propertyName.StartsWith(UserDataAttributePrefix) && baseObject != null)
					{
						baseObject.UserData.Add(propertyName, attr.Value);
					}
				}
			}
			
			var contentProperty = (from p in complexProperties
								   where p.FindAttribute<ContentAttribute>() != null 
								   select p).FirstOrDefault();

			foreach (XElement child in el.Elements())
			{
				string childName = child.Name.ToString();
				if (NodesToIgnore != null && NodesToIgnore.Contains(childName))
				{
					continue;
				}

				var isProperty = false;
				if (childName.Contains("."))
				{
					// Property name
					var parts = childName.Split('.');
					childName = parts[1];
					isProperty = true;
				}

				if (LegacyPropertyNames != null && LegacyPropertyNames.TryGetValue(childName, out newName))
				{
					childName = newName;
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
								var item = ObjectCreator(property.PropertyType.GenericTypeArguments[0], child2);
								Load(item, child2, handler);
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
								var item = ObjectCreator(property.PropertyType.GenericTypeArguments[1], child2);
								Load(item, child2, handler);

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
							Load(value, child, handler);
						}
						else
						{
							var newValue = ObjectCreator(property.PropertyType, child);
							Load(newValue, child, handler);
							property.SetValue(obj, newValue);
						}
						break;
					} while (true);
				}
				else
				{
					// Property not found
					if (isProperty)
					{
						throw new Exception($"Class '{type.Name}' doesn't have property '{childName}'");
					}

					// Should be widget class name then
					string widgetName = childName;
					bool isGeneric = TypeHelper.IsGenericTypeName_FrontEnd(widgetName);
					string genericTypeArgName = null;
					
					LegacyClassNameReplace(child, ref widgetName, ref isGeneric, ref genericTypeArgName);
					
					Type itemType;
					if (isGeneric)
					{
						if (!TryResolveType(widgetName, out itemType))
						{
							throw new Exception($"Could not resolve open-generic type name: '{widgetName}'");
						}
						if (!TryResolveType(genericTypeArgName, out Type genericTypeArg))
						{
							// The type might be a keyword, convert it to internal and try again
							TypeHelper.NameSwap_KeywordToDotNet(ref genericTypeArgName);
							if (!TryResolveType(genericTypeArgName, out genericTypeArg))
							{
								throw new Exception($"Could not resolve generic argument type: '{genericTypeArgName}' of '{widgetName}'");
							}
						}

						TypeHelper.SwapGenericTypeNameFormat(ref widgetName);
						itemType = itemType.MakeGenericType(genericTypeArg);
					}
					else
					{
						TryResolveType(widgetName, out itemType);
					}

					if (itemType != null)
					{
						var item = ObjectCreator(itemType, child);
						Load(item, child, handler);

						if (contentProperty == null)
						{
							throw new Exception($"Class '{type.Name}' lacks property marked with ContentAttribute");
						}

						var containerValue = contentProperty.GetValue(obj);
						var asList = containerValue as IList;
						if (asList != null)
						{
							// List
							asList.Add(item);
						} 
						else
						{
							// Simple
							contentProperty.SetValue(obj, item);
						}
					}
					else
					{
						throw new Exception($"Could not resolve type '{widgetName}'");
					}
				}
			}
		}

		private bool TryResolveType(string typeName, out Type itemType)
		{
			itemType = null;
			foreach (var pair in Assemblies)
			{
				foreach (var ns in pair.Value)
				{
					var type = pair.Key.GetType(ns + "." + typeName);
					if (type != null)
					{
						itemType = type;
						return true;
					}
				}
			}
			return false;
		}
		
		private void LegacyClassNameReplace(XElement element, ref string typeName, ref bool isGeneric, ref string genericTypeArgName)
		{
			if(LegacyClassNames == null)
				return;
			
			if (LegacyClassNames.TryGetValue(typeName, out string replace))
			{
				if (!replace.StartsWith(Project.LegacyClassToGeneric))
				{
					typeName = replace;
					return;
				}

				string[] split = replace.Split(LegacySplitChar, StringSplitOptions.RemoveEmptyEntries);
				
				isGeneric = TypeHelper.IsGenericTypeName(split[1]);
				if (isGeneric)
				{
					XAttribute attr = element.Attribute(Project.GenericTypeArgName);
					if (attr == null)
					{
						for (int i = 0; i < split.Length - 1; i++)
						{
							if (split[i] == Project.LegacyToGenericDefaultType)
								genericTypeArgName = split[i + 1]; //Set a default fallback type arg (LEGACY)
						}
						
						if (!string.IsNullOrEmpty(genericTypeArgName))
						{
							element.SetAttributeValue(Project.GenericTypeArgName, genericTypeArgName);
						}
					}
					else
					{
						genericTypeArgName = attr.Value;
					}
					
					typeName = split[1];
					TypeHelper.SwapGenericTypeNameFormat(ref typeName); //Swap end "<>" -> "`1"
				}
			}
		}
	}
}
