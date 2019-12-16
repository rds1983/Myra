using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using SpriteFontPlus;

namespace Myra.Assets
{
	public class SpriteFontLoader : IAssetLoader<SpriteFont>
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