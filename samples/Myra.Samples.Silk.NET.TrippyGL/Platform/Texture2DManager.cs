using FontStashSharp.Interfaces;
using Silk.NET.OpenGL;
using System;
using System.Drawing;
using TrippyGL;

namespace Myra.Samples.AllWidgets
{
	internal class Texture2DManager : ITexture2DManager
	{
		public GraphicsDevice Device { get; private set; }

		public Texture2DManager(GraphicsDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}

			Device = device;
		}

		public object CreateTexture(int width, int height)
		{
			var texture2d = new Texture2D(Device, (uint)width, (uint)height);

			return texture2d;
		}

		public void SetTextureData(object texture, Rectangle bounds, byte[] data)
		{
			var xnaTexture = (Texture2D)texture;

			xnaTexture.SetData<byte>(data, bounds.X, bounds.Y, (uint)bounds.Width, (uint)bounds.Height, PixelFormat.Rgba);
		}

		public Point GetTextureSize(object texture)
		{
			var xnaTexture = (Texture2D)texture;

			return new Point((int)xnaTexture.Width, (int)xnaTexture.Height);
		}
	}
}
