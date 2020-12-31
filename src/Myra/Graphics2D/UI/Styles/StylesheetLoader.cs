using Myra.Graphics2D.TextureAtlases;
using Myra.MML;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using FontStashSharp;
using XNAssets;

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
			foreach (var el in fontsNode.Elements())
			{
				SpriteFontBase font = null;

				var fontFile = el.Attribute("File").Value;
				if (fontFile.EndsWith(".ttf"))
				{
					var parts = new List<string>();
					parts.Add(fontFile);
					
					var typeAttribute = el.Attribute("Type");
					if (typeAttribute != null)
					{
						parts.Add(typeAttribute.Value);

						var amountAttribute = el.Attribute("Amount");
						parts.Add(amountAttribute.Value);
					}

					parts.Add(el.Attribute("Size").Value);
					font = context.Load<DynamicSpriteFont>(string.Join(":", parts));
				} else if (fontFile.EndsWith(".fnt"))
				{
					font = context.Load<StaticSpriteFont>(fontFile);
				} else
				{
					throw new Exception(string.Format("Font '{0}' isn't supported", fontFile));
				}

				fonts[el.Attribute(BaseContext.IdName).Value] = font;
			}

			return Stylesheet.LoadFromSource(xml, textureRegionAtlas, fonts);
		}
	}
}