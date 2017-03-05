using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Edit;
using Myra.Graphics;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics3D.Terrain
{
	public class TerrainObject : BaseObject
	{
		public const int TileSize = 256;

		private bool _ready;

		private HeightMap _heightMap;
		private int _blockWidth = 4;
		private int _blockHeight = 4;
		private int _width;
		private int _height;

		private readonly MultiCompileEffect _effect;
		private readonly List<TerrainTile> _visibleTiles = new List<TerrainTile>();
		private readonly List<Texture2D> _textureLayers = new List<Texture2D>();

		internal QuadTreeNode TopNode { get; set; }
		internal int[] _blockIndexes;

		public HeightMap HeightMap
		{
			get
			{
				return _heightMap;
			}

			set
			{
				if (value == _heightMap)
				{
					return;
				}

				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_heightMap = value;

				if (Width == 0)
				{
					Width = HeightMap.Columns - 1;
				}

				if (Height == 0)
				{
					Height = HeightMap.Rows - 1;
				}

				Invalidate();
			}
		}

		public int BlockWidth
		{
			get
			{
				return _blockWidth;
			}

			set
			{
				if (value == _blockWidth)
				{
					return;
				}

				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_blockWidth = value;
				Invalidate();
			}
		}


		public int BlockHeight
		{
			get
			{
				return _blockHeight;
			}

			set
			{
				if (value == _blockHeight)
				{
					return;
				}

				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_blockHeight = value;
				Invalidate();
			}
		}

		public int Width
		{
			get
			{
				return _width;
			}

			set
			{
				if (value == _width)
				{
					return;
				}

				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_width = value;
				Invalidate();
			}
		}


		public int Height
		{
			get
			{
				return _height;
			}

			set
			{
				if (value == _height)
				{
					return;
				}

				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_height = value;
				Invalidate();
			}
		}

		public bool Ready
		{
			get
			{
				return _ready;
			}
		}

		private int _tilesProcessed;
		private int _tilesCount;

		[HiddenInEditor]
		[JsonIgnore]
		public IndexBuffer IndexBuffer { get; internal set; }

		public Color Color { get; set; }

		public Texture2D Blend { get; set; }

		public int LayersCount
		{
			get { return _textureLayers.Count; }
		}

		public override BoundingBox BoundingBox
		{
			get { return TopNode.BoundingBox; }
		}

		public Action<ProgressInfo> ProgressReporter { get; set; }

		public TerrainObject()
		{
			_effect = DefaultAssets.TerrainEffect;
			Color = Color.White;
		}

		public void Invalidate()
		{
			_ready = false;
		}

		public Effect GetEffect(bool hasLight, bool hasTexture)
		{
			var defines = new List<string>();

			if (hasLight)
			{
				defines.Add("LIGHTNING");
			}

			if (hasTexture)
			{
				defines.Add("TEXTURE");
			}

			return _effect.GetEffect(defines.ToArray());
		}

		internal void TileProcessed()
		{
			++_tilesProcessed;

			if (_tilesCount == 0)
			{
				return;
			}

			var a = ProgressReporter;
			if (a != null)
			{
				a(new ProgressInfo
				{
					Finished = false,
					Progress = (float) _tilesProcessed/_tilesCount
				});
			}
		}

		public void Update()
		{
			if (_ready)
			{
				return;
			}

			// Calculate number of tiles
			var tilesX = Width/TileSize;
			if (Width%TileSize > 0)
			{
				++tilesX;
			}

			var tilesY = Height/TileSize;
			if (Height%TileSize > 0)
			{
				++tilesY;
			}

			_tilesCount = tilesX*tilesY;
			_tilesProcessed = 0;

			// Building quad tree
			TopNode = new QuadTreeNode(this, new Rectangle(-Width/2, -Height/2, Width, Height));
			TopNode.Build();

			var a = ProgressReporter;
			if (a != null)
			{
				a(new ProgressInfo
				{
					Finished = true,
					Progress = 1.0f
				});
			}

			_ready = true;
		}

		public float GetHeightAt(float x, float z)
		{
			var c = (x + Width/2)/BlockWidth;
			var d = (Height/2 - z)/BlockHeight;

			var col = (int) c;
			var row = (int) d;

			var topLeftHeight = HeightMap.GetHeightAt(col, row);
			var topRightHeight = HeightMap.GetHeightAt(col + 1, row);
			var bottomLeftHeight = HeightMap.GetHeightAt(col, row + 1);
			var bottomRightHeight = HeightMap.GetHeightAt(col + 1, row + 1);

			var s = c - col;
			var t = d - row;

			float result;
			if (t < 1 - s)
			{
				// Upper triangle
				var uy = topRightHeight - topLeftHeight;
				var vy = bottomLeftHeight - topLeftHeight;

				result = topLeftHeight + s*uy + t*vy;
			}
			else
			{
				// Lower triangle
				var uy = bottomLeftHeight - bottomRightHeight;
				var vy = topRightHeight - bottomRightHeight;

				result = bottomRightHeight + (1 - s)*uy + (1 - t)*vy;
			}

			return result;
		}

		public Vector3 GetPointAt(float x, float z)
		{
			return new Vector3(x, GetHeightAt(x, z), z);
		}

		public Vector3 CalculateNormal(Vector3 pv, Vector3 p1, Vector3 p2)
		{
			var e0 = p1 - pv;
			var e1 = p2 - pv;

			var n = Vector3.Cross(e1, e0);
			return n;
		}


		public VertexPositionNormalTexture GetVertexAt(float x, float z)
		{
			// We need to calculate all surrounding vertexes to determine normal
			// 0-1-2
			// | | |
			// 3-*-4
			// | | |
			// 5-6-7
			Vector3 p1, p2, p3, p4, p5, p6, pv;

			p1 = GetPointAt(x, z - BlockHeight);
			p2 = GetPointAt(x + BlockWidth, z - BlockHeight);
			p3 = GetPointAt(x - BlockWidth, z);
			p4 = GetPointAt(x + BlockWidth, z);
			p5 = GetPointAt(x - BlockWidth, z + BlockHeight);
			p6 = GetPointAt(x, z + BlockHeight);
			pv = GetPointAt(x, z);

			var sn = Vector3.Zero;

			sn += CalculateNormal(p3, p1, pv);
			sn += CalculateNormal(p1, p2, pv);
			sn += CalculateNormal(pv, p2, p4);
			sn += CalculateNormal(p3, pv, p5);
			sn += CalculateNormal(p5, pv, p6);
			sn += CalculateNormal(pv, p4, p6);

			sn.Normalize();

			var v = new VertexPositionNormalTexture
			{
				Position = pv,
				Normal = sn
			};

			return v;
		}

		public void AddTextureLayer(Texture2D texture)
		{
			if (_textureLayers.Count == 5)
			{
				throw new Exception("Maximum amount of texture layers had been reached.");
			}

			_textureLayers.Add(texture);
		}

		public override void Render(RenderContext context)
		{
			if (!_ready)
			{
				throw new Exception("Terrain is not ready. Call the Update method and make sure it finishes execution.");
			}

			// Build list of visible tiles
			_visibleTiles.Clear();

			var boundingFrustrum = new BoundingFrustum(context.Camera.ViewProjection);

			TopNode.AddVisibleTiles(boundingFrustrum, _visibleTiles);

			var hasLight = context.Lights != null && context.Lights.Length > 0;

			var effect = GetEffect(hasLight, _textureLayers.Count > 0);

			var worldViewProj = context.Camera.ViewProjection;

			effect.Parameters["_worldViewProj"].SetValue(worldViewProj);
			effect.Parameters["_diffuseColor"].SetValue(Color.ToVector4());

			if (hasLight)
			{
				effect.Parameters["_dirToSun"].SetValue(-context.Lights[0].Direction);
			}

			var device = MyraEnvironment.GraphicsDevice;

			if (_textureLayers.Count > 0)
			{
				effect.Parameters["_scale"].SetValue((float)BlockWidth);

				int i;
				for (i = 0; i < _textureLayers.Count; ++i)
				{
					effect.Parameters["_textureLayer" + i].SetValue(_textureLayers[i]);
				}

				for (; i <= 4; ++i)
				{
					effect.Parameters["_textureLayer" + i].SetValue(DefaultAssets.Transparent);
				}

				effect.Parameters["_textureBlendMap"].SetValue(Blend ?? DefaultAssets.Transparent);

				for (i = 0; i <= 5; ++i)
				{
					device.SamplerStates[i] = SamplerState.LinearWrap;
				}
			}

			device.Indices = IndexBuffer;
			var primitiveCount = Mesh.CalculatePrimitiveCount(PrimitiveType.TriangleList, device.Indices.IndexCount);
			foreach (var t in effect.Techniques)
			{
				foreach (var p in t.Passes)
				{
					p.Apply();

					foreach (var tile in _visibleTiles)
					{
						device.SetVertexBuffer(tile.VertexBuffer);
						device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 0, 0, primitiveCount);
					}
				}
			}

			if ((context.Flags & RenderFlags.DrawNormals) == RenderFlags.None) return;

			foreach (var tile in _visibleTiles)
			{
				var mesh = tile.NormalsMesh;
				device.SetVertexBuffer(mesh.VertexBuffer);
				device.Indices = mesh.IndexBuffer;

				var basicEffect = DefaultAssets.DefaultEffect.GetDefaultEffect();

				basicEffect.Parameters["_worldViewProj"].SetValue(worldViewProj);
				basicEffect.Parameters["_diffuseColor"].SetValue(NormalsColor);

				basicEffect.Techniques[0].Passes[0].Apply();

				device.DrawIndexedPrimitives(mesh.PrimitiveType, 0, 0, 0, 0, mesh.PrimitiveCount);
			}
		}
	}
}