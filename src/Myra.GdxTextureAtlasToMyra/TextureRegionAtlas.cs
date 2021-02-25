using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;

namespace Myra
{
	public struct Thickness
	{
		public int Left, Top, Right, Bottom;
	}

	public class TextureRegion
	{
		public Rectangle Bounds;
	}

	public class NinePatchRegion: TextureRegion
	{
		public Thickness Info;
	}

	public class TextureRegionAtlas
	{
		private const string IdName = "Id";
		private const string TextureAtlasName = "TextureAtlas";
		private const string ImageName = "Image";
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

		public string ImageFile;
		public readonly Dictionary<string, TextureRegion> Regions = new Dictionary<string, TextureRegion>();

		public string ToXml()
		{
			var doc = new XDocument();
			var root = new XElement(TextureAtlasName);
			root.SetAttributeValue(ImageName, ImageFile);
			doc.Add(root);

			foreach (var pair in Regions)
			{
				var region = pair.Value;
				var asNinePatch = region as NinePatchRegion;

				var entry = new XElement(asNinePatch != null ? NinePatchRegionName : TextureRegionName);

				entry.SetAttributeValue(IdName, pair.Key);
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
	}
}
