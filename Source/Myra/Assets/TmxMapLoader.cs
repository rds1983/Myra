using Myra.Graphics2D;
using Myra.Graphics2D.Tiled;

namespace Myra.Assets
{
	public class TmxMapLoader: IAssetLoader<TmxMap>
	{
		public TmxMap Load(AssetLoaderContext context, string assetName)
		{
			TmxMap result;
			using (var stream = context.Open(assetName))
			{
				result = new TmxMap(stream, context.Load<TmxTileset>, context.Load<RawImage>);
			}

			return result;
		}
	}
}
