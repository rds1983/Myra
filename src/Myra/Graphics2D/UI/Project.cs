using System.Reflection;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Linq;
using System.Xml.Serialization;
using System;
using Myra.MML;
using System.Collections.Generic;
using Myra.Attributes;
using Myra.Graphics2D.UI.Properties;
using Myra.Utility;
using Myra.Graphics2D.UI.File;
using AssetManagementBase;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Specifies the visibility of the fields in the exported code.
	/// </summary>
	public enum ExportOptionsFieldsVisibility
	{
		/// <summary>
		/// The fields are public.
		/// </summary>
		Public,
		/// <summary>
		/// The fields are protected.
		/// </summary>
		Protected,
		/// <summary>
		/// The fields are private.
		/// </summary>
		Private
	}

	/// <summary>
	/// Options for exporting a UI project.
	/// </summary>
	public class ExportOptions
	{
		/// <summary>
		/// Gets or sets the namespace for the exported code.
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// Gets or sets the class name for the exported code.
		/// </summary>
		public string Class { get; set; }

		/// <summary>
		/// Gets or sets the output path for the exported files.
		/// </summary>
		public string OutputPath { get; set; }

		/// <summary>
		/// Gets or sets the template for the designer file.
		/// </summary>
		public string TemplateDesigner { get; set; }

		/// <summary>
		/// Gets or sets the template for the main file.
		/// </summary>
		public string TemplateMain { get; set; }

		/// <summary>
		/// Gets or sets the visibility of the fields in the exported code.
		/// </summary>
		[DefaultValue(ExportOptionsFieldsVisibility.Public)]
		public ExportOptionsFieldsVisibility FieldsVisibility { get; set; } = ExportOptionsFieldsVisibility.Public;

		/// <summary>
		/// Gets or sets a value indicating whether the exported code receives a <see cref="Stylesheet"/> parameter and passes it to the styled widgets.
		/// </summary>
		[DefaultValue(false)]
		public bool IsStyleable { get; set; } = false;
	}

	/// <summary>
	/// Represents the position of an object in a document.
	/// </summary>
	public class ObjectPosition
	{
		/// <summary>
		/// Gets the object.
		/// </summary>
		public object Object { get; private set; }

		/// <summary>
		/// Gets the starting position.
		/// </summary>
		public int Start { get; private set; }

		/// <summary>
		/// Gets the ending position.
		/// </summary>
		public int End { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectPosition"/> class with the specified object and positions.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="start">The starting position.</param>
		/// <param name="end">The ending position.</param>
		public ObjectPosition(object obj, int start, int end)
		{
			Object = obj;
			Start = start;
			End = end;
		}
	}

	/// <summary>
	/// Represents a UI project that can be saved, loaded, and exported to code.
	/// </summary>
	public class Project
	{
		/// <summary>Constant name for proportion values.</summary>
		public const string ProportionName = "Proportion";
		/// <summary>Constant name for default proportion values.</summary>
		public const string DefaultProportionName = "DefaultProportion";
		/// <summary>Constant name for default column proportion values.</summary>
		public const string DefaultColumnProportionName = "DefaultColumnProportion";
		/// <summary>Constant name for default row proportion values.</summary>
		public const string DefaultRowProportionName = "DefaultRowProportion";

		// Maps old deprecated class names to their modern replacements for backward compatibility
		private static readonly Dictionary<string, string> LegacyClassNames = new Dictionary<string, string>();

		private readonly ExportOptions _exportOptions = new ExportOptions();  // Code export settings

		/// <summary>
		/// Gets the export options for this project.
		/// </summary>
		[Browsable(false)]
		public ExportOptions ExportOptions
		{
			get { return _exportOptions; }
		}

		/// <summary>
		/// Gets or sets the root widget of the project.
		/// </summary>
		[Browsable(false)]
		[Content]
		public Widget Root { get; set; }

		/// <summary>
		/// Gets or sets the path to the stylesheet file.
		/// </summary>
		[Browsable(false)]
		public string StylesheetPath { get; set; }

		/// <summary>
		/// Gets or sets the stylesheet for this project.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Stylesheet Stylesheet { get; set; }

		/// <summary>
		/// Gets or sets the designer runtime assets folder path.
		/// </summary>
		[FilePath(FileDialogMode.ChooseFolder)]
		public string DesignerRtfAssetsPath { get; set; }

		/// <summary>
		/// Gets the mapping of loaded objects to their respective XML nodes.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public List<Tuple<object, XElement>> ObjectsNodes { get; internal set; }

		// Initializes legacy class name mappings for loading old project files
		static Project()
		{
			LegacyClassNames["VerticalBox"] = "VerticalStackPanel";
			LegacyClassNames["HorizontalBox"] = "HorizontalStackPanel";
			LegacyClassNames["TextField"] = "TextBox";
			LegacyClassNames["TextBlock"] = "Label";
			LegacyClassNames["ScrollPane"] = "ScrollViewer";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Project"/> class with the specified stylesheet.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for this project.</param>
		public Project(Stylesheet stylesheet)
		{
			Stylesheet = stylesheet ?? throw new ArgumentNullException(nameof(stylesheet));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Project"/> class using the current stylesheet.
		/// </summary>
		public Project() : this(Stylesheet.Current)
		{
		}

		/// <summary>
		/// Determines whether the specified name is a proportion property name.
		/// </summary>
		/// <param name="s">The name to check.</param>
		/// <returns>true if the name is a proportion name; otherwise, false.</returns>
		public static bool IsProportionName(string s)
		{
			return s.EndsWith(ProportionName) ||
				s.EndsWith(DefaultProportionName) ||
				s.EndsWith(DefaultColumnProportionName) ||
				s.EndsWith(DefaultRowProportionName);
		}

		/// <summary>
		/// Determines whether a property should be serialized for the specified object.
		/// Omits properties that have default values, match stylesheet, or are auto-managed layout properties.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for comparison.</param>
		/// <param name="o">The object containing the property.</param>
		/// <param name="p">The property information.</param>
		/// <returns>true if the property should be serialized; otherwise, false.</returns>
		internal static bool ShouldSerializeProperty(Stylesheet stylesheet, object o, PropertyInfo p)
		{
			// Skip auto-assigned GridRow/GridColumn when widget is in a SplitPane or StackPanel container
			var asWidget = o as Widget;
			if (asWidget != null && asWidget.Parent != null && asWidget.Parent is Grid)
			{
				var container = asWidget.Parent.Parent;
				if (container != null &&
				   (container is StackPanel || container is SplitPane) &&
				   (p.Name == "GridRow" || p.Name == "GridColumn"))
				{
					return false;
				}
			}

			// Skip default proportion values for Grid
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

			// Skip default proportion values for StackPanel
			var asBox = o as StackPanel;
			if (asBox != null)
			{
				var value = p.GetValue(o);
				if (p.Name == DefaultProportionName && value == Proportion.StackPanelDefault)
				{
					return false;
				}
			}

			// Skip properties that have default values (not modified)
			if (SaveContext.HasDefaultValue(o, p))
			{
				return false;
			}

			// Skip properties that match stylesheet values (inherited from style)
			if (asWidget != null && HasStylesheetValue(asWidget, p, stylesheet))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Determines whether a property should be serialized for the specified object using this project's stylesheet.
		/// </summary>
		/// <param name="o">The object containing the property.</param>
		/// <param name="p">The property information.</param>
		/// <returns>true if the property should be serialized; otherwise, false.</returns>
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

		// Creates save context using this project's stylesheet
		internal SaveContext CreateSaveContext()
		{
			return CreateSaveContext(Stylesheet);
		}

		/// <summary>
		/// Gets or sets the extra widget assemblies and namespaces to include during project loading and saving.
		/// </summary>
		public static Dictionary<Assembly, string[]> ExtraWidgetAssembliesAndNamespaces = new Dictionary<Assembly, string[]>();

		// Creates a load context for deserializing UI projects from XML.
		// Sets up asset loading, widget type resolution, and legacy name mapping.
		internal static LoadContext CreateLoadContext(AssetManager assetManager, Stylesheet stylesheet)
		{
			// Collect widget assemblies: both Myra core types and user-supplied custom widgets
			Dictionary<Assembly, string[]> assemblies = new Dictionary<Assembly, string[]>(ExtraWidgetAssembliesAndNamespaces);
			assemblies.Add(typeof(Widget).Assembly, new string[] { typeof(Widget).Namespace, typeof(PropertyGrid).Namespace });

			return new LoadContext
			{
				Assemblies = assemblies,
				LegacyClassNames = LegacyClassNames,
				ObjectCreator = (t, el) => CreateItem(t, el, stylesheet),
				AssetManager = assetManager,
				Stylesheet = stylesheet
			};
		}

		/// <summary>
		/// Saves the project to an XML string.
		/// </summary>
		/// <returns>An XML string representation of the project.</returns>
		public string ToXml()
		{
			var saveContext = CreateSaveContext();
			var root = saveContext.Save(this);

			var xDoc = new XDocument(root);

			return xDoc.ToString();
		}

		/// <summary>
		/// Saves the project to an XML string. This method is obsolete; use <see cref="ToXml"/> instead.
		/// </summary>
		/// <returns>An XML string representation of the project.</returns>
		[Obsolete("Use ToXml")]
		public string Save() => ToXml();

		/// <summary>
		/// Loads a project from an XML string representation.
		/// </summary>
		/// <param name="data">The XML string containing the project definition.</param>
		/// <param name="assetManager">The asset manager used to load resources referenced by the project. If null, resources will not be loaded.</param>
		/// <param name="customStylesheet">An optional custom stylesheet to apply to the project. If not provided, the stylesheet path from the project's XML will be used.</param>
		/// <returns>A new Project instance loaded from the provided XML data, or null if loading fails.</returns>
		public static Project LoadFromXml(string data, AssetManager assetManager = null, Stylesheet customStylesheet = null)
		{
			var xDoc = XDocument.Parse(data, LoadOptions.SetLineInfo);

			// Check if project specifies external stylesheet
			Stylesheet stylesheet;
			if (customStylesheet == null)
			{
				var stylesheetPathAttr = xDoc.Root.Attribute("StylesheetPath");
				if (stylesheetPathAttr != null)
				{
					if (assetManager == null)
					{
						throw new Exception($"assetManager couldn't be null if the project has external stylesheet");
					}

					stylesheet = assetManager.LoadStylesheet(stylesheetPathAttr.Value);
				}
				else
				{
					stylesheet = Stylesheet.Current;
				}
			}
			else
			{
				stylesheet = customStylesheet;
			}

			var result = new Project(stylesheet);

			var loadContext = CreateLoadContext(assetManager, stylesheet);
			loadContext.Load(result, xDoc.Root);
			result.ObjectsNodes = loadContext.ObjectsNodes;

			return result;
		}

		/// <summary>
		/// Loads a single object from XML string data.
		/// Determines object type from XML tag name, resolving legacy names and special types.
		/// </summary>
		/// <param name="data">The XML data as a string.</param>
		/// <param name="assetManager">The asset manager for loading resources.</param>
		/// <param name="stylesheet">The stylesheet to apply to loaded objects.</param>
		/// <param name="parentType">The parent type context for loading.</param>
		/// <returns>The loaded object.</returns>
		internal static object LoadObjectFromXml(string data, AssetManager assetManager = null, Stylesheet stylesheet = null, Type parentType = null)
		{
			XDocument xDoc = XDocument.Parse(data, LoadOptions.SetLineInfo);

			var name = xDoc.Root.Name.ToString();
			Type itemType;

			// Determine type from XML tag name
			if (name == "PropertyGrid")
			{
				itemType = typeof(PropertyGrid);
			}
			else if (!IsProportionName(name))
			{
				// Check if it's a legacy name and get modern name
				string newName;
				if (LegacyClassNames.TryGetValue(name, out newName))
				{
					name = newName;
				}

				// Look up widget type by name in Myra assemblies
				itemType = GetWidgetTypeByName(name);
			}
			else
			{
				// It's a Proportion (layout configuration)
				itemType = typeof(Proportion);
			}

			if (itemType == null)
			{
				return null;
			}

			// Create and load object, applying stylesheet context if provided
			var item = CreateItem(itemType, xDoc.Root, stylesheet);
			var loadContext = CreateLoadContext(assetManager, stylesheet);
			loadContext.Load(item, xDoc.Root);

			return item;
		}

		/// <summary>
		/// Saves an object to an XML string using this project's stylesheet.
		/// Serializes only properties that differ from stylesheet defaults.
		/// </summary>
		/// <param name="obj">The object to save.</param>
		/// <param name="tagName">The XML tag name for the object.</param>
		/// <param name="parentType">The parent type context for saving.</param>
		/// <returns>An XML string representation of the object.</returns>
		internal string SaveObjectToXml(object obj, string tagName, Type parentType)
		{
			var saveContext = CreateSaveContext(Stylesheet);
			return saveContext.Save(obj, true, tagName, parentType).ToString();
		}

		// Instantiates an object of the given type, handling special case of Widget constructors that accept StyleName parameter
		private static object CreateItem(Type type, XElement element, Stylesheet stylesheet)
		{
			if (typeof(Widget).IsAssignableFrom(type))
			{
				if (type.IsStyleable())
				{
					if (stylesheet == null)
					{
						throw new NullReferenceException(nameof(stylesheet));
					}

					// Extract StyleName from XML attribute, defaulting if not found
					var styleName = Stylesheet.DefaultStyleName;
					var styleNameAttr = element.Attribute("StyleName");
					if (styleNameAttr != null)
					{
						styleName = styleNameAttr.Value;
					}

					// Create widget with style name parameter
					try
					{
						return (Widget)Activator.CreateInstance(type, stylesheet, styleName);
					}
					catch (TargetInvocationException ex)
					{
						if (ex.InnerException != null)
						{
							throw ex.InnerException;
						}

						throw ex;
					}
				}
			}

			// Create non-widget object or widget without style parameter
			return Activator.CreateInstance(type);
		}

		// Checks if widget property value matches the value defined in the stylesheet.
		// Used to skip serializing properties that are already defined by the applied style.
		private static bool HasStylesheetValue(Widget w, PropertyInfo property, Stylesheet stylesheet)
		{
			if (stylesheet == null || w.GetStylesDictionary(stylesheet) == null)
			{
				return false;
			}

			// Get style name: use widget's style or default
			var styleName = w.StyleName;
			if (string.IsNullOrEmpty(styleName))
			{
				styleName = Stylesheet.DefaultStyleName;
			}

			object obj = null;
			try
			{
				obj = w.GetStyle(stylesheet, styleName);
			}
			catch
			{
				// If there's an exception, return false(meaning the widget property doesnt have the stylesheet value)
				return false;
			}

			if (obj == null)
			{
				return false;
			}

			// Navigate to the property in stylesheet using reflection (supports nested paths)
			PropertyInfo styleProperty = null;

			var stylePropertyPathAttribute = property.FindAttribute<StylePropertyPathAttribute>();
			if (stylePropertyPathAttribute != null)
			{
				// Custom path specified (e.g., "/SomeProperty/NestedProperty")
				var path = stylePropertyPathAttribute.Name;
				if (path.StartsWith("/"))
				{
					obj = stylesheet;
					path = path.Substring(1);
				}

				// Traverse path segments separated by '/'
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
				// Use property name directly
				styleProperty = obj.GetType().GetRuntimeProperty(property.Name);
			}

			if (styleProperty == null)
			{
				return false;
			}

			// Compare values: if they match, property is inherited from stylesheet
			var styleValue = styleProperty.GetValue(obj);
			var value = property.GetValue(w);

			if (styleValue == null && value == null)
			{
				return true;
			}
			else if (styleValue == null || value == null)
			{
				return false;
			}

			if (BaseContext.IsTypeExternalAsset(property.PropertyType))
			{
				// Just compare strings
				return styleValue.ToString() == value.ToString();
			}

			return Equals(styleValue, value);
		}

		/// <summary>
		/// Gets the widget type by its name.
		/// Resolves by looking up the type in the Myra.Graphics2D.UI namespace.
		/// </summary>
		/// <param name="name">The name of the widget type.</param>
		/// <returns>The widget type, or null if not found.</returns>
		public static Type GetWidgetTypeByName(string name)
		{
			// Look up type in Widget's namespace and assembly
			var itemNamespace = typeof(Widget).Namespace;
			return typeof(Widget).Assembly.GetType(itemNamespace + "." + name);
		}
	}
}