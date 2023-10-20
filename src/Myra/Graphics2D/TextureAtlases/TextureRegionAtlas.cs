using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Myra.MML;

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

namespace Myra.Graphics2D.TextureAtlases
{
	public partial class TextureRegionAtlas
	{
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

		public string Image { get; set; }

		public Dictionary<string, TextureRegion> Regions { get; } = new Dictionary<string, TextureRegion>();

		public Texture2D Texture { get; private set; }

		public TextureRegion this[string name]
		{
			get
			{
				return Regions[name];
			}
			set
			{
				Regions[name] = value;
			}
		}

		public TextureRegion EnsureRegion(string id)
		{
			TextureRegion result;
			if (!Regions.TryGetValue(id, out result))
			{
				throw new ArgumentNullException(string.Format("Could not resolve region '{0}'", id));
			}

			return result;
		}

		public string ToXml()
		{
			var doc = new XDocument();
			var root = new XElement(TextureAtlasName);
			root.SetAttributeValue(ImageName, Image);
			doc.Add(root);

			foreach(var pair in Regions)
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

		/// <summary>
		/// Loads TextureAtlas from either LibGDX .atlas or Myra .xml format
		/// </summary>
		/// <param name="data"></param>
		/// <param name="textureGetter"></param>
		/// <returns></returns>
		public static TextureRegionAtlas Load(string data, Func<string, Texture2D> textureGetter)
		{
			bool isXml;
			try
			{
				var xDoc = XDocument.Parse(data);
				isXml = true;
			}
			catch (Exception)
			{
				isXml = false;
			}

			if (isXml)
			{
				return FromXml(data, textureGetter);
			}

			return Gdx.FromGDX(data, textureGetter);
		}

		public static TextureRegionAtlas FromXml(string xml, Func<string, Texture2D> textureGetter)
		{
			var doc = XDocument.Parse(xml);
			var root = doc.Root;

			var result = new TextureRegionAtlas();
			var imageFileAttr = root.Attribute(ImageName);
			if (imageFileAttr == null)
			{
				throw new Exception("Mandatory attribute 'ImageFile' doesnt exist");
			}

			result.Image = imageFileAttr.Value;

			var texture = textureGetter(result.Image);
			result.Texture = texture;
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
					var padding = new Thickness
					{
						Left = int.Parse(entry.Attribute(NinePatchLeftName).Value),
						Top = int.Parse(entry.Attribute(NinePatchTopName).Value),
						Right = int.Parse(entry.Attribute(NinePatchRightName).Value),
						Bottom = int.Parse(entry.Attribute(NinePatchBottomName).Value)
					};

					region = new NinePatchRegion(texture, bounds, padding);
				}

				result[id] = region;
			}

			return result;
		}
	}
}