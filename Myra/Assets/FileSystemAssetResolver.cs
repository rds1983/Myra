using System.IO;
using Microsoft.Xna.Framework;

namespace Myra.Assets
{
	public class FileSystemAssetResolver: IAssetResolver
	{
		public string BaseFolder { get; set; }

		public FileSystemAssetResolver(string baseFolder)
		{
			BaseFolder = baseFolder;
		}

		public Stream Open(string assetName)
		{
			if (!string.IsNullOrEmpty(BaseFolder))
			{
				assetName = Path.Combine(BaseFolder, assetName);
			}

			return TitleContainer.OpenStream(assetName);
		}
	}
}
