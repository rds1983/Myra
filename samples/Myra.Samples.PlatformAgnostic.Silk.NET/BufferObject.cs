using Silk.NET.OpenGL;
using System;
using System.Runtime.InteropServices;

namespace Myra.Samples.AllWidgets
{
	public class BufferObject<T> : IDisposable where T : unmanaged
	{
		private readonly uint _handle;
		private readonly BufferTargetARB _bufferType;
		private readonly int _size;

		public unsafe BufferObject(int size, BufferTargetARB bufferType, bool isDynamic)
		{
			_bufferType = bufferType;
			_size = size;

			_handle = Env.Gl.GenBuffer();
			GLUtility.CheckError();
			
			Bind();

			var elementSizeInBytes = Marshal.SizeOf<T>();
			Env.Gl.BufferData(bufferType, (nuint)(size * elementSizeInBytes), null, isDynamic ? BufferUsageARB.StreamDraw : BufferUsageARB.StaticDraw);
			GLUtility.CheckError();
		}

		public void Bind()
		{
			Env.Gl.BindBuffer(_bufferType, _handle);
			GLUtility.CheckError();
		}

		public void Dispose()
		{
			Env.Gl.DeleteBuffer(_handle);
			GLUtility.CheckError();
		}

		public unsafe void SetData(T[] data, int startIndex, int elementCount)
		{
			Bind();

			fixed(T* dataPtr = &data[startIndex])
			{
				var elementSizeInBytes = sizeof(T);

				Env.Gl.BufferSubData(_bufferType, 0, (nuint)(elementCount * elementSizeInBytes), dataPtr);
				GLUtility.CheckError();
			}
		}
	}
}