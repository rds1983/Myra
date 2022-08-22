using StbImageSharp;
using System.IO;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
#endif

namespace Myra.Assets
{
#if PLATFORM_AGNOSTIC
	internal class Texture2DWrapper
	{
		public int Width { get; private set; }
		public int Height { get; private set; }
		public object Texture { get; private set; }

		public Texture2DWrapper(int width, int height, object texture)
		{
			Width = width;
			Height = height;
			Texture = texture;
		}
	}
#endif

#if MONOGAME || FNA || STRIDE
	internal class Texture2DLoader : IAssetLoader<Texture2D>
#else
	internal class Texture2DLoader : IAssetLoader<Texture2DWrapper>
#endif
	{
#if MONOGAME || FNA || STRIDE
		public Texture2D Load(AssetLoaderContext context, string assetName)
#else
		public Texture2DWrapper Load(AssetLoaderContext context, string assetName)
#endif
		{
			ImageResult result = null;
			using (var stream = context.Open(assetName))
			{
				if (stream.CanSeek)
				{
					result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
				}
				else
				{
					// If stream doesnt provide seek functionaly, use MemoryStream instead
					using (var ms = new MemoryStream())
					{
						stream.CopyTo(ms);
						ms.Seek(0, SeekOrigin.Begin);
						result = ImageResult.FromStream(ms, ColorComponents.RedGreenBlueAlpha);
					}
				}
			}

			// Premultiply Alpha
			var b = result.Data;
			for (var i = 0; i < result.Data.Length; i += 4)
			{
				var falpha = b[i + 3] / 255.0f;
				b[i] = (byte)(b[i] * falpha);
				b[i + 1] = (byte)(b[i + 1] * falpha);
				b[i + 2] = (byte)(b[i + 2] * falpha);
			}

#if MONOGAME || FNA || STRIDE
			var texture = CrossEngineStuff.CreateTexture(MyraEnvironment.GraphicsDevice, result.Width, result.Height);
			CrossEngineStuff.SetTextureData(texture, new Rectangle(0, 0, result.Width, result.Height), result.Data);
			return texture;
#else
			var texture = MyraEnvironment.Platform.CreateTexture(result.Width, result.Height);
			MyraEnvironment.Platform.SetTextureData(texture, new Rectangle(0, 0, result.Width, result.Height), result.Data);
			return new Texture2DWrapper(result.Width, result.Height, texture);
#endif
		}
	}
}