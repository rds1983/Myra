// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.Tiled
{
	public class TmxLayer : ITmxElement
	{
		private readonly TmxLayerTile[,] _tiles;

		public string Name { get; private set; }

		// TODO: Legacy (Tiled Java) attributes (x, y, width, height)
		public int Width { get; private set; }
		public int Height { get; private set; }

		public float Opacity { get; private set; }
		public bool Visible { get; private set; }
		public float? OffsetX { get; private set; }
		public float? OffsetY { get; private set; }

		public PropertyDict Properties { get; private set; }

		public Point Offset;

		public TmxLayerTile this[int x, int y]
		{
			get { return _tiles[x, y]; }
			set { _tiles[x, y] = value; }
		}

		public TmxLayer(XElement xLayer, int width, int height)
		{
			if (width == 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}

			if (height == 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}

			Width = width;
			Height = height;

			Name = (string) xLayer.Attribute("name");
			Opacity = (float?) xLayer.Attribute("opacity") ?? 1.0f;
			Visible = (bool?) xLayer.Attribute("visible") ?? true;
			OffsetX = (float?) xLayer.Attribute("offsetx") ?? 0.0f;
			OffsetY = (float?) xLayer.Attribute("offsety") ?? 0.0f;

			var xData = xLayer.Element("data");
			var encoding = (string) xData.Attribute("encoding");

			_tiles = new TmxLayerTile[width, height];

			switch (encoding)
			{
				case "base64":
					var decodedStream = new TmxBase64Data(xData);
					var stream = decodedStream.Data;

					using (var br = new BinaryReader(stream))
						for (int j = 0; j < height; j++)
							for (int i = 0; i < width; i++)
								_tiles[i, j] = new TmxLayerTile(br.ReadUInt32());
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
						_tiles[x, y] = new TmxLayerTile(gid);
						k++;
					}
				}
					break;
				case null:
				{
					var k = 0;
					foreach (var e in xData.Elements("tile"))
					{
						var gid = (uint) e.Attribute("gid");
						var x = k%width;
						var y = k/width;
						_tiles[x, y] = new TmxLayerTile(gid);
						k++;
					}
				}
					break;
				default:
					throw new Exception("TmxLayer: Unknown encoding.");
			}

			Properties = new PropertyDict(xLayer.Element("properties"));
		}

		public void Update(TmxMap parent)
		{
			var offsetX = 0;
			if (OffsetX.HasValue)
			{
				offsetX = -(int)(OffsetX.Value * parent.TileWidth);
			}

			var offsetY = 0;
			if (OffsetY.HasValue)
			{
				offsetY = -(int)(OffsetY.Value * parent.TileHeight);
			}

			Offset = new Point(offsetX, offsetY);
			for (var x = 0; x < Width; ++x)
			{
				for (var y = 0; y < Height; ++y)
				{
					var tile = _tiles[x, y];
					if (tile == null)
					{
						continue;
					}

					tile.Update(parent);
				}
			}
		}
	}

	public class TmxLayerTile
	{
		// Tile flip bit flags
		const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
		const uint FLIPPED_VERTICALLY_FLAG = 0x40000000;
		const uint FLIPPED_DIAGONALLY_FLAG = 0x20000000;

		public int Gid { get; private set; }
		public bool HorizontalFlip { get; private set; }
		public bool VerticalFlip { get; private set; }
		public bool DiagonalFlip { get; private set; }

		public TextureRegion2D Region { get; set; }
		public Point Offset;
		public Point Size;

		[CLSCompliant(false)]
		public TmxLayerTile(uint id)
		{
			var rawGid = id;

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

		public void Update(TmxMap tileMap)
		{
			if (Gid == 0)
			{
				return;
			}

			var gid = Gid;

			TmxTileset tileset = null;
			foreach (var ts in tileMap.Tilesets)
			{
				if (gid < ts.FirstGid + ts.TotalTiles)
				{
					gid -= ts.FirstGid;
					tileset = ts;
					break;
				}
			}

			if (tileset == null)
			{
				return;
			}

			Offset = new Point(tileset.TileOffset.X, tileset.TileOffset.Y);
			Size = new Point(tileset.TileWidth, tileset.TileHeight);

			var col = gid % tileset.TilesPerRow;
			var row = gid / tileset.TilesPerRow;
			var sourceX = col * tileset.TileWidth;
			sourceX += col * tileset.Spacing;
			sourceX += tileset.Margin;

			var sourceY = row * tileset.TileHeight;
			sourceY += row * tileset.Spacing;
			sourceY += tileset.Margin;

			var sourceRect = new Rectangle(sourceX, sourceY,
				tileset.TileWidth - tileset.Margin,
				tileset.TileHeight - tileset.Margin);

			Region = new TextureRegion2D(tileset.Image.Texture, sourceRect);
		}
	}
}