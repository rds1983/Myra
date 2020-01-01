using Microsoft.Xna.Framework.Graphics;
using StbImageSharp;
using System.IO;

namespace Myra.Utility
{
	public static class Texture2DExtensions
	{
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

		/// <summary>
		/// Creates a Texture2D from Stream and optionally premultiplies alpha
		/// </summary>
		/// <param name="graphicsDevice"></param>
		/// <param name="stream"></param>
		/// <param name="premultiplyAlpha"></param>
		/// <returns></returns>
		public static unsafe Texture2D FromStream(GraphicsDevice graphicsDevice, Stream stream, bool premultiplyAlpha)
		{
			byte[] bytes;

			// Rewind stream if it is at end
			if (stream.CanSeek && stream.Length == stream.Position)
			{
				stream.Seek(0, SeekOrigin.Begin);
			}

			// Copy it's data to memory
			// As some platforms dont provide full stream functionality and thus streams can't be read as it is
			using (var ms = new MemoryStream())
			{
				stream.CopyTo(ms);
				bytes = ms.ToArray();
			}

			// The data returned is always four channel BGRA
			var result = ImageResult.FromMemory(bytes, ColorComponents.RedGreenBlueAlpha);

			if (premultiplyAlpha)
			{
				fixed (byte* b = &result.Data[0])
				{
					for (var i = 0; i < result.Data.Length; i += 4)
					{
						var a = b[i + 3];
						b[i] = ApplyAlpha(b[i], a);
						b[i + 1] = ApplyAlpha(b[i + 1], a);
						b[i + 2] = ApplyAlpha(b[i + 2], a);
					}
				}
			}

			Texture2D texture = null;
			texture = new Texture2D(graphicsDevice, result.Width, result.Height);
			texture.SetData(result.Data);

			return texture;
		}
	}
}
