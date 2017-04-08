/* Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
 * Licensed under the Apache License, Version 2.0
 * http://www.apache.org/licenses/LICENSE-2.0 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using Myra.Assets;
using Myra.Attributes;

namespace Myra.Graphics2D.Tiled
{
	// TODO: The design here is all wrong. A Tileset should be a list of tiles,
	//       it shouldn't force the user to do so much tile ID management
	[AssetLoader(typeof(TmxTilesetLoader))]
	public class TmxTileset : ITmxElement
	{
		public string Name { get; private set; }
		public int FirstGid { get; set; }
		public int TileWidth { get; private set; }
		public int TileHeight { get; private set; }
		public int Spacing { get; private set; }
		public int Margin { get; private set; }
		public int? Columns { get; private set; }
		public int? TileCount { get; private set; }
		public int TilesPerRow { get; private set; }
		public int Rows { get; private set; }
		public int TotalTiles { get; private set; }

		public Dictionary<int, TmxTilesetTile> Tiles { get; private set; }
		public TmxTileOffset TileOffset { get; private set; }
		public PropertyDict Properties { get; private set; }
		public TmxImage Image { get; private set; }
		public TmxList<TmxTerrain> Terrains { get; private set; }

		public TmxTileset(XContainer xDoc, Func<string, Texture2D> textureLoader) :
			this(xDoc.Element("tileset"), textureLoader)
		{
		}

		// TMX tileset element constructor
		public TmxTileset(XElement xTileset, Func<string, Texture2D> textureLoader)
		{
			Name = (string) xTileset.Attribute("name");
			TileWidth = (int) xTileset.Attribute("tilewidth");
			TileHeight = (int) xTileset.Attribute("tileheight");
			Spacing = (int?) xTileset.Attribute("spacing") ?? 0;
			Margin = (int?) xTileset.Attribute("margin") ?? 0;
			Columns = (int?) xTileset.Attribute("columns");
			TileCount = (int?) xTileset.Attribute("tilecount");
			TileOffset = new TmxTileOffset(xTileset.Element("tileoffset"));

			Image = new TmxImage(xTileset.Element("image"), textureLoader);

			Terrains = new TmxList<TmxTerrain>();
			var xTerrainType = xTileset.Element("terraintypes");
			if (xTerrainType != null)
			{
				foreach (var e in xTerrainType.Elements("terrain"))
					Terrains.Add(new TmxTerrain(e));
			}

			Tiles = new Dictionary<int, TmxTilesetTile>();
			foreach (var xTile in xTileset.Elements("tile"))
			{
				var tile = new TmxTilesetTile(xTile, Terrains, textureLoader);
				Tiles[tile.Id] = tile;
			}

			Properties = new PropertyDict(xTileset.Element("properties"));

			TilesPerRow = Image.Width.Value/TileWidth;
			Rows = Image.Height.Value/TileHeight;
			TotalTiles = TilesPerRow*Rows;

		}
	}

	public class TmxTileOffset
	{
		public int X { get; private set; }
		public int Y { get; private set; }

		public TmxTileOffset(XElement xTileOffset)
		{
			if (xTileOffset == null)
			{
				X = 0;
				Y = 0;
			}
			else
			{
				X = (int) xTileOffset.Attribute("x");
				Y = (int) xTileOffset.Attribute("y");
			}
		}
	}

	public class TmxTerrain : ITmxElement
	{
		public string Name { get; private set; }
		public int Tile { get; private set; }

		public PropertyDict Properties { get; private set; }

		public TmxTerrain(XElement xTerrain)
		{
			Name = (string) xTerrain.Attribute("name");
			Tile = (int) xTerrain.Attribute("tile");
			Properties = new PropertyDict(xTerrain.Element("properties"));
		}
	}

	public class TmxTilesetTile
	{
		public int Id { get; private set; }
		public Collection<TmxTerrain> TerrainEdges { get; private set; }
		public double Probability { get; private set; }

		public PropertyDict Properties { get; private set; }
		public TmxImage Image { get; private set; }
		public TmxList<TmxObjectGroup> ObjectGroups { get; private set; }
		public Collection<TmxAnimationFrame> AnimationFrames { get; private set; }

		// Human-readable aliases to the Terrain markers
		public TmxTerrain TopLeft
		{
			get { return TerrainEdges[0]; }
		}

		public TmxTerrain TopRight
		{
			get { return TerrainEdges[1]; }
		}

		public TmxTerrain BottomLeft
		{
			get { return TerrainEdges[2]; }
		}

		public TmxTerrain BottomRight
		{
			get { return TerrainEdges[3]; }
		}

		public TmxTilesetTile(XElement xTile, TmxList<TmxTerrain> Terrains, Func<string, Texture2D> textureLoader)
		{
			Id = (int) xTile.Attribute("id");

			TerrainEdges = new Collection<TmxTerrain>();

			var strTerrain = (string) xTile.Attribute("terrain") ?? ",,,";
			foreach (var v in strTerrain.Split(','))
			{
				int result;
				var success = int.TryParse(v, out result);
				var edge = success ? Terrains[result] : null;
				TerrainEdges.Add(edge);

				// TODO: Assert that TerrainEdges length is 4
			}

			Probability = (double?) xTile.Attribute("probability") ?? 1.0;
			Image = new TmxImage(xTile.Element("image"), textureLoader);

			ObjectGroups = new TmxList<TmxObjectGroup>();
			foreach (var e in xTile.Elements("objectgroup"))
				ObjectGroups.Add(new TmxObjectGroup(e));

			AnimationFrames = new Collection<TmxAnimationFrame>();
			if (xTile.Element("animation") != null)
			{
				foreach (var e in xTile.Element("animation").Elements("frame"))
					AnimationFrames.Add(new TmxAnimationFrame(e));
			}

			Properties = new PropertyDict(xTile.Element("properties"));
		}
	}

	public class TmxAnimationFrame
	{
		public int Id { get; private set; }
		public int Duration { get; private set; }

		public TmxAnimationFrame(XElement xFrame)
		{
			Id = (int) xFrame.Attribute("tileid");
			Duration = (int) xFrame.Attribute("duration");
		}
	}
}