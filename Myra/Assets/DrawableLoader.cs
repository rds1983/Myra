using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;

namespace Myra.Assets
{
	public class DrawableLoader : IAssetLoader<TextureRegion>
	{
		public TextureRegion Load(AssetManager assetManager, string assetName)
		{
			if (assetName.Contains(":"))
			{
				// Means page is sprite in the spritesheet
				var parts = assetName.Split(':');

				var spriteSheet = assetManager.Load<SpriteSheet>(parts[0]);
				var drawable = spriteSheet.EnsureDrawable(parts[1]);

				return drawable.TextureRegion;
			}

			return new TextureRegion(assetManager.Load<Texture2D>(assetName));
		}
	}
}