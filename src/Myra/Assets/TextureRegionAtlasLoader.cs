using Myra.Graphics2D.TextureAtlases;

#if MONOGAME || FNA
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
#endif

namespace Myra.Assets
{
	internal class TextureRegionAtlasLoader : IAssetLoader<TextureRegionAtlas>
	{
		public TextureRegionAtlas Load(AssetLoaderContext context, string assetName)
		{
			var data = context.Load<string>(assetName);
#if MONOGAME || FNA || STRIDE
			return TextureRegionAtlas.Load(data, name => context.Load<Texture2D>(name));
#else
			return TextureRegionAtlas.Load(data, name => context.Load<Texture2DWrapper>(name).Texture);
#endif
		}
	}
}