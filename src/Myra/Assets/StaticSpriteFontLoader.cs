using Myra.Graphics2D.TextureAtlases;
using FontStashSharp;
using TextureWithOffset = FontStashSharp.TextureWithOffset;

namespace Myra.Assets
{
	internal class StaticSpriteFontLoader : IAssetLoader<StaticSpriteFont>
	{
		public StaticSpriteFont Load(AssetLoaderContext context, string assetName)
		{
			var fontData = context.Load<string>(assetName);

			return StaticSpriteFont.FromBMFont(fontData,
						name => TextureGetter(context, name));
		}

		private TextureWithOffset TextureGetter(AssetLoaderContext context, string name)
		{
			var textureRegion = context.Load<TextureRegion>(name);
			return new TextureWithOffset(textureRegion.Texture, textureRegion.Bounds.Location);
		}
	}
}