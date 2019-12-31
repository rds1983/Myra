using Myra.Assets;
using Myra.Graphics2D.TextureAtlases;
using Myra.MML;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

#if !XENKO
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Graphics;
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
			var fonts = new Dictionary<string, SpriteFont>();
			var fontsNode = xDoc.Root.Element("Fonts");
			foreach (var el in fontsNode.Elements())
			{
				var font = el.Attribute("File").Value;
				fonts[el.Attribute(BaseContext.IdName).Value] = context.Load<SpriteFont>(font);
			}

			return Stylesheet.LoadFromSource(xml,
				name => textureRegionAtlas[name],
				name => fonts[name]);
		}
	}
}