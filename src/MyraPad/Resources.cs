using Myra.Utility;

namespace MyraPad
{
	public static class Resources
	{
		private static readonly ResourceAssetResolver _assetResolver = new ResourceAssetResolver(typeof(Resources).Assembly, "MyraPad.Resources.");

		private static string _exportCsDesigner, _exportCsMain;

		public static string ExportCSDesigner
		{
			get
			{
				if (string.IsNullOrEmpty(_exportCsDesigner))
				{
					_exportCsDesigner = _assetResolver.ReadAsString("ExportCSDesigner.cstemplate");
				}

				return _exportCsDesigner;
			}
		}

		public static string ExportCSMain
		{
			get
			{
				if (string.IsNullOrEmpty(_exportCsMain))
				{
					_exportCsMain = _assetResolver.ReadAsString("ExportCSMain.cstemplate");
				}

				return _exportCsMain;
			}
		}
	}
}
