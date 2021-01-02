using XNAssets;
using FontStashSharp.Interfaces;
using Myra.Graphics2D.TextureAtlases;

namespace Myra.Assets
{
	internal class TextureRegionAtlasLoader : IAssetLoader<TextureRegionAtlas>
	{
		public TextureRegionAtlas Load(AssetLoaderContext context, string assetName)
		{
			var data = context.Load<string>(assetName);
			return TextureRegionAtlas.Load(data, name => context.Load<Texture2DWrapper>(name).Texture);
		}
	}
}