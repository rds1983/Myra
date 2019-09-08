using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Xml.Linq;

namespace Myra.TextureAtlasConverter
{
	class Program
	{
		private const string BoundsName = "bounds";
		private const string LeftName = "left";
		private const string RightName = "right";
		private const string WidthName = "width";
		private const string HeightName = "height";
		private const string TopName = "top";
		private const string BottomName = "bottom";
		private const string TypeName = "type";
		private const string PaddingName = "padding";

		public static XDocument FromJson(string json)
		{
			var root = JObject.Parse(json);

			var result = new XDocument();

			var rootNode = new XElement("TextureAtlas");
			result.Add(rootNode);

			foreach (var pair in root)
			{
				var entry = (JObject)pair.Value;

				XElement region;
				var isNinePatch = false;
				if (entry[TypeName].ToString() == "0")
				{
					region = new XElement("TextureRegion");
				}
				else
				{
					region = new XElement("NinePatchRegion");
					isNinePatch = true;
				}

				region.SetAttributeValue("Id", pair.Key);

				var jBounds = (JObject)entry[BoundsName];
				region.SetAttributeValue("Left", jBounds[LeftName].ToString());
				region.SetAttributeValue("Top", jBounds[TopName].ToString());
				region.SetAttributeValue("Width", jBounds[WidthName].ToString());
				region.SetAttributeValue("Height", jBounds[HeightName].ToString());

				if (isNinePatch)
				{
					var jPadding = (JObject)entry[PaddingName];
					region.SetAttributeValue("NinePatchLeft", jPadding[LeftName].ToString());
					region.SetAttributeValue("NinePatchTop", jPadding[TopName].ToString());
					region.SetAttributeValue("NinePatchRight", jPadding[RightName].ToString());
					region.SetAttributeValue("NinePatchBottom", jPadding[BottomName].ToString());
				}

				rootNode.Add(region);
			}

			return result;
		}

		static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Myra.TextureAtlasConverter converts old json texture atlas to new xml texture atlas.");
				Console.WriteLine("Usage: Myra.TextureAtlasConverter <input.json> <output.xml>");

				return;
			}

			try
			{
				var inputFile = args[0];
				var outputFile = args[1];

				var doc = FromJson(File.ReadAllText(inputFile));
				doc.Save(args[1]);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
