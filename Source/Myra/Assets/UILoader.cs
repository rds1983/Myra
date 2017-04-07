using Myra.Graphics2D.UI;
using Myra.Utility;

namespace Myra.Assets
{
	public class UILoader: IAssetLoader<Project>
	{
		public Project Load(AssetLoaderContext context, string assetName)
		{
			var text = context.ReadAsText(assetName);

			var result = Serialization.LoadFromData(text);
			return result;
		}
	}
}
