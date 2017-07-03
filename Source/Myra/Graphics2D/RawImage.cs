using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.StbSharp;

namespace Myra.Graphics2D
{
	public class RawImage
	{
		private readonly byte[] _data;

		public int Width { get; private set; }
		public int Height { get; private set; }

		public byte[] Data
		{
			get { return _data; }
		}

		public RawImage(int width, int height)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}

			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}

			Width = width;
			Height = height;

			_data = new byte[width*height*4];
		}

		public RawImage(int width, int height, byte[] data)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}

			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}

			if (data == null)
			{
				throw new ArgumentNullException("data");
			}

			var length = width*height*4;
			if (data.Length != length)
			{
				throw new ArgumentException(string.Format("Inconsistent data length: expected={0}, provided={1}", length, data.Length));
			}

			Width = width;
			Height = height;
			_data = data;
		}

		private static byte ApplyAlpha(byte color, byte alpha)
		{
			var fc = color / 255.0f;
			var fa = alpha / 255.0f;

			var fr = (int)(255.0f * fc * fa);

			if (fr < 0)
			{
				fr = 0;
			}

			if (fr > 255)
			{
				fr = 255;
			}

			return (byte)fr;
		}

		public void Process(bool premultiplyAlpha, Color? transColor = null)
		{
			var data = Data;
			for (var i = 0; i < Width * Height; ++i)
			{
				if (transColor.HasValue)
				{
					if (data[i * 4] == transColor.Value.R &&
						data[i * 4 + 1] == transColor.Value.G &&
						data[i * 4 + 2] == transColor.Value.B)
					{
						data[i *  4 + 3] = 0;
					}
				}

				if (premultiplyAlpha)
				{
					var a = Data[i*4 + 3];

					Data[i*4] = ApplyAlpha(Data[i*4], a);
					Data[i*4 + 1] = ApplyAlpha(Data[i*4 + 1], a);
					Data[i*4 + 2] = ApplyAlpha(Data[i*4 + 2], a);
				}
			}
		}

		public Texture2D CreateTexture2D()
		{
			var texture = new Texture2D(MyraEnvironment.GraphicsDevice, Width, Height, false, SurfaceFormat.Color);
			texture.SetData(_data);
			return texture;
		}

		/// <summary>
		/// Loads an image in PNG, JPG, BMP, GIF, TGA or PSD format from the stream
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static RawImage FromStream(Stream stream)
		{
			var reader = new ImageReader();

			int x, y;
			int comp;
			var data = reader.Read(stream, out x, out y, out comp, Stb.STBI_rgb_alpha);

			return new RawImage(x, y, data);
		}
	}
}
