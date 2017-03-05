using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Edit;
using Myra.Graphics3D.Materials;
using Newtonsoft.Json;

namespace Myra.Graphics3D
{
	public class Mesh
	{
		[HiddenInEditor]
		[JsonIgnore]
		public PrimitiveType PrimitiveType { get; private set; }

		[HiddenInEditor]
		[JsonIgnore]
		public int PrimitiveCount { get; private set; }

		[HiddenInEditor]
		public BaseMaterial Material { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public VertexBuffer VertexBuffer { get; private set; }

		[HiddenInEditor]
		[JsonIgnore]
		public IndexBuffer IndexBuffer { get; private set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Vector3 MinimumBound { get; private set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Vector3 MaximumBound { get; private set; }

		public Mesh(VertexPositionNormalTexture[] vertices, int[] indices, PrimitiveType primitiveType)
		{
			for (var i = 0; i < vertices.Length; ++i)
			{
				vertices[i].Normal.Normalize();
			}

			VertexBuffer = new VertexBuffer(MyraEnvironment.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertices.Length,
				BufferUsage.WriteOnly);
			VertexBuffer.SetData(vertices);

			IndexBuffer = new IndexBuffer(MyraEnvironment.GraphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
			IndexBuffer.SetData(indices);

			PrimitiveType = primitiveType;
			PrimitiveCount = CalculatePrimitiveCount(PrimitiveType, indices.Length);

			var first = true;
			float minX = 0, minY = 0, minZ = 0, maxX = 0, maxY = 0, maxZ = 0;
			foreach (var t in vertices)
			{
				if (first)
				{
					minX = maxX = t.Position.X;
					minY = maxY = t.Position.Y;
					minZ = maxZ = t.Position.Z;
				}
				else
				{
					if (t.Position.X < minX)
					{
						minX = t.Position.X;
					}

					if (t.Position.Y < minY)
					{
						minY = t.Position.Y;
					}

					if (t.Position.Z < minZ)
					{
						minZ = t.Position.Z;
					}

					if (t.Position.X > maxX)
					{
						maxX = t.Position.X;
					}

					if (t.Position.Y > maxY)
					{
						maxY = t.Position.Y;
					}

					if (t.Position.Z > maxZ)
					{
						maxZ = t.Position.Z;
					}
				}

				first = false;
			}

			MinimumBound = new Vector3(minX, minY, minZ);
			MaximumBound = new Vector3(maxX, maxY, maxZ);
		}

		public static int CalculatePrimitiveCount(PrimitiveType primitiveType, int indices)
		{
			var result = 0;

			switch (primitiveType)
			{
				case PrimitiveType.TriangleList:
					result = indices/3;
					break;
				case PrimitiveType.LineList:
					result = indices/2;
					break;
			}

			return result;
		}
	}
}
