using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using BitmapFontExtensions = Myra.Graphics2D.Text.BitmapFontExtensions;

namespace Myra.Assets
{
	public class BitmapFontLoader : IAssetLoader<BitmapFont>
	{
		public BitmapFont Load(AssetManager assetManager, string assetName)
		{
			var text = assetManager.ReadAsText(assetName);
			var result = BitmapFontExtensions.LoadFromFnt(assetName, text, assetManager.Load<TextureRegion2D>);

			return result;
		}
	}
}