using FontStashSharp;
using FontStashSharp.RichText;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace AssetManagementBase
{
	/// <summary>
	/// Provides extension methods for the AssetManager class to load Myra-specific assets like texture atlases, fonts, and stylesheets.
	/// </summary>
	public static partial class MyraAssetManagerExtensions
	{
		private static AssetLoader<StaticSpriteFont> _staticFontLoader = (manager, assetName, settings, tag) =>
		{
			var fontData = manager.ReadAsString(assetName);

			var result = StaticSpriteFont.FromBMFont(fontData,
						name =>
						{
							var region = LoadTextureRegion(manager, name);
							return new TextureWithOffset(region.Texture, region.Bounds.Location);
						});

			result.Name = assetName;

			return result;
		};

		private static AssetLoader<TextureRegionAtlas> _atlasLoader = (manager, assetName, settings, tag) =>
		{
			var data = manager.ReadAsString(assetName);

#if !PLATFORM_AGNOSTIC
			var result = TextureRegionAtlas.FromXml(data, name => manager.LoadTexture2D(MyraEnvironment.GraphicsDevice, name, true));
#else
			var result = TextureRegionAtlas.FromXml(data, name => manager.LoadTexture2D(name).Texture);
#endif

			result.Name = assetName;

			return result;
		};

		private static AssetLoader<Project> _projectLoader = (manager, assetName, settings, tag) =>
		{
			var data = manager.ReadAsString(assetName);

			return Project.LoadFromXml(data, manager);
		};

		/// <summary>
		/// Loads a texture region atlas from an XML asset file.
		/// </summary>
		/// <param name="assetManager">The asset manager instance.</param>
		/// <param name="assetName">The name of the atlas asset to load.</param>
		/// <returns>The loaded texture region atlas.</returns>
		public static TextureRegionAtlas LoadTextureRegionAtlas(this AssetManager assetManager, string assetName) => assetManager.UseLoader(_atlasLoader, assetName);

		/// <summary>
		/// Loads a Myra project from an XML asset file.
		/// </summary>
		/// <param name="assetManager">The asset manager instance.</param>
		/// <param name="assetName">The name of the project asset to load.</param>
		/// <returns>The loaded project.</returns>
		public static Project LoadProject(this AssetManager assetManager, string assetName) => assetManager.UseLoader(_projectLoader, assetName);

		/// <summary>
		/// Loads a texture region from an asset, with optional stylesheet context for resolving atlas references.
		/// </summary>
		/// <param name="assetManager">The asset manager instance.</param>
		/// <param name="assetName">The name of the texture region asset to load. Can be an atlas reference, atlas name:region format, or file path.</param>
		/// <param name="stylesheet">The stylesheet context for resolving atlas names. If null, loads as file.</param>
		/// <returns>The loaded texture region.</returns>
		public static TextureRegion LoadTextureRegion(this AssetManager assetManager, string assetName, Stylesheet stylesheet)
		{
			string regionName;
			if (TextureRegionAtlas.TryGetRegionName(ref assetName, out regionName))
			{
				var textureRegionAtlas = assetManager.LoadTextureRegionAtlas(assetName);
				return textureRegionAtlas[regionName];
			}

			if (stylesheet != null && !assetName.Contains("."))
			{
				// If there's no extension, assume it's a texture region atlas with id equal to the asset name
				var textureRegionAtlas = stylesheet.Atlas;
				return textureRegionAtlas[assetName];
			}

			// Ordinary texture
#if MONOGAME || FNA || STRIDE
			var texture = assetManager.LoadTexture2D(MyraEnvironment.GraphicsDevice, assetName);
			var result = new TextureRegion(texture, new Rectangle(0, 0, texture.Width, texture.Height));
#else
			var texture = assetManager.LoadTexture2D(assetName);
			var result = new TextureRegion(texture.Texture, new Rectangle(0, 0, texture.Width, texture.Height));
#endif

			result.Name = assetName;

			return result;
		}

		/// <summary>
		/// Loads a texture region from an asset using the current stylesheet context.
		/// </summary>
		/// <param name="assetManager">The asset manager instance.</param>
		/// <param name="assetName">The name of the texture region asset to load.</param>
		/// <returns>The loaded texture region.</returns>
		public static TextureRegion LoadTextureRegion(this AssetManager assetManager, string assetName) => LoadTextureRegion(assetManager, assetName, Stylesheet.Current);

		/// <summary>
		/// Loads an image asset with optional color tinting, with stylesheet context for resolving atlas references.
		/// </summary>
		/// <param name="assetManager">The asset manager instance.</param>
		/// <param name="assetName">The name of the image asset to load. Can include color tint separated by '/'.</param>
		/// <param name="stylesheet">The stylesheet context for resolving atlas names.</param>
		/// <returns>The loaded image, or a tinted version if a color tint was specified.</returns>
		public static IImage LoadImage(this AssetManager assetManager, string assetName, Stylesheet stylesheet)
		{
			var parts = assetName.Split(TintedRegion.Separator);
			Color? color = null;
			if (parts.Length > 1)
			{
				color = ColorStorage.FromName(parts[1]);
				if (color == null)
				{
					throw new Exception($"Could not parse color name '{parts[1]}'");
				}
				assetName = parts[0];
			}

			var region = assetManager.LoadTextureRegion(assetName, stylesheet);
			if (color == null)
			{
				return region;
			}

			return new TintedRegion(region, color.Value);
		}

		/// <summary>
		/// Loads an image asset with optional color tinting using the current stylesheet context.
		/// </summary>
		/// <param name="assetManager">The asset manager instance.</param>
		/// <param name="assetName">The name of the image asset to load.</param>
		/// <returns>The loaded image, or a tinted version if a color tint was specified.</returns>
		public static IImage LoadImage(this AssetManager assetManager, string assetName) => LoadImage(assetManager, assetName, Stylesheet.Current);

		/// <summary>
		/// Loads a brush asset, resolving color names or atlas region references with stylesheet context.
		/// </summary>
		/// <param name="assetManager">The asset manager instance.</param>
		/// <param name="assetName">The name of the brush asset to load. Can be a color name or image reference.</param>
		/// <param name="stylesheet">The stylesheet context for resolving atlas references.</param>
		/// <returns>The loaded brush, either a SolidBrush for colors or an image-based brush.</returns>
		public static IBrush LoadBrush(this AssetManager assetManager, string assetName, Stylesheet stylesheet)
		{
			if (!assetName.Contains(".") && assetName.IndexOf(TintedRegion.Separator) == -1 && assetName.IndexOf(StylesheetFont.Separator) == -1)
			{
				// It's either a default stylesheet texture atlas region or color name
				if (stylesheet == null || !stylesheet.Atlas.Regions.TryGetValue(assetName, out var region))
				{
					// Color
					var color = ColorStorage.FromName(assetName);
					if (color == null)
					{
						throw new Exception($"Could not parse brush name '{assetName}'");
					}

					return new SolidBrush(color.Value);
				}
			}

			return assetManager.LoadImage(assetName, stylesheet);
		}

		/// <summary>
		/// Loads a brush asset using the current stylesheet context.
		/// </summary>
		/// <param name="assetManager">The asset manager instance.</param>
		/// <param name="assetName">The name of the brush asset to load.</param>
		/// <returns>The loaded brush.</returns>
		public static IBrush LoadBrush(this AssetManager assetManager, string assetName) => LoadBrush(assetManager, assetName, Stylesheet.Current);

		/// <summary>
		/// Loads a static sprite font (BMFont) from an asset file.
		/// </summary>
		/// <param name="assetManager">The asset manager instance.</param>
		/// <param name="assetName">The name of the font asset to load.</param>
		/// <returns>The loaded static sprite font.</returns>
		private static StaticSpriteFont MyraLoadStaticSpriteFont(this AssetManager assetManager, string assetName) => assetManager.UseLoader(_staticFontLoader, assetName);

		/// <summary>
		/// Loads a font asset, supporting BMFont files, TrueType fonts, and stylesheet-defined fonts.
		/// </summary>
		/// <param name="assetManager">The asset manager instance.</param>
		/// <param name="assetName">The name of the font asset to load. Can be a file path or stylesheet font name.</param>
		/// <param name="stylesheet">The stylesheet context for resolving font names.</param>
		/// <returns>The loaded sprite font.</returns>
		public static SpriteFontBase LoadFont(this AssetManager assetManager, string assetName, Stylesheet stylesheet)
		{
			int? fontSize = null;
			string parameter;

			var originalAssetName = assetName;
			if (StylesheetFont.TryGetParameter(ref assetName, out parameter))
			{
				int fs;
				if (!int.TryParse(parameter, out fs) || fs <= 0)
				{
					throw new Exception($"Invalid font size {fontSize}.");
				}

				fontSize = fs;
			}

			SpriteFontBase result = null;

			do
			{
				if (stylesheet != null && !assetName.Contains("."))
				{
					// If there's no extension, assume it's a current stylesheet font
					if (!stylesheet.Fonts.TryGetValue(assetName, out var font))
					{
						throw new Exception($"Font '{assetName}' not found in current stylesheet.");
					}

					result = font.Font;
					if (fontSize != null)
					{
						var asDynamicFont = result as DynamicSpriteFont;
						if (asDynamicFont != null)
						{
							// Custom font size
							result = asDynamicFont.FontSystem.GetFont(fontSize.Value);
						}
						else
						{
							throw new Exception($"Font '{assetName}' size can't be modified.");
						}
					}

					break;
				}

				if (assetName.Contains(".fnt"))
				{
					result = assetManager.MyraLoadStaticSpriteFont(assetName);
					break;
				}

				if (assetName.Contains(".ttf") || assetName.Contains(".otf"))
				{
					if (fontSize == null)
					{
						throw new Exception("Missing font size.");
					}

					var fontSystem = assetManager.LoadFontSystem(assetName);
					result = fontSystem.GetFont(fontSize.Value);
					break;
				}
			}
			while (false);

			if (result == null)
			{
				throw new Exception(string.Format("Can't load font '{0}'", assetName));
			}

			result.Name = originalAssetName;

			return result;
		}

		/// <summary>
		/// Loads a font asset using the current stylesheet context.
		/// </summary>
		/// <param name="assetManager">The asset manager instance.</param>
		/// <param name="assetName">The name of the font asset to load.</param>
		/// <returns>The loaded sprite font.</returns>
		public static SpriteFontBase LoadFont(this AssetManager assetManager, string assetName) => LoadFont(assetManager, assetName, Stylesheet.Current);
	}
}
