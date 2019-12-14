using System.IO;
using Microsoft.Xna.Framework;

namespace Myra.Assets
{
	public class TitleContainerAssetResolver: IAssetResolver
	{
		public string BaseFolder { get; set; }

		public TitleContainerAssetResolver()
		{
			BaseFolder = string.Empty;
		}

		public TitleContainerAssetResolver(string baseFolder)
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
