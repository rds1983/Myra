using AssetManagementBase;
using FontStashSharp;
using System;

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
					fontSystem = FontSystemFactory.Create(MyraEnvironment.GraphicsDevice);
					break;
				case FontType.Blurry:
					fontSystem = FontSystemFactory.CreateBlurry(MyraEnvironment.GraphicsDevice, amount);
					break;
				case FontType.Stroked:
					fontSystem = FontSystemFactory.CreateStroked(MyraEnvironment.GraphicsDevice, amount);
					break;
			}
#else
			switch (fontType)
			{
				case FontType.Regular:
					fontSystem = new FontSystem(MyraEnvironment.Platform, 1024, 1024);
					break;
				case FontType.Blurry:
					fontSystem = new FontSystem(MyraEnvironment.Platform, 1024, 1024, amount);
					break;
				case FontType.Stroked:
					fontSystem = new FontSystem(MyraEnvironment.Platform, 1024, 1024, 0, amount);
					break;
			}
#endif

			var data = context.Load<byte[]>(parts[0]);
			fontSystem.AddFont(data);

			return fontSystem;
		}
	}
}