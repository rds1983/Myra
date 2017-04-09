using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Tiled;
using Myra.Graphics2D.Tiles;

namespace Myra.Assets
{
	public class TileModelLoader: IAssetLoader<TileMap>
	{
		public TileMap Load(AssetLoaderContext context, string assetName)
		{
			TmxMap result;
			using (var stream = context.Open(assetName))
			{
				result = new TmxMap(stream, context.Load<TmxTileset>, context.Load<Texture2D>);
			}

			return result.CreateTileModel();
		}
	}
}
