using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using TextureAtlasExtensions = Myra.Graphics2D.TextureAtlasExtensions;

namespace Myra.Assets
{
	public class TextureAtlasLoader : IAssetLoader<TextureAtlas>
	{
		public TextureAtlas Load(AssetManager assetManager, string assetName)
		{
			var text = assetManager.ReadAsText(assetName);

			return TextureAtlasExtensions.LoadGDX(assetName, text, assetManager.Load<Texture2D>);
		}
	}
}