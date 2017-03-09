using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StbSharp;

namespace Myra.Samples.Graphics3D
{
	internal class Samples3DAssets
	{
		private static Texture2D _sampleTexture1, _sampleTexture2;

		static Samples3DAssets()
		{
			MyraEnvironment.GameDisposed += (sender, args) =>
			{
				Dispose();
			};
		}

		public static Texture2D SampleTexture1
		{
			get { return _sampleTexture1 ?? (_sampleTexture1 = Texture2DFromResources("chair.png")); }
		}

		public static Texture2D SampleTexture2
		{
			get { return _sampleTexture2 ?? (_sampleTexture2 = Texture2DFromResources("vase.png")); }
		}

		private static Texture2D Texture2DFromResources(string path)
		{
			byte[] buffer;
			using (var ms = new MemoryStream())
			{
				using (var stream = Resources.Resources.GetBinaryResourceStream(path))
				{
					stream.CopyTo(ms);
				}

				buffer = ms.ToArray();
			}

			int x, y, comp;
			var data = Image.stbi_load_from_memory(buffer, out x, out y, out comp, Image.STBI_rgb_alpha);

			var result = new Texture2D(MyraEnvironment.GraphicsDevice, x, y);
			result.SetData(data);

			return result;

			// return GraphicsExtension.PremultipliedTextureFromPngStream(File.OpenRead(path));
		}

		internal static void Dispose()
		{
			if (_sampleTexture1 != null)
			{
				_sampleTexture1.Dispose();
				_sampleTexture1 = null;
			}

			if (_sampleTexture2 != null)
			{
				_sampleTexture2.Dispose();
				_sampleTexture2 = null;
			}
		}
	}
}
