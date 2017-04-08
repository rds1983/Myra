using System;
using Microsoft.Xna.Framework.Graphics;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Assets
{
	public class Texture2DLoader : IAssetLoader<Texture2D>
	{
		internal class Texture2DLoadingParameters
		{
			public string image { get; set; }
			public bool premultiplyAlpha { get; set; }
		}

		public Texture2D Load(AssetLoaderContext context, string assetName)
		{
			var premultiplyAlpha = true;

			if (assetName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
			{
				var text = context.ReadAsText(assetName);
				var parameters = JsonConvert.DeserializeObject<Texture2DLoadingParameters>(text);
				assetName = parameters.image;
				premultiplyAlpha = parameters.premultiplyAlpha;
			}

			Texture2D texture;
			using (var stream = context.Open(assetName))
			{
				texture = GraphicsExtension.FromStream(stream, premultiplyAlpha);
			}

			return texture;
		}
	}
}