using Myra.Graphics2D.UI.Styles;
using Myra.MML;
using Myra.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using System.Globalization;
using FontStashSharp;



#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
using Texture2D = System.Object;
#endif

namespace AssetManagementBase
{
	partial class MyraAssetManagerExtensions
	{
		private static AssetLoader<Stylesheet> _stylesheetLoader = (manager, assetName, settings, tag) =>
		{
			var xmlData = manager.ReadAsString(assetName);
			var xDoc = XDocument.Parse(xmlData);

			var result = new Stylesheet();

			// Load atlas
			var attr = xDoc.Root.Attribute("TextureRegionAtlas");
			if (attr == null)
			{
				throw new Exception("Mandatory attribute 'TextureRegionAtlas' doesnt exist");
			}

			result.Atlas = manager.LoadTextureRegionAtlas(attr.Value);

			// Load fonts
			var fontsNode = xDoc.Root.Element("Fonts");

			var usedSpaceAttr = fontsNode.Attribute("UsedSpace");
			Texture2D existingTexture = null;
			if (usedSpaceAttr != null)
			{
				result.Fonts.AtlasUsedSpace = usedSpaceAttr.Value.ParseRectangle();

				existingTexture = result.Atlas.Texture;
			}

			var firstFont = string.Empty;
			foreach (var el in fontsNode.Elements())
			{
				var font = new StylesheetFont
				{
					Id = el.Attribute(BaseContext.IdName).Value,
					File = el.Attribute("File").Value
				};

				if (string.IsNullOrEmpty(firstFont))
				{
					firstFont = font.File;
				}

				if (font.File.EndsWith(".ttf") || font.File.EndsWith(".otf"))
				{
					font.Size = float.Parse(el.Attribute("Size").Value, CultureInfo.InvariantCulture);

					FontSystem fontSystem;
					if (existingTexture != null && result.Fonts.AtlasUsedSpace != null && firstFont.Equals(font.File, StringComparison.CurrentCultureIgnoreCase))
					{
						fontSystem = manager.LoadFontSystem(font.File, existingTexture: existingTexture, existingTextureUsedSpace: result.Fonts.AtlasUsedSpace.Value);
					} else
					{
						fontSystem = manager.LoadFontSystem(font.File);
					}

					font.Font = fontSystem.GetFont(font.Size.Value);
				}
				else if (font.File.EndsWith(".fnt"))
				{
					font.Font = manager.MyraLoadStaticSpriteFont(font.File);
				}
				else
				{
					throw new Exception(string.Format("Font '{0}' isn't supported", font.File));
				}

				result.Fonts[font.Id] = font;
			}

			// Load rest
			var loadContext = new LoadContext
			{
				Assemblies = new Dictionary<Assembly, string[]>()
				{
					{ typeof( WidgetStyle ).Assembly, new string[] { typeof( WidgetStyle ).Namespace } }
				},
				AssetManager = manager,
				NodesToIgnore = new HashSet<string>(new[] { "Designer", "Colors", "Fonts" }),
				LegacyClassNames = Stylesheet.LegacyClassNames,
				LegacyPropertyNames = Stylesheet.LegacyPropertyNames,
				DemandContentProperty = false,
				Stylesheet = result
			};

			loadContext.Load(result, xDoc.Root);

			return result;
		};

		/// <summary>
		/// Loads a UI stylesheet from an XML asset file.
		/// </summary>
		/// <param name="assetManager">The asset manager instance.</param>
		/// <param name="assetName">The name of the stylesheet asset to load.</param>
		/// <returns>The loaded stylesheet.</returns>
		public static Stylesheet LoadStylesheet(this AssetManager assetManager, string assetName) => assetManager.UseLoader(_stylesheetLoader, assetName);
	}
}
