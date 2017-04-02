using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;

namespace Myra.Assets
{
	public class SpritesheetLoader : IAssetLoader<SpriteSheet>
	{
		public SpriteSheet Load(AssetManager assetManager, string path)
		{
			var text = assetManager.ReadAsText(path);

			return SpriteSheet.LoadGDX(text, s => assetManager.Load<Texture2D>(s));
		}
	}
}