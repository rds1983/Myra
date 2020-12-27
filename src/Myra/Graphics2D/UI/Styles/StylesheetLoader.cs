using Myra.Graphics2D.TextureAtlases;
using Myra.MML;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using XNAssets;
using FontStashSharp;
using System.IO;

#if STRIDE
using Stride.Graphics;
#endif

namespace Myra.Graphics2D.UI.Styles
{
	public class StylesheetLoader : IAssetLoader<Stylesheet>
	{
		private enum FontType
		{
			Regular,
			Blurry,
			Stroked
		}

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
			var fonts = new Dictionary<string, DynamicSpriteFont>();
			var fontsNode = xDoc.Root.Element("Fonts");
			foreach (var el in fontsNode.Elements())
			{
				var fontFile = el.Attribute("File").Value;
				var fontSize = int.Parse(el.Attribute("Size").Value);

				var fontType = FontType.Regular;
				var typeAttribute = el.Attribute("Type");
				if (typeAttribute != null)
				{
					fontType = (FontType)Enum.Parse(typeof(FontType), typeAttribute.Value);
				}

				var amount = 1;
				var amountAttribute = el.Attribute("Amount");
				if (amountAttribute != null)
				{
					amount = int.Parse(amountAttribute.Value);
				}

				FontSystem fontSystem = null;
				switch (fontType)
				{
					case FontType.Regular:
						fontSystem = FontSystemFactory.Create(context.GraphicsDevice, 1024, 1024);
						break;
					case FontType.Blurry:
						fontSystem = FontSystemFactory.CreateBlurry(context.GraphicsDevice, 1024, 1024, amount);
						break;
					case FontType.Stroked:
						fontSystem = FontSystemFactory.CreateStroked(context.GraphicsDevice, 1024, 1024, amount);
						break;
				}

				using (var stream = context.Open(fontFile))
				using (var ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					fontSystem.AddFont(ms.ToArray());
				}

				fonts[el.Attribute(BaseContext.IdName).Value] = fontSystem.GetFont(fontSize);
			}

			return Stylesheet.LoadFromSource(xml, textureRegionAtlas, fonts);
		}
	}
}