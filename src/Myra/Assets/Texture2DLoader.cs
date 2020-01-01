using Myra.Utility;

#if !XENKO
using Microsoft.Xna.Framework.Graphics;
#else
using Texture2D = Xenko.Graphics.Texture;
#endif

namespace Myra.Assets
{
	internal class Texture2DLoader : IAssetLoader<Texture2D>
	{
		public Texture2D Load(AssetLoaderContext context, string assetName)
		{
			using (var stream = context.Open(assetName))
			{
				return Texture2DExtensions.FromStream(MyraEnvironment.GraphicsDevice, stream, true);
			}
		}
	}
}