using System;
using System.IO;

namespace Myra.Assets
{
	public class FileAssetResolver : IAssetResolver
	{
		public string BaseFolder { get; set; }

		public FileAssetResolver(string baseFolder)
		{
			BaseFolder = baseFolder;
		}

		public Stream Open(string assetName)
		{
			if (AssetManager.SeparatorSymbol != Path.DirectorySeparatorChar)
			{
				assetName = assetName.Replace(AssetManager.SeparatorSymbol, Path.DirectorySeparatorChar);
			}

			if (!Path.IsPathRooted(assetName) && !string.IsNullOrEmpty(BaseFolder))
			{
				assetName = Path.Combine(BaseFolder, assetName);
			}

			if (!File.Exists(assetName))
			{
				throw new Exception($"Could not find file '{assetName}'");
			}

			return File.OpenRead(assetName);
		}
	}
}
