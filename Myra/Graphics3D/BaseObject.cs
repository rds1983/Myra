using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Myra.Graphics3D
{
	public abstract class BaseObject
	{
		public static readonly Vector4 NormalsColor = Color.Coral.ToVector4();

		public string Id { get; set; }

		public abstract BoundingBox BoundingBox { get; }

		public abstract void Render(RenderContext context);

		public static Mesh CreateNormalsVesh(IEnumerable<VertexPositionNormalTexture> vertices, float size = 4.0f)
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
	}
}
