using Myra.Graphics2D;
using Myra.Graphics2D.Text;

namespace Myra.Assets
{
	public class BitmapFontLoader : IAssetLoader<BitmapFont>
	{
		public BitmapFont Load(AssetManager assetManager, string assetName)
		{
			var text = assetManager.ReadAsText(assetName);
			var result = BitmapFont.LoadFromFnt(text, s => assetManager.Load<TextureRegion>(s));

			return result;
		}
	}
}