using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Myra.Graphics2D.Tiles
{
	public class TileLayer
	{
		private float _opacity = 1.0f;
		private Point _tileSize;
		private readonly Point _size;
		private readonly List<Tile>[,] _tiles;

		public float Opacity
		{
			get
			{
				return _opacity;
			}

			set
			{
				if (value < 0 || value > 1.0f)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_opacity = value;
			}
		}

		public Point Size
		{
			get { return _size; }
		}

		public Point TileSize
		{
			get
			{
				return _tileSize;
			}

			set
			{
				if (value.X == 0 || value.Y == 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_tileSize = value;
			}
		}

		public Point Offset { get; set; }

		public TileLayer(Point size, Point tileSize)
		{
			if (size.X == 0 || size.Y == 0)
			{
				throw new ArgumentOutOfRangeException("size");
			}

			if (tileSize.X == 0 || tileSize.Y == 0)
			{
				throw new ArgumentOutOfRangeException("tileSize");
			}

			_size = size;
			TileSize = tileSize;
			_tiles = new List<Tile>[size.X, size.Y];

			for (var x = 0; x < size.X; ++x)
			{
				for (var y = 0; y < size.Y; ++y)
				{
					_tiles[x, y] = new List<Tile>();
				}
			}
		}

		public List<Tile> GetTilesAt(int x, int y)
		{
			return _tiles[x, y];
		}
	}
}