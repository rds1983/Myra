// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Myra.Graphics2D.Tiles;

namespace Myra.Graphics2D.Tiled
{
	public class TmxMap
	{
		public string Version { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int TileWidth { get; private set; }
		public int TileHeight { get; private set; }
		public int? HexSideLength { get; private set; }
		public OrientationType Orientation { get; private set; }
		public StaggerAxisType StaggerAxis { get; private set; }
		public StaggerIndexType StaggerIndex { get; private set; }
		public RenderOrderType RenderOrder { get; private set; }
		public TmxColor BackgroundColor { get; private set; }
		public int? NextObjectID { get; private set; }

		public List<TmxTileset> Tilesets { get; private set; }
		public TmxList<TmxLayer> Layers { get; private set; }
		public TmxList<TmxObjectGroup> ObjectGroups { get; private set; }
		public TmxList<TmxImageLayer> ImageLayers { get; private set; }
		public PropertyDict Properties { get; private set; }

		public TmxMap(Stream inputStream, Func<string, TmxTileset> tilesetLoader, Func<string, Texture2D> textureLoader)
		{
			Load(XDocument.Load(inputStream), tilesetLoader, textureLoader);
		}

		private void Load(XDocument xDoc, Func<string, TmxTileset> tilesetLoader, Func<string, Texture2D> textureLoader)
		{
			var xMap = xDoc.Element("map");
			Version = (string) xMap.Attribute("version");

			Width = (int) xMap.Attribute("width");
			Height = (int) xMap.Attribute("height");
			TileWidth = (int) xMap.Attribute("tilewidth");
			TileHeight = (int) xMap.Attribute("tileheight");
			HexSideLength = (int?) xMap.Attribute("hexsidelength");

			// Map orientation type
			var orientDict = new Dictionary<string, OrientationType>
			{
				{"unknown", OrientationType.Unknown},
				{"orthogonal", OrientationType.Orthogonal},
				{"isometric", OrientationType.Isometric},
				{"staggered", OrientationType.Staggered},
				{"hexagonal", OrientationType.Hexagonal},
			};

			var orientValue = (string) xMap.Attribute("orientation");
			if (orientValue != null)
				Orientation = orientDict[orientValue];

			// Hexagonal stagger axis
			var staggerAxisDict = new Dictionary<string, StaggerAxisType>
			{
				{"x", StaggerAxisType.X},
				{"y", StaggerAxisType.Y},
			};

			var staggerAxisValue = (string) xMap.Attribute("staggeraxis");
			if (staggerAxisValue != null)
				StaggerAxis = staggerAxisDict[staggerAxisValue];

			// Hexagonal stagger index
			var staggerIndexDict = new Dictionary<string, StaggerIndexType>
			{
				{"odd", StaggerIndexType.Odd},
				{"even", StaggerIndexType.Even},
			};

			var staggerIndexValue = (string) xMap.Attribute("staggerindex");
			if (staggerIndexValue != null)
				StaggerIndex = staggerIndexDict[staggerIndexValue];

			// Tile render order
			var renderDict = new Dictionary<string, RenderOrderType>
			{
				{"right-down", RenderOrderType.RightDown},
				{"right-up", RenderOrderType.RightUp},
				{"left-down", RenderOrderType.LeftDown},
				{"left-up", RenderOrderType.LeftUp}
			};

			var renderValue = (string) xMap.Attribute("renderorder");
			if (renderValue != null)
				RenderOrder = renderDict[renderValue];

			NextObjectID = (int?) xMap.Attribute("nextobjectid");

			var xBackgroundColor = xMap.Attribute("backgroundcolor");
			if (xBackgroundColor != null)
			{
				BackgroundColor = new TmxColor(xBackgroundColor);
			}

			Properties = new PropertyDict(xMap.Element("properties"));

			Tilesets = new List<TmxTileset>();
			foreach (var e in xMap.Elements("tileset"))
			{
				var source = (string)e.Attribute("source");
				var tileset = source != null ? tilesetLoader(source) : new TmxTileset(e, textureLoader);

				var xFirstGid = e.Attribute("firstgid");
				if (xFirstGid != null)
				{
					tileset.FirstGid = (int)xFirstGid;
				}

				Tilesets.Add(tileset);
			}

			Layers = new TmxList<TmxLayer>();
			foreach (var e in xMap.Elements("layer"))
				Layers.Add(new TmxLayer(e, Width, Height));

			ObjectGroups = new TmxList<TmxObjectGroup>();
			foreach (var e in xMap.Elements("objectgroup"))
				ObjectGroups.Add(new TmxObjectGroup(e));

			ImageLayers = new TmxList<TmxImageLayer>();
			foreach (var e in xMap.Elements("imagelayer"))
				ImageLayers.Add(new TmxImageLayer(e, textureLoader));
		}

		public TmxTilesetTile FindTilesetTile(int gid, out TmxTileset tileset)
		{
			tileset = null;

			TmxTilesetTile result = null;
			foreach (var ts in Tilesets)
			{
				if (ts.Tiles.TryGetValue(gid, out result))
				{
					tileset = ts;
					break;
				}
			}

			return result;
		}

		public TileMap CreateTileModel()
		{
			if (Orientation != OrientationType.Orthogonal)
			{
				throw new Exception("Only orthogonal maps could be rendered");
			}

			var result = new TileMap();
			if (Tilesets.Count <= 0)
			{
				return result;
			}

			foreach (var sourceLayer in Layers)
			{
				var offsetX = 0;
				if (sourceLayer.OffsetX.HasValue)
				{
					offsetX = -(int)(sourceLayer.OffsetX.Value*TileWidth);
				}

				var offsetY = 0;
				if (sourceLayer.OffsetY.HasValue)
				{
					offsetY = -(int)(sourceLayer.OffsetY.Value * TileHeight);
				}

				var layer = new TileLayer(new Point(sourceLayer.Width, sourceLayer.Height), new Point(TileWidth, TileHeight))
				{
					Offset = new Point(offsetX, offsetY),
					Opacity = sourceLayer.Opacity
				};

				for (var i = 0; i < sourceLayer.Tiles.Count; ++i)
				{
					var sourceTile = sourceLayer.Tiles[i];
					if (sourceTile.Gid <= 0)
					{
						continue;
					}

					var gid = sourceTile.Gid;

					TmxTileset tileset = null;
					foreach (var ts in Tilesets)
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
						continue;
					}

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

					var tile = new Tile
					{
						Region = new TextureRegion2D(tileset.Image.Texture, sourceRect),
						Size = new Point(tileset.TileWidth, tileset.TileHeight),
						Offset = new Point(tileset.TileOffset.X, tileset.TileOffset.Y)
					};

					var tiles = layer.GetTilesAt(sourceTile.X, sourceTile.Y);
					tiles.Add(tile);
				}

				result.Layers.Add(layer);
			}

			return result;
		}
	}

	public enum OrientationType
	{
		Unknown,
		Orthogonal,
		Isometric,
		Staggered,
		Hexagonal
	}

	public enum StaggerAxisType
	{
		X,
		Y
	}

	public enum StaggerIndexType
	{
		Odd,
		Even
	}

	public enum RenderOrderType
	{
		RightDown,
		RightUp,
		LeftDown,
		LeftUp
	}
}