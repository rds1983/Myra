using Microsoft.Xna.Framework.Graphics;
using Myra.Assets;

namespace Myra.Graphics2D.TextureAtlases
{
	public class TextureRegionAtlasLoader : IAssetLoader<TextureRegionAtlas>
	{
		public TextureRegionAtlas Load(AssetLoaderContext context, string assetName)
		{
			var xml = context.Load<string>(assetName);
			return TextureRegionAtlas.FromXml(xml, name => context.Load<Texture2D>(name));
		}
	}
}