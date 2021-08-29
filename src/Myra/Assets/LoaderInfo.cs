namespace Myra.Assets
{
	public class LoaderInfo
	{
		public object Loader { get; private set; }
		public bool StoreInCache { get; set; }

		public LoaderInfo(object loader, bool storeInCache = true)
		{
			Loader = loader;
			StoreInCache = storeInCache;
		}
	}
}
