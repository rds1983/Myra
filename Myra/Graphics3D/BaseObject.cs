using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Myra.Graphics3D
{
	public abstract class BaseObject
	{
		public static readonly Vector4 NormalsColor = Color.Coral.ToVector4();
		public static readonly Vector4 BoundingBoxesColor = Color.LightGreen.ToVector4();

		public string Id { get; set; }

		public abstract BoundingBox BoundingBox { get; }

		public abstract void Render(RenderContext context);

		public static Mesh CreateNormalsMesh(IEnumerable<VertexPositionNormalTexture> vertices, float size = 4.0f)
		{
			var resultVertices = new List<VertexPositionNormalTexture>();
			var indices = new List<int>();
			var i = 0;
			foreach (var v in vertices)
			{
				resultVertices.Add(v);

				var v2 = new VertexPositionNormalTexture(v.Position + v.Normal * size, v.Normal, v.TextureCoordinate);
				resultVertices.Add(v2);

				indices.Add(i++);
				indices.Add(i++);
			}

			return new Mesh(resultVertices.ToArray(), indices.ToArray(), PrimitiveType.LineList);
		}

		public static Mesh CreateBoundingBoxMesh(BoundingBox bb)
		{
			var resultVertices = new VertexPositionNormalTexture[8];
			var indices = new int[24];

			var min = bb.Min;
			var max = bb.Max;
			resultVertices[0] = new VertexPositionNormalTexture(new Vector3(min.X, min.Y, min.Z), Vector3.Zero, Vector2.Zero);
			resultVertices[1] = new VertexPositionNormalTexture(new Vector3(min.X, min.Y, max.Z), Vector3.Zero, Vector2.Zero);
			resultVertices[2] = new VertexPositionNormalTexture(new Vector3(max.X, min.Y, max.Z), Vector3.Zero, Vector2.Zero);
			resultVertices[3] = new VertexPositionNormalTexture(new Vector3(max.X, min.Y, min.Z), Vector3.Zero, Vector2.Zero);
			resultVertices[4] = new VertexPositionNormalTexture(new Vector3(min.X, max.Y, min.Z), Vector3.Zero, Vector2.Zero);
			resultVertices[5] = new VertexPositionNormalTexture(new Vector3(min.X, max.Y, max.Z), Vector3.Zero, Vector2.Zero);
			resultVertices[6] = new VertexPositionNormalTexture(new Vector3(max.X, max.Y, max.Z), Vector3.Zero, Vector2.Zero);
			resultVertices[7] = new VertexPositionNormalTexture(new Vector3(max.X, max.Y, min.Z), Vector3.Zero, Vector2.Zero);

			indices[0] = 0;
			indices[1] = 1;
			indices[2] = 1;
			indices[3] = 2;
			indices[4] = 2;
			indices[5] = 3;
			indices[6] = 3;
			indices[7] = 0;
			indices[8] = 0;
			indices[9] = 4;
			indices[10] = 1;
			indices[11] = 5;
			indices[12] = 2;
			indices[13] = 6;
			indices[14] = 3;
			indices[15] = 7;
			indices[16] = 4;
			indices[17] = 5;
			indices[18] = 5;
			indices[19] = 6;
			indices[20] = 6;
			indices[21] = 7;
			indices[22] = 7;
			indices[23] = 4;

			return new Mesh(resultVertices, indices, PrimitiveType.LineList);
		}
	}
}
