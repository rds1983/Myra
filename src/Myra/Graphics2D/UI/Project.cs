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
using XNAssets.Utility;
using Myra.Graphics2D.Brushes;
using XNAssets;

#if !STRIDE
using Microsoft.Xna.Framework.Graphics;
#else
using Stride.Core.Mathematics;
using Stride.Graphics;
#endif

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

	public class Project
	{
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
		public string StylesheetPath
		{
			get; set;
		}

		[Browsable(false)]
		[XmlIgnore]
		public Stylesheet Stylesheet { get; set; }

		static Project()
		{
			LegacyClassNames["Button"] = "ImageTextButton";
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

			if(asWidget != null && HasStylesheetValue(asWidget, p, stylesheet))
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

		internal static LoadContext CreateLoadContext(IAssetManager assetManager, Stylesheet stylesheet)
		{
			Func<Type, string, object> resourceGetter = (t, name) =>
			{
				if (t == typeof(IBrush))
				{
					return new SolidBrush(name);
				}
				else if (t == typeof(IImage))
				{
					return assetManager.Load<TextureRegion>(name);
				}
				else if (t == typeof(SpriteFont))
				{
					return assetManager.Load<SpriteFont>(name);
				}

				throw new Exception(string.Format("Type {0} isn't supported", t.Name));
			};

			return new LoadContext
			{
				LegacyClassNames = LegacyClassNames,
				ObjectCreator = (t, el) => CreateItem(t, el, stylesheet),
				Namespace = typeof(Widget).Namespace,
				ResourceGetter = resourceGetter
			};
		}

		internal LoadContext CreateLoadContext(IAssetManager assetManager)
		{
			return CreateLoadContext(assetManager, Stylesheet);
		}

		public string Save()
		{
			var saveContext = CreateSaveContext();
			var root = saveContext.Save(this);

			var xDoc = new XDocument(root);

			return xDoc.ToString();
		}

		public static Project LoadFromXml(XDocument xDoc, IAssetManager assetManager, Stylesheet stylesheet)
		{
			var result = new Project
			{
				Stylesheet = stylesheet
			};

			var loadContext = result.CreateLoadContext(assetManager);
			loadContext.Load(result, xDoc.Root);

			return result;
		}

		public static Project LoadFromXml(string data, IAssetManager assetManager, Stylesheet stylesheet)
		{
			return LoadFromXml(XDocument.Parse(data), assetManager, stylesheet);
		}

		public static Project LoadFromXml(string data)
		{
			return LoadFromXml(data, null, Stylesheet.Current);
		}

		public static object LoadObjectFromXml(string data, IAssetManager assetManager, Stylesheet stylesheet)
		{
			XDocument xDoc = XDocument.Parse(data);

			Type itemType;
			if (!IsProportionName(xDoc.Root.Name.ToString()))
			{
				var itemNamespace = typeof(Widget).Namespace;

				var widgetName = xDoc.Root.Name.ToString();
				string newName;
				if (LegacyClassNames.TryGetValue(widgetName, out newName))
				{
					widgetName = newName;
				}

				itemType = typeof(Widget).Assembly.GetType(itemNamespace + "." + widgetName);
			}
			else
			{
				itemType = typeof(Proportion);
			}

			if (itemType == null)
			{
				return null;
			}

			var item = CreateItem(itemType, xDoc.Root, stylesheet);
			var loadContext = CreateLoadContext(assetManager, stylesheet);
			loadContext.Load(item, xDoc.Root);

			return item;
		}

		public object LoadObjectFromXml(string data, IAssetManager assetManager)
		{
			return LoadObjectFromXml(data, assetManager, Stylesheet);
		}

		public string SaveObjectToXml(object obj, string tagName)
		{
			var saveContext = CreateSaveContext(Stylesheet);
			return saveContext.Save(obj, true, tagName).ToString();
		}

		private static object CreateItem(Type type, XElement element, Stylesheet stylesheet)
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
					var result = (Widget)Activator.CreateInstance(type, (string)null);

					// Determine style name
					var styleName = Stylesheet.DefaultStyleName;
					var styleNameAttr = element.Attribute("StyleName");
					if (styleNameAttr != null)
					{
						var stylesNames = stylesheet.GetStylesByWidgetName(type.Name);
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

					// Set style
					result.SetStyle(stylesheet, styleName);

					return result;
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

			object obj =  stylesDict[styleName];

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
	}
}