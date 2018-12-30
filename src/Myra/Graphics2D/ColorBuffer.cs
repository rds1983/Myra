using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.StbSharp;

namespace Myra.Graphics2D
{
	/// <summary>
	/// Utility class for loading 32-bit RGBA images in JPG, PNG, BMP, TGA, PSD and GIF formats. Also it can perform simple post-processing.
	/// </summary>
	public class ColorBuffer
	{
		private readonly Color[] _data;

		public int Width { get; private set; }
		public int Height { get; private set; }

		public Color[] Data
		{
			get { return _data; }
		}

		public Color this[int x, int y]
		{
			get
			{
				return _data[y * Width + x];
			}

			set
			{
				_data[y * Width + x] = value;
			}
		}

		public ColorBuffer(int width, int height)
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

			_data = new Color[width * height];
		}

		public ColorBuffer(int width, int height, Color[] data)
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

			var length = width * height;
			if (data.Length != length)
			{
				throw new ArgumentException(string.Format("Inconsistent data length: expected={0}, provided={1}", length,
					data.Length));
			}

			Width = width;
			Height = height;
			_data = data;
		}

		private static byte ApplyAlpha(byte color, byte alpha)
		{
			var fc = color / 255.0f;
			var fa = alpha / 255.0f;

			var fr = (int) (255.0f * fc * fa);

			if (fr < 0)
			{
				fr = 0;
			}

			if (fr > 255)
			{
				fr = 255;
			}

			return (byte) fr;
		}

		public void PremultiplyAlpha()
		{
			var data = Data;
			for (var i = 0; i < Width * Height; ++i)
			{
				var a = Data[i].A;

				Data[i].R = ApplyAlpha(Data[i].R, a);
				Data[i].G = ApplyAlpha(Data[i].G, a);
				Data[i].B = ApplyAlpha(Data[i].B, a);
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
		public static ColorBuffer FromStream(Stream stream)
		{
			var reader = new ImageReader();

			int x, y;
			int comp;
			var data = reader.Read(stream, out x, out y, out comp);

			return new ColorBuffer(x, y, data);
		}
	}
}