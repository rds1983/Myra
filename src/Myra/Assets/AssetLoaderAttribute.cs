using System;

namespace Myra.Assets
{
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
	public class AssetLoaderAttribute : Attribute
	{
		public Type AssetLoaderType { get; private set; }
		public bool StoreInCache { get; private set; }

		public AssetLoaderAttribute(Type assetLoaderType, bool storeInCache = true)
		{
			AssetLoaderType = assetLoaderType ?? throw new ArgumentNullException(nameof(assetLoaderType));
			StoreInCache = storeInCache;
		}
	}
}