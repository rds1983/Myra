using Microsoft.Xna.Framework;
using Myra.Attributes;

namespace Myra.Samples.RogueEditor.Data
{
	public class Map : ItemWithId
	{
		private TileInfo[,] _tiles;

		[HiddenInEditor]
		public Point Size
		{
			get
			{
				var result = Point.Zero;
				if (_tiles != null)
				{
					result.X = _tiles.GetLength(0);
					result.Y = _tiles.GetLength(1);
				}
				return result;
			}

			set
			{
				_tiles = new TileInfo[value.X, value.Y];
				for (var x = 0; x < value.X; ++x)
				{
					for (var y = 0; y < value.Y; ++y)
					{
						_tiles[x, y] = new TileInfo();
					}
				}
			}
		}

		[HiddenInEditor]
		public TileInfo[,] Tiles
		{
			get
			{
				return _tiles;
			}
		}

		public TileInfo GetTileAt(int x, int y)
		{
			return _tiles[x, y];
		}

		public TileInfo GetTileAt(Point pos)
		{
			return _tiles[pos.X, pos.Y];
		}

		public TileInfo GetTileAt(Vector2 pos)
		{
			return _tiles[(int)pos.X, (int)pos.Y];
		}

		public void SetTileAt(Point pos, TileInfo tile)
		{
			_tiles[pos.X, pos.Y] = tile;
		}
	}
}
