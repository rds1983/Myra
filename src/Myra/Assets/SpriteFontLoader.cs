using Myra.Graphics2D.TextureAtlases;
using SpriteFontPlus;

#if !XENKO
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Graphics;
#endif

namespace Myra.Assets
{
	internal class SpriteFontLoader : IAssetLoader<SpriteFont>
	{
		public SpriteFont Load(AssetLoaderContext context, string assetName)
		{
			var fontData = context.Load<string>(assetName);

			return BMFontLoader.Load(fontData, name => TextureGetter(context, name));
		}

		private TextureWithOffset TextureGetter(AssetLoaderContext context, string name)
		{
			var textureRegion = context.Load<TextureRegion>(name);
			return new TextureWithOffset(textureRegion.Texture, textureRegion.Bounds.Location);
		}
	}
}