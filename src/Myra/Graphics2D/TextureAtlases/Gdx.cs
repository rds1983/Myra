using System;
using System.Collections.Generic;
using System.IO;

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
	public static class Gdx
	{
		private enum GDXMode
		{
			PageHeader,
			Sprite
		}

		private class GDXPageData
		{
			public Texture2D Texture { get; set; }
		}

		private class GDXSpriteData
		{
			public GDXPageData PageData { get; set; }
			public string Name { get; set; }
			public Rectangle SourceRectangle;
			public bool IsRotated { get; set; }
			public Thickness? Split;
			public Point OriginalSize;
			public Point Offset;
		}

		public static TextureRegionAtlas FromGDX(string data, Func<string, Texture2D> textureLoader)
		{
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
						continue;
					}

					var parts = s.Split(':');

					var key = parts[0].Trim().ToLower();
					var value = parts[1].Trim();

					switch (key)
					{
						case "format":
							break;
						case "filter":
							break;
						case "repeat":
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
							var split = new Thickness
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

			var result = new TextureRegionAtlas();
			var regions = result.Regions;
			foreach (var sd in spriteDatas)
			{
				var texture = sd.Value.PageData.Texture;
				var bounds = sd.Value.SourceRectangle;

				TextureRegion region;
				if (!sd.Value.Split.HasValue)
				{
					region = new TextureRegion(texture, bounds);
				}
				else
				{
					region = new NinePatchRegion(texture, bounds, sd.Value.Split.Value);
				}

				regions[sd.Key] = region;
			}

			return result;
		}
	}
}