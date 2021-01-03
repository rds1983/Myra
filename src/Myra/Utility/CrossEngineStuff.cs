using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Graphics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
#endif

namespace Myra.Utility
{
	internal static class CrossEngineStuff
	{
		public static Point ViewSize
		{
			get
			{
#if MONOGAME || FNA
				var device = MyraEnvironment.GraphicsDevice;
				return new Point(device.Viewport.Width, _device.Viewport.Height);
#elif STRIDE
				var device = MyraEnvironment.GraphicsDevice;
				return new Point(device.Presenter.BackBuffer.Width, device.Presenter.BackBuffer.Height);
#else
				return MyraEnvironment.Platform.ViewSize;
#endif
			}
		}

		public static Color CreateColor(int r, int g, int b, int a = 255)
		{
#if MONOGAME || FNA || STRIDE
			return new Color((byte)r, (byte)g, (byte)b, (byte)a);
#else
			return Color.FromArgb(a, r, g, b);
#endif
		}

		public static Color MultiplyColor(Color color, float value)
		{
#if MONOGAME || FNA || STRIDE
			return color * value;
#else
			if (value < 0)
			{
				value = 0;
			}

			if (value > 1)
			{
				value = 1;
			}

			return Color.FromArgb((int)(color.A * value),
				(int)(color.R * value),
				(int)(color.G * value),
				(int)(color.B * value));
#endif
		}

#if MONOGAME || FNA || STRIDE
		public static Texture2D CreateTexture(GraphicsDevice device, int width, int height)
		{
#if MONOGAME || FNA
			var texture2d = new Texture2D(device, width, height);
#elif STRIDE
			var texture2d = Texture2D.New2D(device, width, height, false, PixelFormat.R8G8B8A8_UNorm_SRgb, TextureFlags.ShaderResource);
#endif

			return texture2d;
		}

		public static void SetTextureData(Texture2D texture, Rectangle bounds, byte[] data)
		{
#if MONOGAME || FNA
			texture.SetData(0, bounds, data, 0, bounds.Width * bounds.Height * 4);
#elif STRIDE
			var size = bounds.Width * bounds.Height * 4;
			byte[] temp;
			if (size == data.Length)
			{
				temp = data;
			}
			else
			{
				// Since Stride requres buffer size to match exactly, copy data in the temporary buffer
				temp = new byte[bounds.Width * bounds.Height * 4];
				Array.Copy(data, temp, temp.Length);
			}

			var context = MyraEnvironment.Game.GraphicsContext;
			texture.SetData(context.CommandList, temp, 0, 0, new ResourceRegion(bounds.Left, bounds.Top, 0, bounds.Right, bounds.Bottom, 1));
#endif
		}
#endif
	}
}
