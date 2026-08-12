using FontStashSharp.Interfaces;
using Silk.NET.OpenGL;
using System;
using System.Drawing;
using System.Numerics;

namespace Myra.Samples.AllWidgets
{
	internal class QuadBatch: IDisposable
	{
		private const int MAX_SPRITES = 2048;
		private const int MAX_VERTICES = MAX_SPRITES * 4;
		private const int MAX_INDICES = MAX_SPRITES * 6;

		private readonly Shader _shader;
		private readonly BufferObject<VertexPositionColorTexture> _vertexBuffer;
		private readonly BufferObject<short> _indexBuffer;
		private readonly VertexArrayObject _vao;
		private readonly VertexPositionColorTexture[] _vertexData = new VertexPositionColorTexture[MAX_VERTICES];
		private object _lastTexture;
		private int _vertexIndex = 0;

		private static readonly short[] indexData = GenerateIndexArray();

		public Rectangle Viewport { get; set; }

		public unsafe QuadBatch()
		{
			_vertexBuffer = new BufferObject<VertexPositionColorTexture>(MAX_VERTICES, BufferTargetARB.ArrayBuffer, true);
			_indexBuffer = new BufferObject<short>(indexData.Length, BufferTargetARB.ElementArrayBuffer, false);
			_indexBuffer.SetData(indexData, 0, indexData.Length);

			_shader = new Shader("shader.vert", "shader.frag");
			_shader.Use();

			_vao = new VertexArrayObject(sizeof(VertexPositionColorTexture));
			_vao.Bind();

			var location = _shader.GetAttribLocation("a_position");
			_vao.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 0);

			location = _shader.GetAttribLocation("a_color");
			_vao.VertexAttribPointer(location, 4, VertexAttribPointerType.UnsignedByte, true, 12);

			location = _shader.GetAttribLocation("a_texCoords0");
			_vao.VertexAttribPointer(location, 2, VertexAttribPointerType.Float, false, 16);
		}

		~QuadBatch() => Dispose(false);

		public void Dispose() => Dispose(true);

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}

			_vao.Dispose();
			_vertexBuffer.Dispose();
			_indexBuffer.Dispose();
			_shader.Dispose();
		}

		public void Begin()
		{
			Env.Gl.Disable(EnableCap.DepthTest);
			GLUtility.CheckError();
			Env.Gl.Enable(EnableCap.Blend);
			GLUtility.CheckError();
			Env.Gl.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
			GLUtility.CheckError();

			_shader.Use();
			_shader.SetUniform("TextureSampler", 0);

			var transform = Matrix4x4.CreateOrthographicOffCenter(Viewport.Left, Viewport.Right, Viewport.Bottom, Viewport.Top, 0, -1);
			_shader.SetUniform("MatrixTransform", transform);

			_vao.Bind();
			_indexBuffer.Bind();
			_vertexBuffer.Bind();
		}

		public void DrawQuad(object texture, ref VertexPositionColorTexture topLeft, ref VertexPositionColorTexture topRight, ref VertexPositionColorTexture bottomLeft, ref VertexPositionColorTexture bottomRight)
		{
			if (_lastTexture != texture)
			{
				FlushBuffer();
			}

			_vertexData[_vertexIndex++] = topLeft;
			_vertexData[_vertexIndex++] = topRight;
			_vertexData[_vertexIndex++] = bottomLeft;
			_vertexData[_vertexIndex++] = bottomRight;

			_lastTexture = texture;
		}

		public void End()
		{
			FlushBuffer();
		}

		private unsafe void FlushBuffer()
		{
			if (_vertexIndex == 0 || _lastTexture == null)
			{
				return;
			}

			_vertexBuffer.SetData(_vertexData, 0, _vertexIndex);

			var texture = (Texture)_lastTexture;
			texture.Bind();

			Env.Gl.DrawElements(PrimitiveType.Triangles, (uint)(_vertexIndex * 6 / 4), DrawElementsType.UnsignedShort, null);
			_vertexIndex = 0;
		}

		private static short[] GenerateIndexArray()
		{
			short[] result = new short[MAX_INDICES];
			for (int i = 0, j = 0; i < MAX_INDICES; i += 6, j += 4)
			{
				result[i] = (short)(j);
				result[i + 1] = (short)(j + 1);
				result[i + 2] = (short)(j + 2);
				result[i + 3] = (short)(j + 3);
				result[i + 4] = (short)(j + 2);
				result[i + 5] = (short)(j + 1);
			}
			return result;
		}
	}
}
