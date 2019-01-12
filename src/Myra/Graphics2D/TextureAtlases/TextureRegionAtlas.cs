using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

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
		private const string BoundsName = "bounds";

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

		public string ToJson()
		{
			var root = new JObject();

			foreach(var pair in _regions)
			{
				var entry = new JObject();

				var region = pair.Value;

				var jBounds = new JObject
				{
					[StylesheetLoader.LeftName] = region.Bounds.X,
					[StylesheetLoader.TopName] = region.Bounds.Y,
					[StylesheetLoader.WidthName] = region.Bounds.Width,
					[StylesheetLoader.HeightName] = region.Bounds.Height
				};

				entry[StylesheetLoader.BoundsName] = jBounds;

				var asNinePatch = region as NinePatchRegion;
				if (asNinePatch == null)
				{
					entry[StylesheetLoader.TypeName] = "0";
				} else
				{
					entry[StylesheetLoader.TypeName] = "1";

					var jPadding = new JObject
					{
						[StylesheetLoader.LeftName] = asNinePatch.Info.Left,
						[StylesheetLoader.TopName] = asNinePatch.Info.Top,
						[StylesheetLoader.RightName] = asNinePatch.Info.Right,
						[StylesheetLoader.BottomName] = asNinePatch.Info.Bottom
					};

					entry[StylesheetLoader.PaddingName] = jPadding;
				}

				root[pair.Key] = entry;
			}

			return root.ToString();
		}

		public static TextureRegionAtlas FromJson(string json, Texture2D texture)
		{
			var root = JObject.Parse(json);

			var regions = new Dictionary<string, TextureRegion>();

			foreach(var pair in root)
			{
				var entry = pair.Value;

				var jBounds = entry[StylesheetLoader.BoundsName];
				var bounds = new Rectangle(int.Parse(jBounds[StylesheetLoader.LeftName].ToString()),
					int.Parse(jBounds[StylesheetLoader.TopName].ToString()),
					int.Parse(jBounds[StylesheetLoader.WidthName].ToString()),
					int.Parse(jBounds[StylesheetLoader.HeightName].ToString()));

				TextureRegion region;
				if (entry[StylesheetLoader.TypeName].ToString() == "0")
				{
					region = new TextureRegion(texture, bounds);
				} else
				{
					var jPadding = entry[StylesheetLoader.PaddingName];
					var padding = new PaddingInfo
					{
						Left = int.Parse(jPadding[StylesheetLoader.LeftName].ToString()),
						Top = int.Parse(jPadding[StylesheetLoader.TopName].ToString()),
						Right = int.Parse(jPadding[StylesheetLoader.RightName].ToString()),
						Bottom = int.Parse(jPadding[StylesheetLoader.BottomName].ToString())
					};

					region = new NinePatchRegion(texture, bounds, padding);
				}

				regions[pair.Key.ToString()] = region;
			}

			return new TextureRegionAtlas(regions);
		}
	}
}