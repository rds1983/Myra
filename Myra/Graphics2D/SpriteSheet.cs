using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D
{
	public class SpriteSheet
	{
		private enum Mode
		{
			PageHeader,
			Sprite
		}

		public class PageData
		{
			public string FileName { get; set; }
			public Texture Texture { get; set; }
			public SurfaceFormat Format { get; set; }
			public TextureFilter Filter { get; set; }
			public TextureAddressMode UWrap { get; set; }
			public TextureAddressMode VWrap { get; set; }

			public PageData()
			{
				UWrap = TextureAddressMode.Clamp;
				VWrap = TextureAddressMode.Clamp;
			}
		}

		public class SpriteData
		{
			public PageData PageData { get; set; }
			public string Name { get; set; }
			public Rectangle SourceRectangle;
			public bool IsRotated { get; set; }
			public NinePatchInfo? Split;
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
		private readonly Dictionary<string, ImageDrawable> _drawables;

		public Dictionary<string, ImageDrawable> Drawables
		{
			get { return _drawables; }
		}

		static SpriteSheet()
		{
			_gdxIdsToTextureWraps["MirroredRepeat"] = TextureAddressMode.Mirror;
			_gdxIdsToTextureWraps["ClampToEdge"] = TextureAddressMode.Clamp;
			_gdxIdsToTextureWraps["Repeat"] = TextureAddressMode.Wrap;
		}

		public SpriteSheet(Dictionary<string, ImageDrawable> drawables)
		{
			if (drawables == null)
			{
				throw new ArgumentNullException("drawables");
			}

			_drawables = drawables;
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

		public static SpriteSheet LoadLibGDX(string data, Func<string, Texture2D> textureCreator, string[] fontNames = null)
		{
			var fontNamesHash = new HashSet<string>();

			if (fontNames != null)
			{
				foreach (var name in fontNames)
				{
					fontNamesHash.Add(name);
				}
			}

			var mode = Mode.PageHeader;

			PageData pageData = null;
			var spriteDatas = new Dictionary<string, SpriteData>();

			using (var textReader = new StringReader(data))
			{
				SpriteData spriteData = null;
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
						pageData = new PageData
						{
							FileName = s
						};

						mode = Mode.PageHeader;
						continue;
					}

					if (!s.Contains(":"))
					{
						spriteData = new SpriteData()
						{
							PageData = pageData,
							Name = s
						};

						spriteDatas[s] = spriteData;
						mode = Mode.Sprite;
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
							var minFilter = (GDXTextureFilter) Enum.Parse(typeof (GDXTextureFilter), parts[0].Trim());
							var magFilter = (GDXTextureFilter) Enum.Parse(typeof (GDXTextureFilter), parts[1].Trim());

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
							var split = new NinePatchInfo
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

			var drawables = new Dictionary<string, ImageDrawable>();

			foreach (var sd in spriteDatas)
			{
				var texture = textureCreator(sd.Value.PageData.FileName);

				var bounds = sd.Value.SourceRectangle;

				if (fontNamesHash.Contains(sd.Key))
				{
					// For fonts apply weird offset calculation
					// See original libGDX's BitmapFont.setGlyphRegion for more info

					var offset = new Point
					{
						X = sd.Value.Offset.X,
						Y = sd.Value.OriginalSize.Y - sd.Value.SourceRectangle.Height - sd.Value.Offset.Y
					};

					bounds.X -= offset.X;
					if (bounds.X < 0)
					{
						bounds.X = 0;
					}

					bounds.Y -= offset.Y;
					if (bounds.Y < 0)
					{
						bounds.Y = 0;
					}
				}

				ImageDrawable drawable;
				if (!sd.Value.Split.HasValue)
				{
					drawable = new Sprite(new TextureRegion(texture, bounds));
				}
				else
				{
					drawable = new NinePatchSprite(new TextureRegion(texture, bounds), sd.Value.Split.Value);
				}

				drawables[sd.Key] = drawable;
			}

			return new SpriteSheet(drawables);
		}
	}
}