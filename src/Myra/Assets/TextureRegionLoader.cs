using AssetManagementBase;
using Myra.Graphics2D.TextureAtlases;

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
#if MONOGAME || FNA || STRIDE
			var texture = context.Load<Texture2D>(assetName);
			return new TextureRegion(texture, new Rectangle(0, 0, texture.Width, texture.Height));
#else
			var texture = context.Load<Texture2DWrapper>(assetName);
			return new TextureRegion(texture.Texture, new Rectangle(0, 0, texture.Width, texture.Height));
#endif
		}
	}
}