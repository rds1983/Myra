namespace Myra.Assets
{
	public interface IAssetLoader<out T>
	{
		T Load(AssetManager assetManager, string path);
	}
}
