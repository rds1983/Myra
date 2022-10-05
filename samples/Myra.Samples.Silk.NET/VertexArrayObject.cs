using Silk.NET.OpenGL;
using System;

namespace Myra.Samples.AllWidgets
{
	public class VertexArrayObject: IDisposable
	{
		private readonly uint _handle;
		private readonly int _stride;

		public VertexArrayObject(int stride)
		{
			if (stride <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(stride));
			}

			_stride = stride;

			Env.Gl.GenVertexArrays(1, out _handle);
			GLUtility.CheckError();
		}

		public void Dispose()
		{
			Env.Gl.DeleteVertexArray(_handle);
			GLUtility.CheckError();
		}

		public void Bind()
		{
			Env.Gl.BindVertexArray(_handle);
			GLUtility.CheckError();
		}

		public unsafe void VertexAttribPointer(int location, int size, VertexAttribPointerType type, bool normalized, int offset)
		{
			Env.Gl.EnableVertexAttribArray((uint)location);
			Env.Gl.VertexAttribPointer((uint)location, size, type, normalized, (uint)_stride, (void*)offset);
			GLUtility.CheckError();
		}
	}
}
