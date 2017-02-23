using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Myra.Graphics3D.Terrain
{
	internal class QuadTreeNode
	{
		private const int MinimumSize = 256;

		private const int TopLeft = 0;
		private const int TopRight = 1;
		private const int BottomLeft = 2;
		private const int BottomRight = 3;

		private readonly TerrainObject _terrain;
		private readonly QuadTreeNode[] _subNodes = new QuadTreeNode[4];

		public Rectangle Bounds { get; private set; }
		public TerrainTile Leaf { get; private set; }

		public BoundingBox BoundingBox { get; private set; }

		public QuadTreeNode(TerrainObject terrain, Rectangle bounds)
		{
			if (terrain == null)
			{
				throw new ArgumentNullException("terrain");
			}

			_terrain = terrain;
			Bounds = bounds;
		}

		public void Build()
		{
			if (Bounds.Width > MinimumSize)
			{
				// Not leaf
				var width = Bounds.Width/2;
				var height = Bounds.Height/2;

				_subNodes[TopLeft] = new QuadTreeNode(_terrain, new Rectangle(Bounds.X, Bounds.Y + height, width, height));
				_subNodes[TopLeft].Build();

				_subNodes[TopRight] = new QuadTreeNode(_terrain, new Rectangle(Bounds.X + width, Bounds.Y + height, width, height));
				_subNodes[TopRight].Build();

				_subNodes[BottomLeft] = new QuadTreeNode(_terrain, new Rectangle(Bounds.X, Bounds.Y, width, height));
				_subNodes[BottomLeft].Build();

				_subNodes[BottomRight] = new QuadTreeNode(_terrain, new Rectangle(Bounds.X + width, Bounds.Y, width, height));
				_subNodes[BottomRight].Build();

				BoundingBox = BoundingBox.CreateMerged(_subNodes[TopLeft].BoundingBox, _subNodes[TopRight].BoundingBox);
				BoundingBox = BoundingBox.CreateMerged(BoundingBox, _subNodes[BottomLeft].BoundingBox);
				BoundingBox = BoundingBox.CreateMerged(BoundingBox, _subNodes[BottomRight].BoundingBox);
			}
			else
			{
				Leaf = new TerrainTile(_terrain, Bounds);
				Leaf.Build();
				BoundingBox = Leaf.BoundingBox;
			}
		}

		public void AddVisibleTiles(BoundingFrustum boundingFrustum, List<TerrainTile> visibleTiles)
		{
			if (!boundingFrustum.Intersects(BoundingBox))
			{
				return;
			}

			if (Leaf == null)
			{
				foreach (var subNode in _subNodes)
				{
					subNode.AddVisibleTiles(boundingFrustum, visibleTiles);
				}
			}
			else
			{
				visibleTiles.Add(Leaf);
			}
		}

	}
}
