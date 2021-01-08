using AssetManagementBase;
using FontStashSharp;
using System;

#if PLATFORM_AGNOSTIC
using Myra.Platform;
#endif

namespace Myra.Assets
{
	internal class FontSystemLoader : IAssetLoader<FontSystem>
	{
		private enum FontType
		{
			Regular,
			Blurry,
			Stroked
		}

		public FontSystem Load(AssetLoaderContext context, string assetName)
		{
			var parts = assetName.Split(':');

			var fontType = FontType.Regular;
			var amount = 1;
			if (parts.Length > 1)
			{
				fontType = (FontType)Enum.Parse(typeof(FontType), parts[1]);

				if (fontType != FontType.Regular)
				{
					if (parts.Length < 3)
					{
						throw new Exception("Missing amount");
					}

					amount = int.Parse(parts[2]);
				}
			}

			FontSystem fontSystem = null;

#if MONOGAME || FNA || STRIDE
			switch (fontType)
			{
				case FontType.Regular:
					fontSystem = FontSystemFactory.Create(MyraEnvironment.GraphicsDevice, MyraEnvironment.FontAtlasSize, MyraEnvironment.FontAtlasSize);
					break;
				case FontType.Blurry:
					fontSystem = FontSystemFactory.CreateBlurry(MyraEnvironment.GraphicsDevice, amount, MyraEnvironment.FontAtlasSize, MyraEnvironment.FontAtlasSize);
					break;
				case FontType.Stroked:
					fontSystem = FontSystemFactory.CreateStroked(MyraEnvironment.GraphicsDevice, amount, MyraEnvironment.FontAtlasSize, MyraEnvironment.FontAtlasSize);
					break;
			}
#else
			switch (fontType)
			{
				case FontType.Regular:
					fontSystem = FontSystemFactory.Create(MyraEnvironment.FontAtlasSize, MyraEnvironment.FontAtlasSize);
					break;
				case FontType.Blurry:
					fontSystem = FontSystemFactory.CreateBlurry(amount, MyraEnvironment.FontAtlasSize, MyraEnvironment.FontAtlasSize);
					break;
				case FontType.Stroked:
					fontSystem = FontSystemFactory.CreateStroked(amount, MyraEnvironment.FontAtlasSize, MyraEnvironment.FontAtlasSize);
					break;
			}
#endif

			var data = context.Load<byte[]>(parts[0]);
			fontSystem.AddFont(data);

			return fontSystem;
		}
	}
}