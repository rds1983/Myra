using System.Collections;
using System.Reflection;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Linq;
using System.Xml.Serialization;
using System;
using Myra.MML;
using System.Collections.Generic;
using Myra.Attributes;
using System.Linq;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Properties;
using FontStashSharp;
using Myra.Utility;
using Myra.Graphics2D.UI.File;
using AssetManagementBase;

namespace Myra.Graphics2D.UI
{
	public class ExportOptions
	{
		public string Namespace { get; set; }
		public string Class { get; set; }
		public string OutputPath { get; set; }
		public string TemplateDesigner { get; set; }
		public string TemplateMain { get; set; }
	}

	public class ObjectPosition
	{
		public object Object { get; private set; }
		public int Start { get; private set; }
		public int End { get; private set; }

		public ObjectPosition(object obj, int start, int end)
		{
			Object = obj;
			Start = start;
			End = end;
		}
	}

	public class Project
	{
		private struct StylesheetChanger: IDisposable
		{
			private readonly Stylesheet _oldStylesheet;

			public StylesheetChanger(Stylesheet newStylesheet)
			{
				_oldStylesheet = Stylesheet.Current;
				Stylesheet.Current = newStylesheet;
			}

			public void Dispose()
			{
				Stylesheet.Current = _oldStylesheet;
			}
		}

		public const string ProportionName = "Proportion";
		public const string DefaultProportionName = "DefaultProportion";
		public const string DefaultColumnProportionName = "DefaultColumnProportion";
		public const string DefaultRowProportionName = "DefaultRowProportion";

		private static readonly Dictionary<string, string> LegacyClassNames = new Dictionary<string, string>();

		private readonly ExportOptions _exportOptions = new ExportOptions();

		[Browsable(false)]
		public ExportOptions ExportOptions
		{
			get { return _exportOptions; }
		}

		[Browsable(false)]
		[Content]
		public Widget Root { get; set; }

		[Browsable(false)]
		public string StylesheetPath { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Stylesheet Stylesheet { get; set; }

		[FilePath(FileDialogMode.ChooseFolder)]
		public string DesignerRtfAssetsPath { get; set; }

		static Project()
		{
			LegacyClassNames["VerticalBox"] = "VerticalStackPanel";
			LegacyClassNames["HorizontalBox"] = "HorizontalStackPanel";
			LegacyClassNames["TextField"] = "TextBox";
			LegacyClassNames["TextBlock"] = "Label";
			LegacyClassNames["ScrollPane"] = "ScrollViewer";
		}

		public Project()
		{
			Stylesheet = Stylesheet.Current;
		}

		public static bool IsProportionName(string s)
		{
			return s.EndsWith(ProportionName) ||
				s.EndsWith(DefaultProportionName) ||
				s.EndsWith(DefaultColumnProportionName) ||
				s.EndsWith(DefaultRowProportionName);
		}

		public static bool ShouldSerializeProperty(Stylesheet stylesheet, object o, PropertyInfo p)
		{
			var asWidget = o as Widget;
			if (asWidget != null && asWidget.Parent != null && asWidget.Parent is Grid)
			{
				var container = asWidget.Parent.Parent;
				if (container != null &&
				   (container is StackPanel || container is SplitPane) &&
				   (p.Name == "GridRow" || p.Name == "GridColumn"))
				{
					// Skip serializing auto-assigned GridRow/GridColumn for SplitPane and Box containers
					return false;
				}
			}

			var asGrid = o as Grid;
			if (asGrid != null)
			{
				var value = p.GetValue(o);
				if ((p.Name == DefaultColumnProportionName || p.Name == DefaultRowProportionName) &&
					value == Proportion.GridDefault)
				{
					return false;
				}
			}

			var asBox = o as StackPanel;
			if (asBox != null)
			{
				var value = p.GetValue(o);
				if (p.Name == DefaultProportionName && value == Proportion.StackPanelDefault)
				{
					return false;
				}
			}

			if (SaveContext.HasDefaultValue(o, p))
			{
				return false;
			}

			if (asWidget != null && HasStylesheetValue(asWidget, p, stylesheet))
			{
				return false;
			}

			return true;
		}

		public bool ShouldSerializeProperty(object o, PropertyInfo p)
		{
			return ShouldSerializeProperty(Stylesheet, o, p);
		}

		internal static SaveContext CreateSaveContext(Stylesheet stylesheet)
		{
			return new SaveContext
			{
				ShouldSerializeProperty = (o, p) => ShouldSerializeProperty(stylesheet, o, p)
			};
		}

		internal SaveContext CreateSaveContext()
		{
			return CreateSaveContext(Stylesheet);
		}

		internal static LoadContext CreateLoadContext(AssetManager assetManager)
		{
			Func<Type, string, object> resourceGetter = (t, name) =>
			{
				if (t == typeof(IBrush))
				{
					return new SolidBrush(name);
				}
				else if (t == typeof(IImage))
				{
					return assetManager.LoadTextureRegion(name);
				}
				else if (t == typeof(SpriteFontBase))
				{
					return assetManager.LoadFont(name);
				}

				throw new Exception(string.Format("Type {0} isn't supported", t.Name));
			};

			return new LoadContext
			{
				Namespaces = new[]
				{
					typeof(Widget).Namespace,
					typeof(PropertyGrid).Namespace,
				},
				LegacyClassNames = LegacyClassNames,
				ObjectCreator = (t, el) => CreateItem(t, el),
				ResourceGetter = resourceGetter
			};
		}

		public string Save()
		{
			var saveContext = CreateSaveContext();
			var root = saveContext.Save(this);

			var xDoc = new XDocument(root);

			return xDoc.ToString();
		}

		public static Project LoadFromXml<T>(XDocument xDoc, AssetManager assetManager = null, T handler = null) where T : class
		{
			var stylesheet = Stylesheet.Current;
			var stylesheetPathAttr = xDoc.Root.Attribute("StylesheetPath");
			if (stylesheetPathAttr != null)
			{
				stylesheet = assetManager.LoadStylesheet(stylesheetPathAttr.Value);
			}

			var result = new Project();

			if (stylesheetPathAttr != null)
			{
				if (assetManager == null)
				{
					throw new Exception($"assetManager couldn't be null if the project has external stylesheet");
				}

				result.Stylesheet = stylesheet;
				using(var stylesheetChanger = new StylesheetChanger(stylesheet))
				{ 
					var loadContext = CreateLoadContext(assetManager);
					loadContext.Load(result, xDoc.Root, handler);
				}
			}
			else
			{
				var loadContext = CreateLoadContext(assetManager);
				loadContext.Load(result, xDoc.Root, handler);
			}

			return result;
		}

		public static Project LoadFromXml<T>(string data, AssetManager assetManager = null, T handler = null) where T : class
		{
			return LoadFromXml(XDocument.Parse(data), assetManager, handler);
		}

		public static Project LoadFromXml(XDocument xDoc, AssetManager assetManager = null)
		{
			return LoadFromXml<object>(xDoc, assetManager, null);
		}

		public static Project LoadFromXml(string data, AssetManager assetManager = null)
		{
			return LoadFromXml<object>(XDocument.Parse(data), assetManager, null);
		}

		public static object LoadObjectFromXml<T>(string data, AssetManager assetManager = null, Stylesheet stylesheet = null, T handler = null, Type parentType = null) where T : class
		{
			XDocument xDoc = XDocument.Parse(data);

			var name = xDoc.Root.Name.ToString();
			Type itemType;

			if (name == "PropertyGrid")
			{
				itemType = typeof(PropertyGrid);
			}
			else if (!IsProportionName(name))
			{
				string newName;
				if (LegacyClassNames.TryGetValue(name, out newName))
				{
					name = newName;
				}

				itemType = GetWidgetTypeByName(name);
			}
			else
			{
				itemType = typeof(Proportion);
			}

			if (itemType == null)
			{
				return null;
			}

			object item = null;
			if (stylesheet != null)
			{
				using (var stylesheetChanger = new StylesheetChanger(stylesheet))
				{
					item = CreateItem(itemType, xDoc.Root);
					var loadContext = CreateLoadContext(assetManager);
					loadContext.Load(item, xDoc.Root, handler);
				}
			}
			else
			{
				item = CreateItem(itemType, xDoc.Root);
				var loadContext = CreateLoadContext(assetManager);
				loadContext.Load(item, xDoc.Root, handler);
			}

			return item;
		}

		public static object LoadObjectFromXml(string data, AssetManager assetManager, Stylesheet stylesheet)
		{
			return LoadObjectFromXml<object>(data, assetManager, stylesheet, null);
		}

		public object LoadObjectFromXml(string data, AssetManager assetManager)
		{
			return LoadObjectFromXml(data, assetManager, Stylesheet);
		}

		public string SaveObjectToXml(object obj, string tagName, Type parentType)
		{
			var saveContext = CreateSaveContext(Stylesheet);
			return saveContext.Save(obj, true, tagName, parentType).ToString();
		}

		private static object CreateItem(Type type, XElement element)
		{
			if (typeof(Widget).IsAssignableFrom(type))
			{
				// Check whether it accepts style name parameter
				var acceptsStyleName = false;
				foreach (var c in type.GetConstructors())
				{
					var p = c.GetParameters();
					if (p != null && p.Length == 1)
					{
						if (p[0].ParameterType == typeof(string))
						{
							acceptsStyleName = true;
							break;
						}
					}
				}

				if (acceptsStyleName)
				{
					// Determine style name
					var styleName = Stylesheet.DefaultStyleName;
					var styleNameAttr = element.Attribute("StyleName");
					if (styleNameAttr != null)
					{
						var stylesNames = Stylesheet.Current.GetStylesByWidgetName(type.Name);
						if (stylesNames != null && stylesNames.Contains(styleNameAttr.Value))
						{
							styleName = styleNameAttr.Value;
						}
						else
						{
							// Remove property with absent value
							styleNameAttr.Remove();
						}
					}

					return (Widget)Activator.CreateInstance(type, styleName);
				}
			}

			return Activator.CreateInstance(type);
		}

		private static bool HasStylesheetValue(Widget w, PropertyInfo property, Stylesheet stylesheet)
		{
			if (stylesheet == null)
			{
				return false;
			}

			var styleName = w.StyleName;
			if (string.IsNullOrEmpty(styleName))
			{
				styleName = Stylesheet.DefaultStyleName;
			}

			// Find styles dict of that widget
			var typeName = w.GetType().Name;
			var styleTypeNameAttribute = w.GetType().FindAttribute<StyleTypeNameAttribute>();
			if (styleTypeNameAttribute != null)
			{
				typeName = styleTypeNameAttribute.Name;
			}

			var stylesDictPropertyName = typeName + "Styles";
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
			if (!stylesDict.Contains(styleName))
			{
				styleName = Stylesheet.DefaultStyleName;
			}

			object obj = stylesDict[styleName];

			// Now find corresponding property
			PropertyInfo styleProperty = null;

			var stylePropertyPathAttribute = property.FindAttribute<StylePropertyPathAttribute>();
			if (stylePropertyPathAttribute != null)
			{
				var path = stylePropertyPathAttribute.Name;
				if (path.StartsWith("/"))
				{
					obj = stylesheet;
					path = path.Substring(1);
				}

				var parts = path.Split('/');
				for (var i = 0; i < parts.Length; ++i)
				{
					styleProperty = obj.GetType().GetRuntimeProperty(parts[i]);

					if (i < parts.Length - 1)
					{
						obj = styleProperty.GetValue(obj);
					}
				}
			}
			else
			{
				styleProperty = obj.GetType().GetRuntimeProperty(property.Name);
			}

			if (styleProperty == null)
			{
				return false;
			}

			// Compare values
			var styleValue = styleProperty.GetValue(obj);
			var value = property.GetValue(w);
			if (!Equals(styleValue, value))
			{
				return false;
			}

			return true;
		}

		public static Type GetWidgetTypeByName(string name)
		{
			var itemNamespace = typeof(Widget).Namespace;
			return typeof(Widget).Assembly.GetType(itemNamespace + "." + name);
		}
	}
}