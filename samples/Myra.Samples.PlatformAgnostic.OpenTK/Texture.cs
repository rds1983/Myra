using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;

namespace Myra.Samples.AllWidgets
{
	public unsafe class Texture : IDisposable
	{
		private readonly int _handle;

		public readonly int Width;
		public readonly int Height;

		public Texture(int width, int height)
		{
			Width = width;
			Height = height;

			_handle = GL.GenTexture();
			GLUtility.CheckError();
			Bind();

			//Reserve enough memory from the gpu for the whole image
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
			GLUtility.CheckError();

			SetParameters();
		}

		private void SetParameters()
		{
			//Setting some texture perameters so the texture behaves as expected.
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GLUtility.CheckError();

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			GLUtility.CheckError();

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GLUtility.CheckError();

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GLUtility.CheckError();

			//Generating mipmaps.
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			GLUtility.CheckError();
		}

		public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
		{
			//When we bind a texture we can choose which textureslot we can bind it to.
			GL.ActiveTexture(textureSlot);
			GLUtility.CheckError();

			GL.BindTexture(TextureTarget.Texture2D, _handle);
			GLUtility.CheckError();
		}

		public void Dispose()
		{
			//In order to dispose we need to delete the opengl handle for the texure.
			GL.DeleteTexture(_handle);
			GLUtility.CheckError();
		}

		public void SetData(Rectangle bounds, byte[] data)
		{
			Bind();
			fixed (byte* ptr = data)
			{
				GL.TexSubImage2D(
					target: TextureTarget.Texture2D,
					level: 0,
					xoffset: bounds.Left,
					yoffset: bounds.Top,
					width: bounds.Width,
					height: bounds.Height,
					format: PixelFormat.Rgba,
					type: PixelType.UnsignedByte,
					pixels: (IntPtr)ptr
				);
				GLUtility.CheckError();
			}
		}
	}
}
