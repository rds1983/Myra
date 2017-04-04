using System;

namespace Myra.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AssetLoaderAttribute: Attribute
	{
		public Type AssetLoaderType { get; private set; }

		public AssetLoaderAttribute(Type assetLoaderType)
		{
			if (assetLoaderType == null)
			{
				throw new ArgumentNullException("assetLoaderType");
			}

			AssetLoaderType = assetLoaderType;
		}
	}
}
