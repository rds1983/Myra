using Myra.Graphics2D.UI;
using Myra.Utility;

namespace Myra.Assets
{
	public class UILoader: IAssetLoader<Project>
	{
		public Project Load(AssetManager assetManager, string assetName)
		{
			var text = assetManager.ReadAsText(assetName);

			var result = Serialization.LoadFromData(text);
			return result;
		}
	}
}
