using FontStashSharp;
using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
using Myra.Platform;
using Texture2D = System.Object;
#endif

namespace Myra.Assets
{
	internal class FontSystemLoader : IAssetLoader<FontSystem>
	{
		public static Texture2D ExistingTexture;
		public static Rectangle ExistingTextureUsedSpace;

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
				EffectAmount = amount,
				Effect =
					fontType switch
					{
						FontType.Regular => FontSystemEffect.None,
						FontType.Blurry => FontSystemEffect.Blurry,
						FontType.Stroked => FontSystemEffect.Stroked,
						_ => throw new Exception(),
					},
				ExistingTexture = ExistingTexture,
				ExistingTextureUsedSpace = ExistingTextureUsedSpace,
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