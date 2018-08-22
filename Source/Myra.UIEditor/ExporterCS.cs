using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Editor.Utils;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.UIEditor
{
	public class ExporterCS
	{
		private readonly Project _project;
		private readonly Dictionary<string, int> ids = new Dictionary<string, int>();
		private readonly StringBuilder sbFields = new StringBuilder();
		private readonly StringBuilder sbBuild = new StringBuilder();
		private bool isFirst = true;

		public ExporterCS(Project project)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}

			_project = project;
		}

		public string ExportMain()
		{
			var path = Path.Combine(_project.ExportOptions.OutputPath, _project.ExportOptions.Class + ".cs");
			if (File.Exists(path))
			{
				// Do not overwrite main
				return string.Empty;
			}

			var template = Resources.ExportCSMain;

			template = template.Replace("$namespace$", _project.ExportOptions.Namespace);
			template = template.Replace("$class$", _project.ExportOptions.Class);
			template = template.Replace("$parentClass$", _project.Root.GetType().Name);
			template = template.Replace("$generationDate$", DateTime.Now.ToString());

			File.WriteAllText(path, template);

			return path;
		}

		private static string LowercaseFirstLetter(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}

			return char.ToLowerInvariant(s[0]) + s.Substring(1);
		}

		private static bool IsSkippable(object value, PropertyInfo property)
		{
			var defaultValue = GetDefaultValue(property.PropertyType);
			var defaultAttribute = property.FindAttribute<DefaultValueAttribute>();
			if ((defaultAttribute != null && Equals(value, defaultAttribute.Value)) ||
				(defaultAttribute == null && Equals(value, defaultValue)))
			{
				// Skip default
				return true;
			}

			return false;
		}

		private bool HasStylesheetValue(Widget w, PropertyInfo property, string styleName)
		{
			if (_project.Stylesheet == null)
			{
				return false;
			}

			return Serialization.HasStylesheetValue(_project.Stylesheet, w, property, styleName);
		}

		public string ExportDesignerRecursive(IItemWithId w)
		{
			var properties = BuildProperties(w.GetType());
			var simpleProperties = new List<PropertyInfo>();

			// Build subitems data
			var subItems = new List<string>();
			string styleName = string.Empty;
			foreach (var property in properties)
			{
				var value = property.GetValue(w);

				if (property.Name == "StyleName")
				{
					// Special case
					styleName = (string) value;
					continue;
				}

				if (value == null)
				{
					simpleProperties.Add(property);
					continue;
				}

				if (value is IItemWithId)
				{
					var subItemId = ExportDesignerRecursive((IItemWithId) value);
					var subItemCode = string.Format("{0} = {1}", property.Name, subItemId);
					subItems.Add(subItemCode);
				}
				else
				{

					var type = value.GetType();
					var asList = value as IList;

					if (asList != null && type.IsGenericType &&
					    typeof (IItemWithId).IsAssignableFrom(type.GetGenericArguments()[0]))
					{
						foreach (var comp in asList)
						{
							var subItemId = ExportDesignerRecursive((IItemWithId) comp);
							var subItemCode = string.Format("{0}.Add({1})", property.Name, subItemId);
							subItems.Add(subItemCode);
						}
					}
					else
					{
						simpleProperties.Add(property);
					}
				}
			}

			// Write code of this item
			if (!isFirst)
			{
				sbBuild.Append("\n\n\t\t\t");
			}

			isFirst = false;

			string id;
			var typeName = w.GetType().GetFriendlyName();
			if (_project.Root == w)
			{
				id = string.Empty;
			}
			else
			{
				id = w.Id;
				if (string.IsNullOrEmpty(id))
				{
					var onlyTypeName = LowercaseFirstLetter(w.GetType().GetOnlyTypeName());
					int count;
					if (!ids.TryGetValue(onlyTypeName, out count))
					{
						count = 1;
					}
					else
					{
						++count;
					}
					ids[onlyTypeName] = count;

					id = onlyTypeName + count;

					sbBuild.Append("var " + id);
				}
				else
				{
					sbBuild.Append(w.Id);
				}
			}

			var idPrefix = string.IsNullOrEmpty(id) ? string.Empty : id + ".";
			if (!string.IsNullOrEmpty(id))
			{
				sbBuild.Append(" = new " + typeName + "(" +
				               (string.IsNullOrEmpty(styleName) ? string.Empty : ("\"" + styleName + "\"")) + ");");
			}

			if (!string.IsNullOrEmpty(w.Id) && _project.Root != w)
			{
				if (!isFirst)
				{
					sbFields.Append("\n\t\t");
				}
				sbFields.Append("public " + w.GetType().Name + " " + w.Id + ";");
			}

			foreach (var property in simpleProperties)
			{
				var value = property.GetValue(w);
				if (IsSkippable(value, property))
				{
					continue;
				}

				var asWidget = w as Widget;
				if (asWidget != null && HasStylesheetValue(asWidget, property, styleName))
				{
					continue;
				}

				sbBuild.Append(BuildPropertyCode(property, w, idPrefix));
			}

			foreach (var subItem in subItems)
			{
				sbBuild.Append("\n\t\t\t");
				sbBuild.Append(idPrefix);
				sbBuild.Append(subItem);
				sbBuild.Append(";");
			}

			return id;
		}

		public string ExportDesigner()
		{
			var template = Resources.ExportCSDesigner;

			template = template.Replace("$namespace$", _project.ExportOptions.Namespace);
			template = template.Replace("$class$", _project.ExportOptions.Class);
			template = template.Replace("$generationDate$", DateTime.Now.ToString());

			ids.Clear();
			sbFields.Clear();
			sbBuild.Clear();

			isFirst = true;
			ExportDesignerRecursive(_project.Root);

			template = template.Replace("$fields$", sbFields.ToString());
			template = template.Replace("$build$", sbBuild.ToString());


			var path = Path.Combine(_project.ExportOptions.OutputPath, _project.ExportOptions.Class + ".Generated.cs");
			File.WriteAllText(path, template);

			return path;
		}

		private static List<PropertyInfo> BuildProperties(Type type)
		{
			var result = new List<PropertyInfo>();
			var properties = from p in type.GetProperties() select p;
			foreach (var property in properties)
			{
				if (property.GetGetMethod() == null ||
				    !property.GetGetMethod().IsPublic ||
				    property.GetGetMethod().IsStatic)
				{
					continue;
				}

				var attr = property.FindAttribute<JsonIgnoreAttribute>();
				if (attr != null)
				{
					continue;
				}

				result.Add(property);
			}

			return result;
		}

		private static string BuildPropertyCode(PropertyInfo property, object o, string idPrefix)
		{
			var sb = new StringBuilder();

			var value = property.GetValue(o);

			var asList = value as IList;
			if (asList == null)
			{
				sb.Append("\n\t\t\t" + idPrefix + property.Name);
				sb.Append(" = ");
				sb.Append(BuildValue(value));
				sb.Append(";");
			}
			else
			{
				foreach (var comp in asList)
				{
					sb.Append("\n\t\t\t" + idPrefix + property.Name);
					sb.Append(".Add(");
					sb.Append(BuildValue(comp));
					sb.Append(");");
				}
			}

			return sb.ToString();
		}

		private static object GetDefaultValue(Type type)
		{
			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}

			return null;
		}

		private static string BuildValue(object value)
		{
			if (value == null)
			{
				return "null";
			}

			if (value is bool)
			{
				return (bool) value ? "true" : "false";
			}

			var asString = value as string;
			if (asString != null)
			{
				return "\"" + asString + "\"";
			}

			if (value is Color)
			{
				var name = ((Color) value).GetColorName();
				if (!string.IsNullOrEmpty(name))
				{
					return "Color." + name;
				}
			}

			if (value.GetType().IsPrimitive)
			{
				return value.ToString();
			}

			var sb = new StringBuilder();
			if (value.GetType().IsEnum)
			{
				sb.Append(value.GetType());
				sb.Append(".");
				sb.Append(value);
				return sb.ToString().Replace("+", ".");
			}

			sb.Append("new " + value.GetType().GetFriendlyName());

			var isEmpty = true;
			var properties = from p in value.GetType().GetProperties() select p;
			foreach (var property in properties)
			{
				if (property.GetGetMethod() == null ||
				    property.GetSetMethod() == null ||
				    !property.GetGetMethod().IsPublic ||
				    property.GetGetMethod().IsStatic)
				{
					continue;
				}

				var jsonIgnoreAttribute = property.FindAttribute<JsonIgnoreAttribute>();
				if (jsonIgnoreAttribute != null)
				{
					continue;
				}

				var subValue = property.GetValue(value);

				if (IsSkippable(subValue, property))
				{
					continue;
				}

				if (value.GetType().Name == "Color" && property.Name == "PackedValue")
				{
					// Skip unused properties of MG
					continue;
				}

				if (isEmpty)
				{
					sb.Append("\n\t\t\t{");
					isEmpty = false;
				}

				sb.Append("\n\t\t\t\t" + property.Name);
				sb.Append(" = ");
				sb.Append(BuildValue(subValue));
				sb.Append(",");
			}

			if (!isEmpty)
			{
				sb.Append("\n\t\t\t}");
			}
			else
			{
				sb.Append("()");
			}

			return sb.ToString();
		}

		public string[] Export()
		{
			if (string.IsNullOrEmpty(_project.ExportOptions.Namespace))
			{
				throw new Exception("Namespace could not be empty.");
			}

			if (string.IsNullOrEmpty(_project.ExportOptions.Class))
			{
				throw new Exception("Class could not be empty.");
			}

			if (string.IsNullOrEmpty(_project.ExportOptions.OutputPath))
			{
				throw new Exception("Output path could not be empty.");
			}

			var result = new List<string>();

			var path = ExportMain();
			if (!string.IsNullOrEmpty(path))
			{
				result.Add(path);
			}

			path = ExportDesigner();
			if (!string.IsNullOrEmpty(path))
			{
				result.Add(path);
			}

			return result.ToArray();
		}
	}
}