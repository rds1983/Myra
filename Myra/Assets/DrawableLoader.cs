using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;

namespace Myra.Assets
{
	public class DrawableLoader : IAssetLoader<TextureRegion>
	{
		public TextureRegion Load(AssetManager assetManager, string fn)
		{
			if (fn.Contains(":"))
			{
				// Means page is sprite in the spritesheet
				var parts = fn.Split(':');

				var spriteSheet = assetManager.Load<SpriteSheet>(parts[0]);
				var drawable = spriteSheet.EnsureDrawable(parts[1]);

				return drawable.TextureRegion;
			}

			return new TextureRegion(assetManager.Load<Texture2D>(fn));
		}
	}
}