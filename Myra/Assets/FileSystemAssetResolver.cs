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

		public Stream Open(string path)
		{
			if (!string.IsNullOrEmpty(BaseFolder))
			{
				path = Path.Combine(BaseFolder, path);
			}

			return TitleContainer.OpenStream(path);
		}
	}
}
