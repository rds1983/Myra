using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Myra.MML;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Texture2D = Xenko.Graphics.Texture;
#endif

namespace Myra.Graphics2D.TextureAtlases
{
	public partial class TextureRegionAtlas
	{
		private const string TextureAtlasName = "TextureAtlas";
		private const string TextureRegionName = "TextureRegion";
		private const string NinePatchRegionName = "NinePatchRegion";
		private const string LeftName = "Left";
		private const string TopName = "Top";
		private const string WidthName = "Width";
		private const string HeightName = "Height";
		private const string NinePatchLeftName = "NinePatchLeft";
		private const string NinePatchTopName = "NinePatchTop";
		private const string NinePatchRightName = "NinePatchRight";
		private const string NinePatchBottomName = "NinePatchBottom";

		private readonly Dictionary<string, TextureRegion> _regions;

		public Dictionary<string, TextureRegion> Regions
		{
			get { return _regions; }
		}

		public TextureRegion this[string name]
		{
			get { return Regions[name]; }
		}

		public TextureRegionAtlas(Dictionary<string, TextureRegion> regions)
		{
			if (regions == null)
			{
				throw new ArgumentNullException("regions");
			}

			_regions = regions;
		}

		public TextureRegion EnsureRegion(string id)
		{
			TextureRegion result;
			if (!_regions.TryGetValue(id, out result))
			{
				throw new ArgumentNullException(string.Format("Could not resolve region '{0}'", id));
			}

			return result;
		}

		public string ToXml()
		{
			var doc = new XDocument();
			var root = new XElement(TextureAtlasName);
			doc.Add(root);

			foreach(var pair in _regions)
			{
				var region = pair.Value;
				var asNinePatch = region as NinePatchRegion;

				var entry = new XElement(asNinePatch != null ? NinePatchRegionName : TextureRegionName);

				entry.SetAttributeValue(BaseContext.IdName, pair.Key);
				entry.SetAttributeValue(LeftName, region.Bounds.Left);
				entry.SetAttributeValue(TopName, region.Bounds.Top);
				entry.SetAttributeValue(WidthName, region.Bounds.Width);
				entry.SetAttributeValue(HeightName, region.Bounds.Height);

				if (asNinePatch != null)
				{
					entry.SetAttributeValue(NinePatchLeftName, asNinePatch.Info.Left);
					entry.SetAttributeValue(NinePatchTopName, asNinePatch.Info.Top);
					entry.SetAttributeValue(NinePatchRightName, asNinePatch.Info.Right);
					entry.SetAttributeValue(NinePatchBottomName, asNinePatch.Info.Bottom);
				}

				root.Add(entry);
			}

			return doc.ToString();
		}

		public static TextureRegionAtlas FromXml(string xml, Texture2D texture)
		{
			var doc = XDocument.Parse(xml);
			var root = doc.Root;

			var regions = new Dictionary<string, TextureRegion>();

			foreach(XElement entry in root.Elements())
			{
				var id = entry.Attribute(BaseContext.IdName).Value;

				var bounds = new Rectangle(
					int.Parse(entry.Attribute(LeftName).Value),
					int.Parse(entry.Attribute(TopName).Value),
					int.Parse(entry.Attribute(WidthName).Value),
					int.Parse(entry.Attribute(HeightName).Value)
				);

				var isNinePatch = entry.Name == NinePatchRegionName;

				TextureRegion region;
				if (!isNinePatch)
				{
					region = new TextureRegion(texture, bounds);
				} else
				{
					var padding = new PaddingInfo
					{
						Left = int.Parse(entry.Attribute(NinePatchLeftName).Value),
						Top = int.Parse(entry.Attribute(NinePatchTopName).Value),
						Right = int.Parse(entry.Attribute(NinePatchRightName).Value),
						Bottom = int.Parse(entry.Attribute(NinePatchBottomName).Value)
					};

					region = new NinePatchRegion(texture, bounds, padding);
				}

				regions[id] = region;
			}

			return new TextureRegionAtlas(regions);
		}
	}
}