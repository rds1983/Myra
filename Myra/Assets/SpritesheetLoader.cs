using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;

namespace Myra.Assets
{
	public class SpritesheetLoader : IAssetLoader<SpriteSheet>
	{
		public SpriteSheet Load(AssetManager assetManager, string assetName)
		{
			var text = assetManager.ReadAsText(assetName);

			return SpriteSheet.LoadGDX(text, assetManager.Load<Texture2D>);
		}
	}
}