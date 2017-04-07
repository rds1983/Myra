using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using BitmapFontExtensions = Myra.Graphics2D.Text.BitmapFontExtensions;

namespace Myra.Assets
{
	public class BitmapFontLoader : IAssetLoader<BitmapFont>
	{
		public BitmapFont Load(AssetLoaderContext context, string assetName)
		{
			var text = context.ReadAsText(assetName);
			var result = BitmapFontExtensions.LoadFromFnt(assetName, text, context.Load<TextureRegion2D>);

			return result;
		}
	}
}