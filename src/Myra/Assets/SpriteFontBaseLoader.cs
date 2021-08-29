using FontStashSharp;
using System;

namespace Myra.Assets
{
	internal class SpriteFontBaseLoader : IAssetLoader<SpriteFontBase>
	{
		public SpriteFontBase Load(AssetLoaderContext context, string assetName)
		{
			if (assetName.Contains(".fnt"))
			{
				return context.Load<StaticSpriteFont>(assetName);
			} else if (assetName.Contains(".ttf"))
			{
				return context.Load<DynamicSpriteFont>(assetName);
			}

			throw new Exception(string.Format("Can't load font '{0}'", assetName));
		}
	}
}