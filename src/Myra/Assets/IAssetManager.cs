namespace Myra.Assets
{
	public interface IAssetManager
	{
		T Load<T>(string assetName);
	}
}
