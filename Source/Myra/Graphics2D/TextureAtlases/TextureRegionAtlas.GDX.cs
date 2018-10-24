using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.TextureAtlases
{
	public partial class TextureRegionAtlas
	{
		private enum GDXMode
		{
			PageHeader,
			Sprite
		}

		private class GDXPageData
		{
			public Texture2D Texture { get; set; }
			public SurfaceFormat Format { get; set; }
			public TextureFilter Filter { get; set; }
			public TextureAddressMode UWrap { get; set; }
			public TextureAddressMode VWrap { get; set; }

			public GDXPageData()
			{
				UWrap = TextureAddressMode.Clamp;
				VWrap = TextureAddressMode.Clamp;
			}
		}

		private class GDXSpriteData
		{
			public GDXPageData PageData { get; set; }
			public string Name { get; set; }
			public Rectangle SourceRectangle;
			public bool IsRotated { get; set; }
			public PaddingInfo? Split;
			public Point OriginalSize;
			public Point Offset;
		}

		private enum GDXTextureFilter
		{
			Nearest,
			Linear,
			MipMap,
			MipMapNearestNearest,
			MipMapLinearNearest,
			MipMapNearestLinear,
			MipMapLinearLinear
		}

		private static readonly Dictionary<string, TextureAddressMode> _gdxIdsToTextureWraps =
			new Dictionary<string, TextureAddressMode>();

		static TextureRegionAtlas()
		{
			_gdxIdsToTextureWraps["MirroredRepeat"] = TextureAddressMode.Mirror;
			_gdxIdsToTextureWraps["ClampToEdge"] = TextureAddressMode.Clamp;
			_gdxIdsToTextureWraps["Repeat"] = TextureAddressMode.Wrap;
		}

		private static TextureFilter FromGDXFilters(GDXTextureFilter min, GDXTextureFilter mag)
		{
			if (min == GDXTextureFilter.Nearest && mag == GDXTextureFilter.Nearest)
			{
				return TextureFilter.Point;
			}

			if (min == GDXTextureFilter.Linear && mag == GDXTextureFilter.Linear)
			{
				return TextureFilter.Linear;
			}

			throw new Exception("Not supported!");
		}

		public static TextureRegionAtlas FromGDX(string data, Func<string, Texture2D> textureLoader)
		{
			var mode = GDXMode.PageHeader;

			GDXPageData pageData = null;
			var spriteDatas = new Dictionary<string, GDXSpriteData>();

			using (var textReader = new StringReader(data))
			{
				GDXSpriteData spriteData = null;
				while (true)
				{
					var s = textReader.ReadLine();
					if (s == null)
					{
						break;
					}

					s = s.Trim();
					if (string.IsNullOrEmpty(s))
					{
						// New PageData
						pageData = null;
						continue;
					}

					if (pageData == null)
					{
						var texture = textureLoader(s);
						if (texture == null)
						{
							throw new Exception(string.Format("Unable to resolve texture {0}", s));
						}

						pageData = new GDXPageData {Texture = texture};
						mode = GDXMode.PageHeader;
						continue;
					}

					if (!s.Contains(":"))
					{
						spriteData = new GDXSpriteData
						{
							PageData = pageData,
							Name = s
						};

						spriteDatas[s] = spriteData;
						mode = GDXMode.Sprite;
						continue;
					}

					var parts = s.Split(':');

					var key = parts[0].Trim().ToLower();
					var value = parts[1].Trim();

					switch (key)
					{
						case "format":
							SurfaceFormat format;
							if (!Enum.TryParse(value, out format))
							{
								format = SurfaceFormat.Color;
							}

							pageData.Format = format;
							break;
						case "filter":
							parts = value.Split(',');
							var minFilter = (GDXTextureFilter) Enum.Parse(typeof(GDXTextureFilter), parts[0].Trim());
							var magFilter = (GDXTextureFilter) Enum.Parse(typeof(GDXTextureFilter), parts[1].Trim());

							pageData.Filter = FromGDXFilters(minFilter, magFilter);
							break;
						case "repeat":
							if (value == "x")
							{
								pageData.UWrap = TextureAddressMode.Wrap;
							}
							else if (value == "y")
							{
								pageData.VWrap = TextureAddressMode.Wrap;
							}
							else if (value == "xy")
							{
								pageData.UWrap = TextureAddressMode.Wrap;
								pageData.VWrap = TextureAddressMode.Wrap;
							}

							break;
						case "rotate":
							bool isRotated;
							bool.TryParse(value, out isRotated);
							spriteData.IsRotated = isRotated;
							break;
						case "xy":
							parts = value.Split(',');
							spriteData.SourceRectangle.X = int.Parse(parts[0].Trim());
							spriteData.SourceRectangle.Y = int.Parse(parts[1].Trim());

							break;
						case "size":
							if (spriteData == null)
							{
								continue;
							}

							parts = value.Split(',');
							spriteData.SourceRectangle.Width = int.Parse(parts[0].Trim());
							spriteData.SourceRectangle.Height = int.Parse(parts[1].Trim());

							break;
						case "orig":
							parts = value.Split(',');
							spriteData.OriginalSize = new Point(int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim()));

							break;
						case "offset":
							parts = value.Split(',');
							spriteData.Offset = new Point(int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim()));
							break;
						case "split":
							parts = value.Split(',');
							var split = new PaddingInfo
							{
								Left = int.Parse(parts[0].Trim()),
								Right = int.Parse(parts[1].Trim()),
								Top = int.Parse(parts[2].Trim()),
								Bottom = int.Parse(parts[3].Trim())
							};

							spriteData.Split = split;
							break;
					}
				}
			}

			var drawables = new Dictionary<string, TextureRegion>();

			foreach (var sd in spriteDatas)
			{
				var texture = sd.Value.PageData.Texture;
				var bounds = sd.Value.SourceRectangle;

				TextureRegion drawable;
				if (!sd.Value.Split.HasValue)
				{
					drawable = new TextureRegion(texture, bounds);
				}
				else
				{
					drawable = new NinePatchRegion(texture, bounds, sd.Value.Split.Value);
				}

				drawables[sd.Key] = drawable;
			}

			return new TextureRegionAtlas(drawables);
		}

	}
}