using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics3D.Terrain
{
	internal class TerrainTile
	{
		private readonly TerrainObject _terrain;
		private VertexPositionNormalTexture[] _vertexes;
		private Mesh _normalsMesh;

		public Rectangle Bounds { get; private set; }
		public VertexBuffer VertexBuffer { get; private set; }
		public BoundingBox BoundingBox { get; private set; }

		public Mesh NormalsMesh
		{
			get
			{
				if (_normalsMesh != null)
				{
					return _normalsMesh;
				}

				_normalsMesh = new Mesh();

				var vertexes = new List<VertexPositionNormalTexture>();
				var indices = new List<int>();
				var i = 0;
				foreach (var v in _vertexes)
				{
					vertexes.Add(v);

					var v2 = new VertexPositionNormalTexture(v.Position + v.Normal * 4, v.Normal, v.TextureCoordinate);
					vertexes.Add(v2);

					indices.Add(i++);
					indices.Add(i++);
				}

				_normalsMesh.Init(vertexes.ToArray(), indices.ToArray(), PrimitiveType.LineList);

				return _normalsMesh;
			}
		}


		public TerrainTile(TerrainObject terrain, Rectangle bounds)
		{
			if (terrain == null)
			{
				throw new ArgumentNullException("terrain");
			}

			_terrain = terrain;
			Bounds = bounds;
		}

		internal void Build()
		{
			var heightSet = false;
			float MinHeight = 0, MaxHeight = 0;

			var vertexes = new List<VertexPositionNormalTexture>();
			var vertCols = 0;
			var vertRows = 0;
			var topBounds = _terrain.TopNode.Bounds;
			for (float z = Bounds.Top; z <= Bounds.Bottom; z += _terrain.BlockHeight)
			{
				vertCols = 0;
				for (float x = Bounds.Left; x <= Bounds.Right; x += _terrain.BlockWidth)
				{
					var vertex = _terrain.GetVertexAt(x, z);
					float height = vertex.Position.Y;
					if (!heightSet)
					{
						MinHeight = MaxHeight = height;
						heightSet = true;
					}
					else
					{
						if (height < MinHeight)
						{
							MinHeight = height;
						}

						if (height > MaxHeight)
						{
							MaxHeight = height;
						}
					}

					vertex.TextureCoordinate.X = (vertex.Position.X - topBounds.X)/topBounds.Width;
					vertex.TextureCoordinate.Y = (vertex.Position.Z - topBounds.Y)/topBounds.Height;
					vertexes.Add(vertex);
					++vertCols;
				}

				++vertRows;
			}

			int cellCols = vertCols - 1, cellRows = vertRows - 1;
			if (_terrain._blockIndexes == null)
			{
				_terrain._blockIndexes = new int[cellCols*cellRows*6];
				var indexes = _terrain._blockIndexes;
				var k = 0;
				for (var row = 0; row < cellRows; ++row)
				{
					for (var col = 0; col < cellCols; ++col)
					{
						var topLeft = row*vertCols + col;
						var topRight = row*vertCols + col + 1;
						var bottomLeft = (row + 1)*vertCols + col;
						var bottomRight = (row + 1)*vertCols + col + 1;

						indexes[k] = topLeft;
						indexes[k + 1] = topRight;
						indexes[k + 2] = bottomLeft;
						indexes[k + 3] = bottomLeft;
						indexes[k + 4] = topRight;
						indexes[k + 5] = bottomRight;
						k += 6;
					}
				}

				_terrain.IndexBuffer = new IndexBuffer(MyraEnvironment.GraphicsDevice, IndexElementSize.ThirtyTwoBits,
					indexes.Length,
					BufferUsage.WriteOnly);
				_terrain.IndexBuffer.SetData(indexes);
			}

			VertexBuffer = new VertexBuffer(MyraEnvironment.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration,
				vertexes.Count,
				BufferUsage.WriteOnly);
			_vertexes = vertexes.ToArray();
			VertexBuffer.SetData(_vertexes);
			BoundingBox = new BoundingBox(new Vector3(Bounds.X, MinHeight, Bounds.Y),
				new Vector3(Bounds.Right, MaxHeight, Bounds.Bottom));
		}
	}
}
