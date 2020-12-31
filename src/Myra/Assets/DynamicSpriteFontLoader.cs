using XNAssets;
using FontStashSharp;
using System;
using System.Collections.Generic;

namespace Myra.Assets
{
	internal class DynamicSpriteFontLoader : IAssetLoader<DynamicSpriteFont>
	{
		public DynamicSpriteFont Load(AssetLoaderContext context, string assetName)
		{
			var parts = assetName.Split(':');
			if (parts.Length < 2)
			{
				throw new Exception("Missing font size");
			}

			var fontSize = int.Parse(parts[parts.Length - 1]);

			var partsWithoutSize = new List<string>();
			for(var i = 0; i < parts.Length - 1; ++i)
			{
				partsWithoutSize.Add(parts[i]);
			}

			var fontSystem = context.Load<FontSystem>(string.Join(":", partsWithoutSize));

			return fontSystem.GetFont(fontSize);
		}
	}
}