using System.IO;

namespace Myra.Assets
{
	public class FileAssetResolver : IAssetResolver
	{
		public string BaseFolder { get; set; }

		public FileAssetResolver()
		{
			BaseFolder = string.Empty;
		}

		public FileAssetResolver(string baseFolder)
		{
			BaseFolder = baseFolder;
		}

		public Stream Open(string assetName)
		{
			if (!Path.IsPathRooted(assetName) && !string.IsNullOrEmpty(BaseFolder))
			{
				assetName = Path.Combine(BaseFolder, assetName);
			}

			return File.OpenRead(assetName);
		}
	}
}
