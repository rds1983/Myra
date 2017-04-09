using Myra.Graphics2D;

namespace Myra.Assets
{
	public class RawImageLoader: IAssetLoader<RawImage>
	{
		public RawImage Load(AssetLoaderContext context, string assetName)
		{
			RawImage image;
			using (var stream = context.Open(assetName))
			{
				image = RawImage.FromStream(stream);
			}

			return image;
		}
	}
}
