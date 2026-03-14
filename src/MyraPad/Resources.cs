using System.Reflection;
using Myra.Utility;

namespace MyraPad
{
	public static class Resources
	{
		private static string _exportCsDesigner, _exportCsLight, _exportCsMain, _newProjectTemplate;

		private static Assembly Assembly
		{
			get
			{
				return typeof(Resources).Assembly;
			}
		}

		public static string ExportCSDesigner
		{
			get
			{
				if (string.IsNullOrEmpty(_exportCsDesigner))
				{
					_exportCsDesigner = Assembly.ReadResourceAsString("MyraPad.Resources.ExportCSDesigner.cstemplate");
				}

				return _exportCsDesigner;
			}
		}

		public static string ExportCSLight
		{
			get
			{
				if (string.IsNullOrEmpty(_exportCsLight))
				{
					_exportCsLight = Assembly.ReadResourceAsString("MyraPad.Resources.ExportCSLight.cstemplate");
				}

				return _exportCsLight;
			}
		}

		public static string ExportCSMain
		{
			get
			{
				if (string.IsNullOrEmpty(_exportCsMain))
				{
					_exportCsMain = Assembly.ReadResourceAsString("MyraPad.Resources.ExportCSMain.cstemplate");
				}

				return _exportCsMain;
			}
		}

		public static string NewProjectTemplate
		{
			get
			{
				if (string.IsNullOrEmpty(_newProjectTemplate))
				{
					_newProjectTemplate = Assembly.ReadResourceAsString("MyraPad.Resources.NewProject.xmmptemplate");
				}

				return _newProjectTemplate;
			}
		}
	}
}