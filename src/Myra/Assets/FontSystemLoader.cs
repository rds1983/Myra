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

			var fontSystemSettings = new FontSystemSettings
			{
				TextureWidth = MyraEnvironment.FontAtlasSize,
				TextureHeight = MyraEnvironment.FontAtlasSize,
				EffectAmount = amount,
				KernelWidth = MyraEnvironment.FontKernelWidth,
				KernelHeight = MyraEnvironment.FontKernelHeight,
				PremultiplyAlpha = MyraEnvironment.FontPremultiplyAlpha,
				FontResolutionFactor = MyraEnvironment.FontResolutionFactor,
				Effect =
					fontType switch
					{
						FontType.Regular => FontSystemEffect.None,
						FontType.Blurry => FontSystemEffect.Blurry,
						FontType.Stroked => FontSystemEffect.Stroked,
						_ => throw new Exception(),
					}
			};

			var fontSystem = new FontSystem(fontSystemSettings);

			var files = parts[0].Split('|');
			foreach (var file in files)
			{
				var data = context.Load<byte[]>(file);
				fontSystem.AddFont(data);
			}

			return fontSystem;
		}
	}
}