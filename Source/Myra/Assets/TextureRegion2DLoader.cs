using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Assets
{
	public class TextureRegion2DLoader : IAssetLoader<TextureRegion2D>
	{
		public TextureRegion2D Load(AssetLoaderContext context, string assetName)
		{
			if (assetName.Contains(":"))
			{
				// Means page is sprite in the spritesheet
				var parts = assetName.Split(':');

				var spriteSheet = context.Load<TextureAtlas>(parts[0]);

				var image = spriteSheet.GetRegion(parts[1]);

				return image;
			}

			return new TextureRegion2D(context.Load<Texture2D>(assetName));
		}
	}
}