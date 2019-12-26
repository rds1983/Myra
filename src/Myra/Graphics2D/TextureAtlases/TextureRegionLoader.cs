using Microsoft.Xna.Framework.Graphics;
using Myra.Assets;

namespace Myra.Graphics2D.TextureAtlases
{
	internal class TextureRegionLoader : IAssetLoader<TextureRegion>
	{
		public TextureRegion Load(AssetLoaderContext context, string assetName)
		{
			if (assetName.Contains(":"))
			{
				// First part is texture region atlas name
				// Second part is texture region name
				var parts = assetName.Split(':');
				var textureRegionAtlas = context.Load<TextureRegionAtlas>(parts[0]);
				return textureRegionAtlas[parts[1]];
			}

			// Ordinary texture
			var texture = context.Load<Texture2D>(assetName);
			return new TextureRegion(texture);
		}
	}
}