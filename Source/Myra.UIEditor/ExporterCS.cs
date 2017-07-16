using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Graphics2D.UI;
using Myra.Utility;
using Container = Myra.Graphics2D.UI.Container;

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

		public void ExportMain()
		{
			var template = Resources.ExportCSMain;

			template = template.Replace("$namespace$", _project.ExportOptions.Namespace);
			template = template.Replace("$class$", _project.ExportOptions.Class);

			var path = Path.Combine(_project.ExportOptions.OutputPath, _project.ExportOptions.Class + ".cs");
			File.WriteAllText(path, template);
		}

		private static string LowercaseFirstLetter(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}

			return Char.ToLowerInvariant(s[0]) + s.Substring(1);
		}

		public string ExportDesignerRecursive(Widget w)
		{
			if (!isFirst)
			{
				sbBuild.Append("\n\n\t\t\t");
			}

			string id;
			var typeName = w.GetType().Name;
			if (_project.Root == w)
			{
				id = string.Empty;
			}
			else
			{
				id = w.Id;
				if (string.IsNullOrEmpty(id))
				{
					int count;
					if (!ids.TryGetValue(typeName, out count))
					{
						count = 1;
					}
					else
					{
						++count;
					}
					ids[typeName] = count;

					id = LowercaseFirstLetter(typeName) + count;

					sbBuild.Append("var " + id);
				}
				else
				{
					sbBuild.Append(w.Id);
				}
			}

			if (!string.IsNullOrEmpty(id))
			{
				sbBuild.Append(" = new " + typeName + "();");
			}

			var idPrefix = string.IsNullOrEmpty(id) ? string.Empty : id + ".";
			sbBuild.Append(BuildPropertiesCode(w, idPrefix));

			if (!string.IsNullOrEmpty(w.Id) && _project.Root != w)
			{
				if (!isFirst)
				{
					sbFields.Append("\n\t\t");
				}
				sbFields.Append("public " + w.GetType().Name + " " + w.Id + ";");
			}

			isFirst = false;

			var asContainer = w as Container;
			if (asContainer != null && asContainer.GetType().FindAttribute<IgnoreChildrenAttribute>() == null)
			{
				foreach (var widget in asContainer.Children)
				{
					var childId = ExportDesignerRecursive(widget);

					sbBuild.Append("\n\t\t\t" + idPrefix + "Widgets.Add(" + childId + ");");
				}
			}

			return id;
		}

		public void ExportDesigner()
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
		}

		private static string BuildPropertiesCode(Widget w, string idPrefix)
		{
			var sb = new StringBuilder();

			var properties = from p in w.GetType().GetProperties() select p;
			foreach (var property in properties)
			{
				if (property.GetGetMethod() == null ||
					!property.GetGetMethod().IsPublic ||
					property.GetGetMethod().IsStatic)
				{
					continue;
				}

				var browsableAttr = property.FindAttribute<BrowsableAttribute>();
				if (browsableAttr != null && !browsableAttr.Browsable)
				{
					continue;
				}

				var hiddenAttr = property.FindAttribute<HiddenInEditorAttribute>();
				if (hiddenAttr != null)
				{
					continue;
				}

				var value = property.GetValue(w);

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
			}

			return sb.ToString();
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

			sb.Append("new " + value.GetType().Name + "\n\t\t\t{");

			var properties = from p in value.GetType().GetProperties() select p;
			foreach (var property in properties)
			{
				if (property.GetGetMethod() == null ||
					!property.GetGetMethod().IsPublic ||
					property.GetGetMethod().IsStatic)
				{
					continue;
				}

				var browsableAttr = property.FindAttribute<BrowsableAttribute>();
				if (browsableAttr != null && !browsableAttr.Browsable)
				{
					continue;
				}

				var hiddenAttr = property.FindAttribute<HiddenInEditorAttribute>();
				if (hiddenAttr != null)
				{
					continue;
				}

				var subValue = property.GetValue(value);

				sb.Append("\n\t\t\t\t" + property.Name);
				sb.Append(" = ");
				sb.Append(BuildValue(subValue));
				sb.Append(",");
			}

			sb.Append("\n\t\t\t}");

			return sb.ToString();
		}

		public void Export()
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

			ExportMain();
			ExportDesigner();
		}
	}
}
