using Myra.Graphics2D.TextureAtlases;
using Myra.MML;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using FontStashSharp;
using Myra.Utility;
using Myra.Assets;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
#endif

namespace Myra.Graphics2D.UI.Styles
{
	public class StylesheetLoader : IAssetLoader<Stylesheet>
	{
		public Stylesheet Load(AssetLoaderContext context, string assetName)
		{
			var xml = context.Load<string>(assetName);

			var xDoc = XDocument.Parse(xml);
			var attr = xDoc.Root.Attribute("TextureRegionAtlas");
			if (attr == null)
			{
				throw new Exception("Mandatory attribute 'TextureRegionAtlas' doesnt exist");
			}

			var textureRegionAtlas = context.Load<TextureRegionAtlas>(attr.Value);

			// Load fonts
			var fonts = new Dictionary<string, SpriteFontBase>();
			var fontsNode = xDoc.Root.Element("Fonts");

			try
			{
				var usedSpaceAttr = fontsNode.Attribute("UsedSpace");
				if (usedSpaceAttr != null)
				{
					var usedSpace = usedSpaceAttr.Value.ParseRectangle();

					FontSystemLoader.ExistingTexture = textureRegionAtlas.Texture;
					FontSystemLoader.ExistingTextureUsedSpace = usedSpace;
				}
				foreach (var el in fontsNode.Elements())
				{
					SpriteFontBase font = null;

					var fontFile = el.Attribute("File").Value;
					if (fontFile.EndsWith(".ttf") || fontFile.EndsWith(".otf"))
					{
						var parts = new List<string>
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

						parts.Add(el.Attribute("Size").Value);
						font = context.Load<DynamicSpriteFont>(string.Join(":", parts));
					}
					else if (fontFile.EndsWith(".fnt"))
					{
						font = context.Load<StaticSpriteFont>(fontFile);
					}
					else
					{
						throw new Exception(string.Format("Font '{0}' isn't supported", fontFile));
					}

					fonts[el.Attribute(BaseContext.IdName).Value] = font;
				}
			}
			finally
			{
				FontSystemLoader.ExistingTexture = null;
			}

			return Stylesheet.LoadFromSource(xml, textureRegionAtlas, fonts);
		}
	}
}