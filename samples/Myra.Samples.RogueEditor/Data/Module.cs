using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Myra.Attributes;

namespace Myra.Samples.RogueEditor.Data
{
	public class Module
	{
		private readonly Dictionary<string, TileInfo> _tileInfos = new Dictionary<string, TileInfo>();
		private readonly Dictionary<string, CreatureInfo> _creatureInfos = new Dictionary<string, CreatureInfo>();
		private readonly Dictionary<string, Map> _maps = new Dictionary<string, Map>();

		[HiddenInEditor]
		public Dictionary<string, TileInfo> TileInfos
		{
			get
			{
				return _tileInfos;
			}
		}

		[HiddenInEditor]
		public Dictionary<string, CreatureInfo> CreatureInfos
		{
			get
			{
				return _creatureInfos;
			}
		}

		[HiddenInEditor]
		public Dictionary<string, Map> Maps
		{
			get
			{
				return _maps;
			}
		}

		public string ToJSON()
		{
			return string.Empty;
		}

		public static Module FromJSON(string data)
		{
			return null;
		}

		public void AddTileInfo(string id, Color color, char image, bool passable)
		{
			_tileInfos.Add(id, new TileInfo
			{
				Id = id,
				Color = color,
				Image = image,
				Passable = passable
			});
		}

		public static Module New()
		{
			var result = new Module();

			result.AddTileInfo("grass", Color.DarkGreen, '.', true);
			result.AddTileInfo("highGrass", Color.Green, '.', true);
			result.AddTileInfo("ground", Color.Brown, '.', true);
			result.AddTileInfo("tree", Color.Green, '%', false);
			result.AddTileInfo("hill", Color.Gray, '.', true);
			result.AddTileInfo("mountain", Color.White, '^', false);
			result.AddTileInfo("water", Color.Blue, '.', false);
			result.AddTileInfo("road", Color.Brown, '.', true);
			result.AddTileInfo("floor", Color.Gray, '.', true);
			result.AddTileInfo("plato", Color.White, '.', true);
			result.AddTileInfo("buildingWall", Color.Gray, '#', false);


			var map = new Map
			{
				Id = "map1",
				Size = new Point(256, 256)
			};

			for (var x = 0; x < map.Size.X; ++x)
			{
				for (var y = 0; y < map.Size.Y; ++y)
				{
					map.Tiles[x, y] = result.TileInfos["grass"];
				}
			}

			result.Maps[map.Id] = map;

			return result;
		}
	}
}