using AssetManagementBase;
using System.Reflection;

namespace MyraPad
{
	public static class Resources
	{
		private static string _exportCsDesigner, _exportCsLight, _exportCsMain, _newProjectTemplate;

		public static string ExportCSDesigner
		{
			get
			{
				if (string.IsNullOrEmpty(_exportCsDesigner))
				{
					var assetManager = CreateAssetManager();
					_exportCsDesigner = assetManager.ReadAsString("ExportCSDesigner.cstemplate");
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
					var assetManager = CreateAssetManager();
					_exportCsLight = assetManager.ReadAsString("ExportCSLight.cstemplate");
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
					var assetManager = CreateAssetManager();
					_exportCsMain = assetManager.ReadAsString("ExportCSMain.cstemplate");
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
					var assetManager = CreateAssetManager();
					_newProjectTemplate = assetManager.ReadAsString("NewProject.xmmptemplate");
				}

				return _newProjectTemplate;
			}
		}

		private static AssetManager CreateAssetManager() => AssetManager.CreateResourceAssetManager(typeof(Resources).Assembly, "MyraPad.Resources", false);
	}
}