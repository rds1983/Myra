using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using System.Globalization;
using Microsoft.Xna.Framework.Input;

namespace Myra.Graphics2D.UI
{
	public class ExportOptions
	{
		public string Namespace { get; set; }
		public string Class { get; set; }
		public string OutputPath { get; set; }
	}

	public class Project
	{
		private readonly ExportOptions _exportOptions = new ExportOptions();

		public ExportOptions ExportOptions
		{
			get { return _exportOptions; }
		}

		public Widget Root { get; set; }

		[HiddenInEditor]
		public string StylesheetPath
		{
			get; set;
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Stylesheet Stylesheet { get; set; }

		public Project()
		{
			Stylesheet = Stylesheet.Current;
		}

		public string Save()
		{
			var root = InternalSave(this);

			var xDoc = new XDocument(root);

			return xDoc.ToString();
		}

		private static readonly Type[] SerializableTypes = new Type[]
		{
			typeof(IItemWithId),
			typeof(ExportOptions),
			typeof(Grid.Proportion)
		};

		private static void ParseProperties(Type type, out List<PropertyInfo> complexProperties, out List<PropertyInfo> simpleProperties)
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

				var attr = property.FindAttribute<JsonIgnoreAttribute>();
				if (attr != null)
				{
					continue;
				}

				if ((from t in SerializableTypes where t.GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo()) select t).FirstOrDefault() != null)
				{
					complexProperties.Add(property);
				}
				else
				{
					var propertyType = property.PropertyType;
					if (typeof(IList).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo()) && propertyType.GetTypeInfo().IsGenericType &&
						(from t in SerializableTypes where t.GetTypeInfo().IsAssignableFrom(propertyType.GenericTypeArguments[0].GetTypeInfo()) select t).FirstOrDefault() != null)
					{
						complexProperties.Add(property);
					}
					else
					{
						simpleProperties.Add(property);
					}
				}
			}
		}

		private XElement InternalSave(object obj)
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
					el.Add(InternalSave(value));
				}
				else
				{
					var collectionRoot = el;
					if (!typeof(IItemWithId).GetTypeInfo().IsAssignableFrom(property.PropertyType.GenericTypeArguments[0].GetTypeInfo()))
					{
						collectionRoot = new XElement(property.Name);
						el.Add(collectionRoot);
					}

					foreach (var comp in asList)
					{
						collectionRoot.Add(InternalSave(comp));
					}
				}
			}

			return el;
		}

		public static Project LoadFromJson(string data)
		{
			return (Project)JsonConvert.DeserializeObject(data, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Objects
			});
		}

		public static Project LoadFromXml(string data)
		{
			XDocument xDoc = XDocument.Parse(data);

			var result = new Project();
			InternalLoad(result, xDoc.Root);

			return result;
		}

		private static void InternalLoad(object obj, XElement el)
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

					if (property.PropertyType.GetTypeInfo().IsEnum)
					{
						value = Enum.Parse(property.PropertyType, attr.Value);
					}
					else if (property.PropertyType == typeof(Color) ||
						property.PropertyType == typeof(Color?))
					{
						value = attr.Value.FromName();
					}
					else
					{
						var type2 = property.PropertyType;
						if (property.PropertyType.IsNullablePrimitive())
						{
							type2 = property.PropertyType.GetNullableType();
						}

						value = Convert.ChangeType(attr.Value, type2, CultureInfo.InvariantCulture);
					}
					property.SetValue(obj, value);
				}
			}

			var widgetNamespace = typeof(Widget).GetTypeInfo().Namespace;
			foreach (var child in el.Elements())
			{
				// Find property
				var property = (from p in complexProperties where p.Name == child.Name select p).FirstOrDefault();
				if (property != null)
				{
					if (property.SetMethod == null)
					{
						// Readonly property
						var value = property.GetValue(obj);
						var asCollection = value as IList;
						if (asCollection != null)
						{
							foreach (var child2 in child.Elements())
							{
								var item = Activator.CreateInstance(property.PropertyType.GenericTypeArguments[0]);
								InternalLoad(item, child2);
								asCollection.Add(item);
							}
						}
						else
						{
							InternalLoad(value, child);
						}
					}
					else
					{
						var value = Activator.CreateInstance(property.PropertyType);
						InternalLoad(value, child);
						property.SetValue(obj, value);
					}
				}
				else
				{
					// Property not found
					// Should be widget class name then
					var widgetType = typeof(Widget).GetTypeInfo().Assembly.GetType(widgetNamespace + "." + child.Name);

					if (widgetType != null)
					{
						var item = (IItemWithId)Activator.CreateInstance(widgetType);
						InternalLoad(item, child);

						if (obj is ComboBox)
						{
							((ComboBox)obj).Items.Add((ListItem)item);
						}
						else
						if (obj is ListBox)
						{
							((ListBox)obj).Items.Add((ListItem)item);
						}
						else
						if (obj is MenuItem)
						{
							((MenuItem)obj).Items.Add((IMenuItem)item);
						}
						else if (obj is Menu)
						{
							((Menu)obj).Items.Add((IMenuItem)item);
						}
						else if (obj is Window)
						{
							((Window)obj).Content = (Widget)item;
						}
						else if (obj is MultipleItemsContainer)
						{
							((MultipleItemsContainer)obj).Widgets.Add((Widget)item);
						}
						else if (obj is SplitPane)
						{
							((SplitPane)obj).Widgets.Add((Widget)item);
						}
						else if (obj is ScrollPane)
						{
							((ScrollPane)obj).InternalChild = (Widget)item;
						}
						else if (obj is Project)
						{
							((Project)obj).Root = (Widget)item;
						}
					}
					else
					{
						throw new Exception(string.Format("Could not resolve tag '{0}'", child.Name));
					}
				}
			}
		}

		public static void PopulateFromData<T>(string data, T target) where T: Widget
		{
			var project = JObject.Parse(data);
			var root = project["Root"];

			using (var sr = root.CreateReader())
			{
				var serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Objects
				});

				serializer.Populate(sr, target);
			}
		}

		public bool ShouldSerializeProperty(Object w, PropertyInfo property)
		{
			var value = property.GetValue(w);
			if (property.HasDefaultValue(value))
			{
				return false;
			}

			var asWidget = w as Widget;
			if (asWidget != null && HasStylesheetValue(asWidget, property))
			{
				return false;
			}

			return true;
		}

		private bool HasStylesheetValue(Widget w, PropertyInfo property)
		{
			if (Stylesheet == null)
			{
				return false;
			}

			var styleName = w.StyleName;
			if (string.IsNullOrEmpty(styleName))
			{
				styleName = Stylesheet.DefaultStyleName;
			}

			var stylesheet = Stylesheet;

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