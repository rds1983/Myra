using Microsoft.Xna.Framework.Graphics;

namespace Myra.Assets
{
	internal class Texture2DLoader : IAssetLoader<Texture2D>
	{
		public Texture2D Load(AssetLoaderContext context, string assetName)
		{
			using (var stream = context.Open(assetName))
			{
				return Texture2D.FromStream(MyraEnvironment.GraphicsDevice, stream);
			}
		}
	}
}