using System.Collections.Generic;
using System.Xml.Linq;
using System;
using FontStashSharp;
using Myra;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using Myra.MML;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
using SolidBrush = Myra.Graphics2D.Brushes.SolidBrush;
using Color = FontStashSharp.FSColor;
using Texture2D = System.Object;
using StbImageSharp;
using System.IO;
#endif

namespace AssetManagementBase
{
	public static class MyraAssetManagerExtensions
	{
#if PLATFORM_AGNOSTIC
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

		internal static Texture2DWrapper LoadTexture2D(this AssetManager assetManager, string assetName) =>
				assetManager.UseLoader(_textureLoader, assetName);
#endif

		private class FontSystemLoadingSettings : IAssetSettings
		{
			public Texture2D ExistingTexture { get; set; }
			public Rectangle ExistingTextureUsedSpace { get; set; }
			public string[] AdditionalFonts { get; set; }

			public string BuildKey() => string.Empty;
		}

		private static AssetLoader<TextureRegionAtlas> _atlasLoader = (manager, assetName, settings, tag) =>
		{
			var data = manager.ReadAsString(assetName);

#if !PLATFORM_AGNOSTIC
			return TextureRegionAtlas.Load(data, name => manager.LoadTexture2D(MyraEnvironment.GraphicsDevice, name, true));
#else
			return TextureRegionAtlas.Load(data, name => manager.LoadTexture2D(name).Texture);
#endif
		};

		private static AssetLoader<FontSystem> _fontSystemLoader = (manager, assetName, settings, tag) =>
		{
			var fontSystemSettings = new FontSystemSettings();

			var fontSystemLoadingSettings = (FontSystemLoadingSettings)settings;
			if (fontSystemLoadingSettings != null)
			{
				fontSystemSettings.ExistingTexture = fontSystemLoadingSettings.ExistingTexture;
				fontSystemSettings.ExistingTextureUsedSpace = fontSystemLoadingSettings.ExistingTextureUsedSpace;
			};

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

		private static AssetLoader<StaticSpriteFont> _staticFontLoader = (manager, assetName, settings, tag) =>
		{
			var fontData = manager.ReadAsString(assetName);

			return StaticSpriteFont.FromBMFont(fontData,
						name =>
						{
							var region = LoadTextureRegion(manager, name);
							return new TextureWithOffset(region.Texture, region.Bounds.Location);
						});
		};

		private static AssetLoader<Stylesheet> _stylesheetLoader = (manager, assetName, settings, tag) =>
		{
			var xml = manager.ReadAsString(assetName);

			var xDoc = XDocument.Parse(xml);
			var attr = xDoc.Root.Attribute("TextureRegionAtlas");
			if (attr == null)
			{
				throw new Exception("Mandatory attribute 'TextureRegionAtlas' doesnt exist");
			}

			var textureRegionAtlas = manager.LoadTextureRegionAtlas(attr.Value);

			// Load fonts
			var fonts = new Dictionary<string, SpriteFontBase>();
			var fontsNode = xDoc.Root.Element("Fonts");

			var usedSpaceAttr = fontsNode.Attribute("UsedSpace");
			Texture2D existingTexture = null;
			var existingTextureUsedSpace = Rectangle.Empty;
			if (usedSpaceAttr != null)
			{
				var usedSpace = usedSpaceAttr.Value.ParseRectangle();

				existingTexture = textureRegionAtlas.Texture;
				existingTextureUsedSpace = usedSpace;
			}

			foreach (var el in fontsNode.Elements())
			{
				SpriteFontBase font = null;

				var fontFile = el.Attribute("File").Value;
				if (fontFile.EndsWith(".ttf") || fontFile.EndsWith(".otf"))
				{
					var parts = new List<string>()
						{
							fontFile
						};

					var typeAttribute = el.Attribute("Effect");
					if (typeAttribute != null)
					{
						parts.Add(typeAttribute.Value);

						var amountAttribute = el.Attribute("Amount");
						parts.Add(amountAttribute.Value);
					}

					if (el.Attribute("Size") == null)
					{
						throw new Exception($"Can't load stylesheet ttf font '{fontFile}', since Size isn't specified.");
					}

					parts.Add(el.Attribute("Size").Value);
					var fontSystem = manager.LoadFontSystem(fontFile, existingTexture: existingTexture, existingTextureUsedSpace: existingTextureUsedSpace);
					font = fontSystem.GetFont(float.Parse(el.Attribute("Size").Value));
				}
				else if (fontFile.EndsWith(".fnt"))
				{
					font = manager.LoadStaticSpriteFont(fontFile);
				}
				else
				{
					throw new Exception(string.Format("Font '{0}' isn't supported", fontFile));
				}

				fonts[el.Attribute(BaseContext.IdName).Value] = font;
			}

			return Stylesheet.LoadFromSource(xml, textureRegionAtlas, fonts);
		};

		public static TextureRegionAtlas LoadTextureRegionAtlas(this AssetManager assetManager, string assetName) => assetManager.UseLoader(_atlasLoader, assetName);

		/// <summary>
		/// Loads texture region by either image name(i.e. 'image.png') or atlas name/id(i.e. 'atlas.xmat:id')
		/// </summary>
		/// <param name="assetManager"></param>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public static TextureRegion LoadTextureRegion(this AssetManager assetManager, string assetName)
		{
			if (assetName.Contains(":"))
			{
				// First part is texture region atlas name
				// Second part is texture region name
				var parts = assetName.Split(':');
				var textureRegionAtlas = assetManager.LoadTextureRegionAtlas(parts[0]);
				return textureRegionAtlas[parts[1]];
			}

			// Ordinary texture
#if MONOGAME || FNA || STRIDE
			var texture = assetManager.LoadTexture2D(MyraEnvironment.GraphicsDevice, assetName);
			return new TextureRegion(texture, new Rectangle(0, 0, texture.Width, texture.Height));
#else
			var texture = assetManager.LoadTexture2D(assetName);
			return new TextureRegion(texture.Texture, new Rectangle(0, 0, texture.Width, texture.Height));
#endif
		}

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

		public static StaticSpriteFont LoadStaticSpriteFont(this AssetManager assetManager, string assetName) => assetManager.UseLoader(_staticFontLoader, assetName);

		/// <summary>
		/// Loads a font by either ttf name/size(i.e. 'font.ttf:32') or by fnt name(i.e. 'font.fnt')
		/// </summary>
		/// <param name="assetManager"></param>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public static SpriteFontBase LoadFont(this AssetManager assetManager, string assetName)
		{
			if (assetName.Contains(".fnt"))
			{
				return assetManager.LoadStaticSpriteFont(assetName);
			}
			else if (assetName.Contains(".ttf"))
			{

				var parts = assetName.Split(':');
				if (parts.Length < 2)
				{
					throw new Exception("Missing font size");
				}

				var fontSize = int.Parse(parts[1].Trim());
				var fontSystem = assetManager.LoadFontSystem(parts[0].Trim());

				return fontSystem.GetFont(fontSize);
			}

			throw new Exception(string.Format("Can't load font '{0}'", assetName));
		}

		public static Stylesheet LoadStylesheet(this AssetManager assetManager, string assetName) => assetManager.UseLoader(_stylesheetLoader, assetName);
	}
}
