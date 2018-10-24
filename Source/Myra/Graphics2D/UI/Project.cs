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
using Newtonsoft.Json.Serialization;

namespace Myra.Graphics2D.UI
{
	public class ExportOptions
	{
		public string Namespace { get; set; }
		public string Class { get; set; }
		public string OutputPath { get; set; }
	}

	internal class ShouldWriteContractResolver : DefaultContractResolver
	{
		public Project Project { get; private set; }

		public ShouldWriteContractResolver(Project project)
		{
			Project = project;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			JsonProperty property = base.CreateProperty(member, memberSerialization);

			property.ShouldSerialize =
				instance =>
				{
					var asPropertyInfo = member as PropertyInfo;
					if (asPropertyInfo == null)
					{
						return true;
					}

					return Project.ShouldSerializeProperty(instance, asPropertyInfo);
				};


			return property;
		}
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
		[JsonIgnore]
		public Stylesheet Stylesheet { get; set; }

		public Project()
		{
			Stylesheet = Stylesheet.Current;
		}

		public string Save()
		{
			var result = JsonConvert.SerializeObject(this, Formatting.Indented,
				new JsonSerializerSettings
				{
					ContractResolver = new ShouldWriteContractResolver(this),
					PreserveReferencesHandling = PreserveReferencesHandling.Objects,
					TypeNameHandling = TypeNameHandling.Objects
				});

			return result;
		}


		public static Project LoadFromData(string data)
		{
			var result = (Project)JsonConvert.DeserializeObject(data, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Objects
			});

			return result;
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
