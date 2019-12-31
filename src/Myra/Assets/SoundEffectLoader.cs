#if !XENKO

using Microsoft.Xna.Framework.Audio;

namespace Myra.Assets
{
	internal class SoundEffectLoader: IAssetLoader<SoundEffect>
	{
		public SoundEffect Load(AssetLoaderContext context, string assetName)
		{
			using (var stream = context.Open(assetName))
			{
				return SoundEffect.FromStream(stream);
			}
		}
	}
}

#endif