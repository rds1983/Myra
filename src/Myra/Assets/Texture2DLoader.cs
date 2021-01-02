using StbImageSharp;
using System.IO;
using XNAssets;
using Myra.Graphics2D;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Assets
{
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

	internal class Texture2DLoader : IAssetLoader<Texture2DWrapper>
	{
		private static int PowerOfTwo(int x)
		{
			int power = 1;
			while (power < x)
				power *= 2;

			return power;
		}

		public Texture2DWrapper Load(AssetLoaderContext context, string assetName)
		{
			Stream stream = null;
			try
			{
				stream = context.Open(assetName);
				if (!stream.CanSeek)
				{
					// If stream isn't seekable, use MemoryStream instead
					var ms = new MemoryStream();
					stream.CopyTo(ms);
					ms.Seek(0, SeekOrigin.Begin);
					stream.Dispose();
					stream = ms;
				}

				var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

				var width = PowerOfTwo(image.Width);
				var height = PowerOfTwo(image.Height);

				var texture = MyraEnvironment.Platform.CreateTexture(width, height);
				MyraEnvironment.Platform.SetTextureData(texture, new Rectangle(0, 0, image.Width, image.Height), image.Data);

				return new Texture2DWrapper(width, height, texture);
			}
			finally
			{
				stream.Dispose();
			}
		}
	}
}