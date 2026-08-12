using OpenTK.Graphics.OpenGL4;
using System;
using System.Runtime.InteropServices;

namespace Myra.Samples.AllWidgets
{
	public class BufferObject<T> : IDisposable where T : unmanaged
	{
		private readonly int _handle;
		private readonly BufferTarget _bufferType;
		private readonly int _size;

		public unsafe BufferObject(int size, BufferTarget bufferType, bool isDynamic)
		{
			_bufferType = bufferType;
			_size = size;

			_handle = GL.GenBuffer();
			GLUtility.CheckError();

			Bind();

			var elementSizeInBytes = Marshal.SizeOf<T>();
			GL.BufferData(bufferType, size * elementSizeInBytes, IntPtr.Zero, isDynamic ? BufferUsageHint.StreamDraw : BufferUsageHint.StaticDraw);
			GLUtility.CheckError();
		}

		public void Bind()
		{
			GL.BindBuffer(_bufferType, _handle);
			GLUtility.CheckError();
		}

		public void Dispose()
		{
			GL.DeleteBuffer(_handle);
			GLUtility.CheckError();
		}

		public unsafe void SetData(T[] data, int startIndex, int elementCount)
		{
			Bind();

			fixed (T* dataPtr = &data[startIndex])
			{
				var elementSizeInBytes = sizeof(T);

				GL.BufferSubData(_bufferType, IntPtr.Zero, elementCount * elementSizeInBytes, (IntPtr)dataPtr);
				GLUtility.CheckError();
			}
		}
	}
}
