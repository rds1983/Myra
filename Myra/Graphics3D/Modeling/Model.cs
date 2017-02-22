using Microsoft.Xna.Framework.Graphics;
using Myra.Edit;
using Myra.Graphics3D.Materials;
using Newtonsoft.Json;

namespace Myra.Graphics3D.Modeling
{
	public class Model
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
		public VertexDeclaration VertexDeclaration { get; private set; }

		[HiddenInEditor]
		[JsonIgnore]
		public VertexBuffer VertexBuffer { get; private set; }

		[HiddenInEditor]
		[JsonIgnore]
		public IndexBuffer IndexBuffer { get; private set; }

		public void Init<T>(GraphicsDevice device, T[] vertices, int[] indices, PrimitiveType primitiveType)
			where T : struct, IVertexType
		{
			VertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length,
				BufferUsage.WriteOnly);
			VertexBuffer.SetData(vertices);

			IndexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
			IndexBuffer.SetData(indices);

			PrimitiveType = primitiveType;
			VertexDeclaration = new T().VertexDeclaration;
			PrimitiveCount = CalculatePrimitiveCount(PrimitiveType, indices.Length);
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
