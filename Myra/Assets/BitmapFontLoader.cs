using Myra.Graphics2D;
using Myra.Graphics2D.Text;

namespace Myra.Assets
{
	public class BitmapFontLoader : IAssetLoader<BitmapFont>
	{
		public BitmapFont Load(AssetManager assetManager, string path)
		{
			var text = assetManager.ReadAsText(path);
			var result = BitmapFont.LoadFromFnt(text, s => assetManager.Load<TextureRegion>(s));

			return result;
		}
	}
}