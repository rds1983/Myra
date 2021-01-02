using XNAssets;
using Myra.Graphics2D.TextureAtlases;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Assets
{
	internal class TextureRegionLoader : IAssetLoader<TextureRegion>
	{
		public TextureRegion Load(AssetLoaderContext context, string assetName)
		{
			if (assetName.Contains(":"))
			{
				// First part is texture region atlas name
				// Second part is texture region name
				var parts = assetName.Split(':');
				var textureRegionAtlas = context.Load<TextureRegionAtlas>(parts[0]);
				return textureRegionAtlas[parts[1]];
			}

			// Ordinary texture
			var texture = context.Load<Texture2DWrapper>(assetName);
			return new TextureRegion(texture.Texture, new Rectangle(0, 0, texture.Width, texture.Height));
		}
	}
}