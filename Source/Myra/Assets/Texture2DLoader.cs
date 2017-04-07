using System;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.StbSharp;
using Newtonsoft.Json;

namespace Myra.Assets
{
	internal class Texture2DLoader : IAssetLoader<Texture2D>
	{
		internal class Texture2DLoadingParameters
		{
			public string image { get; set; }
			public bool premultiplyAlpha { get; set; }
		}

		private static byte ApplyAlpha(byte color, byte alpha)
		{
			var fc = color/255.0f;
			var fa = alpha/255.0f;

			var fr = (int) (255.0f*fc*fa);

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

		public Texture2D Load(AssetLoaderContext context, string assetName)
		{
			var premultiplyAlpha = false;

			if (assetName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
			{
				var text = context.ReadAsText(assetName);
				var parameters = JsonConvert.DeserializeObject<Texture2DLoadingParameters>(text);
				assetName = parameters.image;
				premultiplyAlpha = parameters.premultiplyAlpha;
			}

			var reader = new ImageReader();

			int x, y;
			byte[] data;
			using (var stream = context.Open(assetName))
			{
				int comp;
				data = reader.Read(stream, out x, out y, out comp, Stb.STBI_rgb_alpha);
			}

			if (premultiplyAlpha)
			{
				// Premultiply alpha
				for (var i = 0; i < x*y; ++i)
				{
					var a = data[i*4 + 3];

					data[i*4] = ApplyAlpha(data[i*4], a);
					data[i*4 + 1] = ApplyAlpha(data[i*4 + 1], a);
					data[i*4 + 2] = ApplyAlpha(data[i*4 + 2], a);
				}
			}

			var texture = new Texture2D(MyraEnvironment.GraphicsDevice, x, y, false, SurfaceFormat.Color);
			texture.SetData(data);

			return texture;
		}
	}
}