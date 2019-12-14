using Microsoft.Xna.Framework.Graphics;
using Myra.Assets;

namespace Myra.Graphics2D.TextureAtlases
{
	public class TextureRegionLoader : IAssetLoader<TextureRegion>
	{
		public TextureRegion Load(AssetLoaderContext context, string assetName)
		{
			if (assetName.Contains(":"))
			{
				// First part is texture region atlas name
				// Second part is texture region name
				var parts = assetName.Split(':');
				var textureRegionAtlas = context.AssetManager.Load<TextureRegionAtlas>(parts[0]);
				return textureRegionAtlas[parts[1]];
			}

			// Ordinary texture
			var texture = context.AssetManager.Load<Texture2D>(assetName);
			return new TextureRegion(texture);
		}
	}
}