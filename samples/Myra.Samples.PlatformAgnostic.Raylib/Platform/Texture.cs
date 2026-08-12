using System;
using Raylib_cs;
using Rectangle = System.Drawing.Rectangle;

namespace Myra.Samples.AllWidgets
{
	public class Texture : IDisposable
	{
		public Texture2D RaylibTexture { get; private set; }

		public readonly int Width;
		public readonly int Height;

		public Texture(int width, int height)
		{
			Width = width;
			Height = height;

			// Create an empty image and convert to texture
			Image img = Raylib.GenImageColor(width, height, new Color(0, 0, 0, 0));
			RaylibTexture = Raylib.LoadTextureFromImage(img);
			Raylib.UnloadImage(img);
		}

		public void Bind()
		{
		}

		public void Dispose()
		{
			if (RaylibTexture.Id != 0)
			{
				Raylib.UnloadTexture(RaylibTexture);
			}
		}

		public void SetData(Rectangle bounds, byte[] data)
		{
			if (data == null || data.Length == 0)
				return;

			Raylib.UpdateTextureRec(RaylibTexture, bounds.ToRaylib(), data);
		}
	}
}
