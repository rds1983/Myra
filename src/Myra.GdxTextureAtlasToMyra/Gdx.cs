using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Myra
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
			public string ImageFile;
		}

		private class GDXSpriteData
		{
			public string Name { get; set; }
			public Rectangle SourceRectangle;
			public bool IsRotated { get; set; }
			public Thickness? Split;
			public Point OriginalSize;
			public Point Offset;
		}

		public static TextureRegionAtlas FromGDX(string data)
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
						continue;
					}

					if (pageData == null)
					{
						pageData = new GDXPageData { ImageFile = s };
						continue;
					}

					if (!s.Contains(":"))
					{
						spriteData = new GDXSpriteData
						{
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

			if (pageData == null)
			{
				throw new Exception("Could not parse header");
			}

			var result = new TextureRegionAtlas
			{
				ImageFile = pageData.ImageFile
			};

			var regions = result.Regions;
			foreach (var sd in spriteDatas)
			{
				var bounds = sd.Value.SourceRectangle;

				TextureRegion region;
				if (!sd.Value.Split.HasValue)
				{
					region = new TextureRegion
					{
						Bounds = bounds
					};
				}
				else
				{
					region = new NinePatchRegion
					{
						Bounds = bounds,
						Info = sd.Value.Split.Value
					};
				}

				regions[sd.Key] = region;
			}

			return result;
		}
	}
}
