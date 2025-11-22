#if PLATFORM_AGNOSTIC

using FontStashSharp;
using Myra;
using StbImageSharp;
using System.Drawing;
using System.IO;

using Texture2D = System.Object;

namespace AssetManagementBase
{
	partial class MyraAssetManagerExtensions
	{
		internal class Texture2DWrapper
		{
			public int Width { get; private set; }
			public int Height { get; private set; }
			public object Texture { get; private set; }

			public Texture2DWrapper(int width, int height, object texture)
			{
				Width = width;
				Height = height;
				Texture = texture;
			}
		}

		private static AssetLoader<Texture2DWrapper> _textureLoader = (manager, assetName, settings, tag) =>
		{
			ImageResult result = null;
			using (var stream = manager.Open(assetName))
			{
				if (stream.CanSeek)
				{
					result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
				}
				else
				{
					// If stream doesnt provide seek functionaly, use MemoryStream instead
					using (var ms = new MemoryStream())
					{
						stream.CopyTo(ms);
						ms.Seek(0, SeekOrigin.Begin);
						result = ImageResult.FromStream(ms, ColorComponents.RedGreenBlueAlpha);
					}
				}
			}

			// Premultiply Alpha
			var b = result.Data;
			for (var i = 0; i < result.Data.Length; i += 4)
			{
				var falpha = b[i + 3] / 255.0f;
				b[i] = (byte)(b[i] * falpha);
				b[i + 1] = (byte)(b[i + 1] * falpha);
				b[i + 2] = (byte)(b[i + 2] * falpha);
			}

			var textureManager = MyraEnvironment.Platform.Renderer.TextureManager;
			var texture = textureManager.CreateTexture(result.Width, result.Height);
			textureManager.SetTextureData(texture, new Rectangle(0, 0, result.Width, result.Height), result.Data);
			return new Texture2DWrapper(result.Width, result.Height, texture);
		};

		private class FontSystemLoadingSettings : IAssetSettings
		{
			public Texture2D ExistingTexture { get; set; }
			public Rectangle ExistingTextureUsedSpace { get; set; }
			public string[] AdditionalFonts { get; set; }

			public string BuildKey() => string.Empty;
		}

		private static AssetLoader<FontSystem> _fontSystemLoader = (manager, assetName, settings, tag) =>
		{
			var fontSystemSettings = new FontSystemSettings();

			var fontSystemLoadingSettings = (FontSystemLoadingSettings)settings;
			if (fontSystemLoadingSettings != null)
			{
				fontSystemSettings.ExistingTexture = fontSystemLoadingSettings.ExistingTexture;
				fontSystemSettings.ExistingTextureUsedSpace = fontSystemLoadingSettings.ExistingTextureUsedSpace;
			}
			;

			var fontSystem = new FontSystem(fontSystemSettings);
			var data = manager.ReadAsByteArray(assetName);
			fontSystem.AddFont(data);
			if (fontSystemLoadingSettings != null && fontSystemLoadingSettings.AdditionalFonts != null)
			{
				foreach (var file in fontSystemLoadingSettings.AdditionalFonts)
				{
					data = manager.ReadAsByteArray(file);
					fontSystem.AddFont(data);
				}
			}

			return fontSystem;
		};

		internal static Texture2DWrapper LoadTexture2D(this AssetManager assetManager, string assetName) =>
				assetManager.UseLoader(_textureLoader, assetName);

		public static FontSystem LoadFontSystem(this AssetManager assetManager, string assetName, string[] additionalFonts = null, Texture2D existingTexture = null, Rectangle existingTextureUsedSpace = default(Rectangle))
		{
			FontSystemLoadingSettings settings = null;
			if (additionalFonts != null || existingTexture != null)
			{
				settings = new FontSystemLoadingSettings
				{
					AdditionalFonts = additionalFonts,
					ExistingTexture = existingTexture,
					ExistingTextureUsedSpace = existingTextureUsedSpace
				};
			}

			return assetManager.UseLoader(_fontSystemLoader, assetName, settings);
		}
	}
}

#endif
