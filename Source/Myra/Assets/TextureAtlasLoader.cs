using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using TextureAtlasExtensions = Myra.Graphics2D.TextureAtlasExtensions;

namespace Myra.Assets
{
	public class TextureAtlasLoader : IAssetLoader<TextureAtlas>
	{
		public TextureAtlas Load(AssetLoaderContext context, string assetName)
		{
			var text = context.ReadAsText(assetName);

			return TextureAtlasExtensions.LoadGDX(assetName, text, context.Load<Texture2D>);
		}
	}
}