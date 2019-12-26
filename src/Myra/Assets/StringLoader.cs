namespace Myra.Assets
{
	internal class StringLoader: IAssetLoader<string>
	{
		public string Load(AssetLoaderContext context, string assetName)
		{
			return context.ReadAsText(assetName);
		}
	}
}
