using System;
using System.Collections.Generic;
using MiniJSON;

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
		private const string LeftName = "left";
		private const string RightName = "right";
		private const string WidthName = "width";
		private const string HeightName = "height";
		private const string TopName = "top";
		private const string BottomName = "bottom";
		private const string TypeName = "type";
		private const string PaddingName = "padding";

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
			var root = new Dictionary<string, object>();

			foreach(var pair in _regions)
			{
				var entry = new Dictionary<string, object>();

				var region = pair.Value;

				var jBounds = new Dictionary<string, object>
				{
					[LeftName] = region.Bounds.X,
					[TopName] = region.Bounds.Y,
					[WidthName] = region.Bounds.Width,
					[HeightName] = region.Bounds.Height
				};

				entry[BoundsName] = jBounds;

				var asNinePatch = region as NinePatchRegion;
				if (asNinePatch == null)
				{
					entry[TypeName] = "0";
				} else
				{
					entry[TypeName] = "1";

					var jPadding = new Dictionary<string, object>
					{
						[LeftName] = asNinePatch.Info.Left,
						[TopName] = asNinePatch.Info.Top,
						[RightName] = asNinePatch.Info.Right,
						[BottomName] = asNinePatch.Info.Bottom
					};

					entry[PaddingName] = jPadding;
				}

				root[pair.Key] = entry;
			}

			return Json.Serialize(root);
		}

		public static TextureRegionAtlas FromJson(string json, Texture2D texture)
		{
			var root = (Dictionary<string, object>)Json.Deserialize(json);

			var regions = new Dictionary<string, TextureRegion>();

			foreach(var pair in root)
			{
				var entry = (Dictionary<string, object>)pair.Value;

				var jBounds = (Dictionary<string, object>)entry[BoundsName];
				var bounds = new Rectangle(int.Parse(jBounds[LeftName].ToString()),
					int.Parse(jBounds[TopName].ToString()),
					int.Parse(jBounds[WidthName].ToString()),
					int.Parse(jBounds[HeightName].ToString()));

				TextureRegion region;
				if (entry[TypeName].ToString() == "0")
				{
					region = new TextureRegion(texture, bounds);
				} else
				{
					var jPadding = (Dictionary<string, object>)entry[PaddingName];
					var padding = new PaddingInfo
					{
						Left = int.Parse(jPadding[LeftName].ToString()),
						Top = int.Parse(jPadding[TopName].ToString()),
						Right = int.Parse(jPadding[RightName].ToString()),
						Bottom = int.Parse(jPadding[BottomName].ToString())
					};

					region = new NinePatchRegion(texture, bounds, padding);
				}

				regions[pair.Key.ToString()] = region;
			}

			return new TextureRegionAtlas(regions);
		}
	}
}