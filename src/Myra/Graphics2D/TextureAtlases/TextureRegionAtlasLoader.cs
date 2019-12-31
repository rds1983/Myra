using Myra.Assets;

#if !XENKO
using Microsoft.Xna.Framework.Graphics;
#else
using Texture2D = Xenko.Graphics.Texture;
#endif

namespace Myra.Graphics2D.TextureAtlases
{
	internal class TextureRegionAtlasLoader : IAssetLoader<TextureRegionAtlas>
	{
		public TextureRegionAtlas Load(AssetLoaderContext context, string assetName)
		{
			var xml = context.Load<string>(assetName);
			return TextureRegionAtlas.FromXml(xml, name => context.Load<Texture2D>(name));
		}
	}
}