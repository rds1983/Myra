using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Assets
{
	public class TextureRegion2DLoader : IAssetLoader<TextureRegion2D>
	{
		public TextureRegion2D Load(AssetManager assetManager, string assetName)
		{
			if (assetName.Contains(":"))
			{
				// Means page is sprite in the spritesheet
				var parts = assetName.Split(':');

				var spriteSheet = assetManager.Load<TextureAtlas>(parts[0]);


				var image = spriteSheet.GetRegion(parts[1]);

				return image;
			}

			return new TextureRegion2D(assetManager.Load<Texture2D>(assetName));
		}
	}
}