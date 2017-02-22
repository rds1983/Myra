using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Myra.Utility;
using StbSharp;

namespace Myra.Samples
{
	public static class SampleAssets
	{
		private static Texture2D _sampleTexture1, _sampleTexture2;

		public static Texture2D SampleTexture1
		{
			get { return _sampleTexture1 ?? (_sampleTexture1 = Texture2DFromFile("Assets/chair.png")); }
		}

		public static Texture2D SampleTexture2
		{
			get { return _sampleTexture2 ?? (_sampleTexture2 = Texture2DFromFile("Assets/vase.png")); }
		}

		private static Texture2D Texture2DFromFile(string path)
		{
			var buffer = File.ReadAllBytes(path);

			int x, y, comp;
			var data = Image.stbi_load_from_memory(buffer, out x, out y, out comp, Image.STBI_rgb_alpha);

			var result = new Texture2D(MyraEnvironment.GraphicsDevice, x, y);
			result.SetData(data);

			return result;

			// return GraphicsExtension.PremultipliedTextureFromPngStream(File.OpenRead(path));
		}
	}
}
