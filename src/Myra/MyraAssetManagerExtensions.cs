using System.Collections.Generic;
using System.Xml.Linq;
using System;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using Myra.MML;
using Myra.Utility;

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

	internal class Texture2DLoader : IAssetLoader<Texture2DWrapper>
	{
		public Texture2DWrapper Load(AssetLoaderContext context, string assetName)
		{
			ImageResult result = null;
			using (var stream = context.Open(assetName))
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
		}
	}
#endif

		private class FontSystemLoadingSettings
		{
			public Texture2D ExistingTexture { get; set; }
			public Rectangle ExistingTextureUsedSpace { get; set; }
			public string[] AdditionalFonts { get; set; }
		}

		private static AssetLoader<TextureRegionAtlas> _atlasLoader = (context) =>
		{
			var data = context.ReadDataAsString();
			return TextureRegionAtlas.Load(data, name => context.Manager.LoadTexture2D(MyraEnvironment.GraphicsDevice, name, true));
		};

		private static AssetLoader<FontSystem> _fontSystemLoader = (context) =>
		{
			var fontSystemSettings = new FontSystemSettings();

			var fontSystemLoadingSettings = (FontSystemLoadingSettings)context.Settings;
			if (fontSystemLoadingSettings != null)
			{
				fontSystemSettings.ExistingTexture = fontSystemLoadingSettings.ExistingTexture;
				fontSystemSettings.ExistingTextureUsedSpace = fontSystemLoadingSettings.ExistingTextureUsedSpace;
			};

			var fontSystem = new FontSystem(fontSystemSettings);
			var data = context.ReadAssetAsByteArray();
			fontSystem.AddFont(data);
			if (fontSystemLoadingSettings != null && fontSystemLoadingSettings.AdditionalFonts != null)
			{
				foreach (var file in fontSystemLoadingSettings.AdditionalFonts)
				{
					data = context.Manager.LoadByteArray(file, false);
					fontSystem.AddFont(data);
				}
			}

			return fontSystem;
		};

		private static AssetLoader<StaticSpriteFont> _staticFontLoader = (context) =>
		{
			var fontData = context.ReadDataAsString();

			return StaticSpriteFont.FromBMFont(fontData,
						name =>
						{
							var region = LoadTextureRegion(context.Manager, name);
							return new TextureWithOffset(region.Texture, region.Bounds.Location);
						});
		};

		private static AssetLoader<Stylesheet> _stylesheetLoader = (context) =>
		{
			var xml = context.ReadDataAsString();

			var xDoc = XDocument.Parse(xml);
			var attr = xDoc.Root.Attribute("TextureRegionAtlas");
			if (attr == null)
			{
				throw new Exception("Mandatory attribute 'TextureRegionAtlas' doesnt exist");
			}

			var textureRegionAtlas = context.Manager.LoadTextureRegionAtlas(attr.Value);

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
					var fontSystem = context.Manager.LoadFontSystem(fontFile, existingTexture: existingTexture, existingTextureUsedSpace: existingTextureUsedSpace);
					font = fontSystem.GetFont(float.Parse(el.Attribute("Size").Value));
				}
				else if (fontFile.EndsWith(".fnt"))
				{
					font = context.Manager.LoadStaticSpriteFont(fontFile);
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
			var texture = context.Load<Texture2DWrapper>(assetName);
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
