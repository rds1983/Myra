// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.Tiled
{
	public class TmxLayer : ITmxElement
	{
		public string Name { get; private set; }

		// TODO: Legacy (Tiled Java) attributes (x, y, width, height)
		public int Width { get; private set; }
		public int Height { get; private set; }

		public float Opacity { get; private set; }
		public bool Visible { get; private set; }
		public float? OffsetX { get; private set; }
		public float? OffsetY { get; private set; }

		public Collection<TmxLayerTile> Tiles { get; private set; }
		public PropertyDict Properties { get; private set; }

		public TmxLayer(XElement xLayer, int width, int height)
		{
			Width = width;
			Height = height;

			Name = (string) xLayer.Attribute("name");
			Opacity = (float?) xLayer.Attribute("opacity") ?? 1.0f;
			Visible = (bool?) xLayer.Attribute("visible") ?? true;
			OffsetX = (float?) xLayer.Attribute("offsetx") ?? 0.0f;
			OffsetY = (float?) xLayer.Attribute("offsety") ?? 0.0f;

			var xData = xLayer.Element("data");
			var encoding = (string) xData.Attribute("encoding");

			Tiles = new Collection<TmxLayerTile>();
			switch (encoding)
			{
				case "base64":
					var decodedStream = new TmxBase64Data(xData);
					var stream = decodedStream.Data;

					using (var br = new BinaryReader(stream))
						for (int j = 0; j < height; j++)
							for (int i = 0; i < width; i++)
								Tiles.Add(new TmxLayerTile(br.ReadUInt32(), i, j));
					break;
				case "csv":
				{
					var csvData = xData.Value;
					var k = 0;
					foreach (var s in csvData.Split(','))
					{
						var gid = uint.Parse(s.Trim());
						var x = k%width;
						var y = k/width;
						Tiles.Add(new TmxLayerTile(gid, x, y));
						k++;
					}
				}
					break;
				case null:
				{
					int k = 0;
					foreach (var e in xData.Elements("tile"))
					{
						var gid = (uint) e.Attribute("gid");
						var x = k%width;
						var y = k/width;
						Tiles.Add(new TmxLayerTile(gid, x, y));
						k++;
					}
				}
					break;
				default:
					throw new Exception("TmxLayer: Unknown encoding.");
			}

			Properties = new PropertyDict(xLayer.Element("properties"));
		}
	}

	public class TmxLayerTile
	{
		// Tile flip bit flags
		const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
		const uint FLIPPED_VERTICALLY_FLAG = 0x40000000;
		const uint FLIPPED_DIAGONALLY_FLAG = 0x20000000;

		public int Gid { get; private set; }
		public int X { get; private set; }
		public int Y { get; private set; }
		public bool HorizontalFlip { get; private set; }
		public bool VerticalFlip { get; private set; }
		public bool DiagonalFlip { get; private set; }

		[CLSCompliant(false)]
		public TmxLayerTile(uint id, int x, int y)
		{
			var rawGid = id;
			X = x;
			Y = y;

			// Scan for tile flip bit flags
			bool flip;

			flip = (rawGid & FLIPPED_HORIZONTALLY_FLAG) != 0;
			HorizontalFlip = flip;

			flip = (rawGid & FLIPPED_VERTICALLY_FLAG) != 0;
			VerticalFlip = flip;

			flip = (rawGid & FLIPPED_DIAGONALLY_FLAG) != 0;
			DiagonalFlip = flip;

			// Zero the bit flags
			rawGid &= ~(FLIPPED_HORIZONTALLY_FLAG |
			            FLIPPED_VERTICALLY_FLAG |
			            FLIPPED_DIAGONALLY_FLAG);

			// Save GID remainder to int
			Gid = (int) rawGid;
		}
	}
}