using FontStashSharp.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;

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
			var texture2d = new Texture2D(Device, width, height);

			return texture2d;
		}

		public void SetTextureData(object texture, Rectangle bounds, byte[] data)
		{
			var xnaTexture = (Texture2D)texture;
			xnaTexture.SetData(0, bounds.ToXNA(), data, 0, bounds.Width * bounds.Height * 4);
		}

		public Point GetTextureSize(object texture)
		{
			var xnaTexture = (Texture2D)texture;
			return new Point(xnaTexture.Width, xnaTexture.Height);
		}
	}
}
