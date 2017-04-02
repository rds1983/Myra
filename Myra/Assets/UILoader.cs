using Myra.Graphics2D.UI;
using Myra.Utility;

namespace Myra.Assets
{
	public class UILoader: IAssetLoader<Project>
	{
		public Project Load(AssetManager assetManager, string path)
		{
			var text = assetManager.ReadAsText(path);

			var result = Serialization.LoadFromData(text);
			return result;
		}
	}
}
