using OpenTK.Graphics.OpenGL4;
using System;

namespace Myra.Samples.AllWidgets
{
	public class VertexArrayObject: IDisposable
	{
		private readonly int _handle;
		private readonly int _stride;

		public VertexArrayObject(int stride)
		{
			if (stride <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(stride));
			}

			_stride = stride;

			_handle = GL.GenVertexArray();
			GLUtility.CheckError();
		}

		public void Dispose()
		{
			GL.DeleteVertexArray(_handle);
			GLUtility.CheckError();
		}

		public void Bind()
		{
			GL.BindVertexArray(_handle);
			GLUtility.CheckError();
		}

		public unsafe void VertexAttribPointer(int location, int size, VertexAttribPointerType type, bool normalized, int offset)
		{
			GL.EnableVertexAttribArray(location);
			GL.VertexAttribPointer(location, size, type, normalized, _stride, offset);
			GLUtility.CheckError();
		}
	}
}
