using Microsoft.Xna.Framework.Graphics;
using Myra.Assets;

namespace Myra.Graphics2D.TextureAtlases
{
	public class TextureRegionAtlasLoader : IAssetLoader<TextureRegionAtlas>
	{
		public TextureRegionAtlas Load(AssetLoaderContext context, string assetName)
		{
			var xml = context.AssetManager.Load<string>(assetName);
			return TextureRegionAtlas.FromXml(xml, name => context.AssetManager.Load<Texture2D>(name));
		}
	}
}