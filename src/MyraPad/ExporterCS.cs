using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Myra.MML;
using Myra.Utility;

namespace MyraPad
{
	public class ExporterCS : IDisposable
	{
		private readonly Project _project;
		private readonly Dictionary<string, int> ids = new Dictionary<string, int>();
		private readonly StringBuilder sbFields = new StringBuilder();
		private readonly StringBuilder sbBuild = new StringBuilder();
		private readonly PrimitiveConverter converter = new PrimitiveConverter();
		private bool isFirst = true;

		public ExporterCS(Project project)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}

			_project = project;
		}

		private string ExportPath
		{
			get
			{
				return Environment.ExpandEnvironmentVariables(_project.ExportOptions.OutputPath);
			}
		}

		public string ExportMain()
		{
			var path = Path.Combine(ExportPath, _project.ExportOptions.Class + ".cs");
			if (File.Exists(path))
			{
				// Do not overwrite main
				return string.Empty;
			}

			var template = string.IsNullOrWhiteSpace(_project.ExportOptions.TemplateMain) ?
				Resources.ExportCSMain :
				File.ReadAllText(_project.ExportOptions.TemplateMain);

			template = template.Replace("$namespace$", _project.ExportOptions.Namespace);
			template = template.Replace("$class$", _project.ExportOptions.Class);
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

		public string ExportDesignerRecursive(IItemWithId w, IItemWithId parent)
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
					styleName = (string)value;
					continue;
				}

				if (value == null)
				{
					simpleProperties.Add(property);
					continue;
				}

				if (value is IItemWithId)
				{
					var subItemId = ExportDesignerRecursive((IItemWithId)value, w);
					var subItemCode = string.Format("{0} = {1}", property.Name, subItemId);
					subItems.Add(subItemCode);
				}
				else
				{
					// Type should be assignable to IList<T> in order to be serialized as container
					var type = value.GetType();
					var listInterface = (from i in type.GetInterfaces()
										 where i.IsGenericType &&
										 i.GetGenericTypeDefinition() == typeof(IList<>)
										 select i).FirstOrDefault();

					if (listInterface != null &&
						typeof(IItemWithId).IsAssignableFrom(listInterface.GetGenericArguments()[0]))
					{
						var asEnumerable = value as IEnumerable;
						foreach (var comp in asEnumerable)
						{
							var subItemId = ExportDesignerRecursive((IItemWithId)comp, w);
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
				if (!_project.ShouldSerializeProperty(w, property))
				{
					continue;
				}

				var propertyCode = BuildPropertyCode(property, w, idPrefix);
				if (!string.IsNullOrEmpty(propertyCode))
				{
					sbBuild.Append(propertyCode);
				}
			}

			var asBaseObject = w as BaseObject;
			if (parent != null && asBaseObject != null)
			{
				var attachedProperties = AttachedPropertiesRegistry.GetPropertiesOfType(parent.GetType());
				foreach (var property in attachedProperties)
				{
					var value = property.GetValueObject(asBaseObject);
					if (value != null && !value.Equals(property.DefaultValueObject))
					{
						value = BuildValue(value, converter);
						sbBuild.Append($"\n\t\t\t{property.OwnerType.Name}.Set{property.Name}({id}, {value});");
					}
				}
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
			var template = string.IsNullOrWhiteSpace(_project.ExportOptions.TemplateDesigner) ?
				Resources.ExportCSDesigner :
				File.ReadAllText(_project.ExportOptions.TemplateDesigner);

			template = template.Replace("$namespace$", _project.ExportOptions.Namespace);
			template = template.Replace("$class$", _project.ExportOptions.Class);
			template = template.Replace("$parentClass$", _project.Root.GetType().Name);
			template = template.Replace("$generationDate$", DateTime.Now.ToString());

			ids.Clear();
			sbFields.Clear();
			sbBuild.Clear();

			isFirst = true;
			ExportDesignerRecursive(_project.Root, null);

			template = template.Replace("$fields$", sbFields.ToString());
			template = template.Replace("$build$", sbBuild.ToString());


			var path = Path.Combine(ExportPath, _project.ExportOptions.Class + ".Generated.cs");
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

				var jsonIgnoreAttr = property.FindAttribute<XmlIgnoreAttribute>();
				if (jsonIgnoreAttr != null)
				{
					continue;
				}

				var obsoleteAttr = property.FindAttribute<ObsoleteAttribute>();
				if (obsoleteAttr != null)
				{
					continue;
				}

				result.Add(property);
			}

			return result;
		}

		private string BuildPropertyCode(PropertyInfo property, object o, string idPrefix)
		{
			var sb = new StringBuilder();

			var value = property.GetValue(o);

			var asList = value as IList;
			if (asList == null)
			{
				string strValue = null;
				if (typeof(IBrush).IsAssignableFrom(property.PropertyType) ||
					property.PropertyType == typeof(SpriteFontBase))
				{
					var baseObject = o as BaseObject;
					string s;
					if (baseObject != null && baseObject.Resources.TryGetValue(property.Name, out s))
					{
						var typeName = property.PropertyType.Name;
						if (typeof(IImage).IsAssignableFrom(property.PropertyType))
						{
							typeName = "TextureRegion";
						}

						if (property.PropertyType != typeof(IBrush))
						{
							if (typeName == "SpriteFontBase")
							{
								typeName = "Font";
							}
							strValue = "MyraEnvironment.DefaultAssetManager.Load" + typeName + "(\"" + s + "\")";
						}
						else
						{
							strValue = "new SolidBrush(\"" + s + "\")";
						}
					}
				}
				else if (property.PropertyType == typeof(Thickness))
				{
					var thickness = (Thickness)value;
					strValue = "new Thickness(" + thickness.ToString() + ")";
				}
				else
				{
					strValue = BuildValue(value, converter);
				}

				if (strValue == null)
				{
					return null;
				}

				sb.Append("\n\t\t\t" + idPrefix + property.Name);
				sb.Append(" = ");
				sb.Append(strValue);
				sb.Append(";");
			}
			else
			{
				foreach (var comp in asList)
				{
					sb.Append("\n\t\t\t" + idPrefix + property.Name);
					sb.Append(".Add(");
					sb.Append(BuildValue(comp, converter));
					sb.Append(");");
				}
			}

			return sb.ToString();
		}

		private static string BuildValue(object value, PrimitiveConverter converter)
		{
			if (value == null)
			{
				return "null";
			}

			if (value is Color)
			{
				var name = ((Color)value).GetColorName();
				if (!string.IsNullOrEmpty(name))
				{
					return "Color." + name;
				}
				else
				{
					var c = (Color)value;

					return string.Format("ColorStorage.CreateColor({0}, {1}, {2}, {3})", (int)c.R, (int)c.G, (int)c.B, (int)c.A);
				}
			}

			if (value is string || value.GetType().IsPrimitive)
			{
				return converter.PrimitiveToLiteral(value);
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

				var jsonIgnoreAttribute = property.FindAttribute<XmlIgnoreAttribute>();
				if (jsonIgnoreAttribute != null)
				{
					continue;
				}

				var subValue = property.GetValue(value);

				if (property.HasDefaultValue(subValue))
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
				sb.Append(BuildValue(subValue, converter));
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

		public void Dispose() => converter.Dispose();
	}

	class PrimitiveConverter : IDisposable
	{
		private readonly CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

		public string PrimitiveToLiteral(object input)
		{
			using (var writer = new StringWriter())
			{
				provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
				var result = writer.ToString();

				// Remove trailing 'f' from float
				if (input is float && !string.IsNullOrEmpty(result))
				{
					result = result.ToLower();
					if (result.EndsWith("f"))
					{
						result = result.Substring(0, result.Length - 1);
					}
				}

				return result;
			}
		}

		public void Dispose() => provider.Dispose();
	}
}