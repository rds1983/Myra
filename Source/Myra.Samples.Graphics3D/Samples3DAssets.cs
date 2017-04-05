using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Assets;

namespace Myra.Samples.Graphics3D
{
	internal class Samples3DAssets
	{
		private static Texture2D _sampleTexture1, _sampleTexture2;

		private static readonly AssetManager _defaultAssetManager =
			new AssetManager(new ResourceAssetResolver(typeof (DefaultAssets).GetTypeInfo().Assembly, "Myra.Samples.Graphics3D."));

		static Samples3DAssets()
		{
			MyraEnvironment.GameDisposed += (sender, args) =>
			{
				Dispose();
			};
		}

		public static Texture2D SampleTexture1
		{
			get { return _sampleTexture1 ?? (_sampleTexture1 = _defaultAssetManager.Load<Texture2D>("chair.png")); }
		}

		public static Texture2D SampleTexture2
		{
			get { return _sampleTexture2 ?? (_sampleTexture2 = _defaultAssetManager.Load<Texture2D>("vase.png")); }
		}

		internal static void Dispose()
		{
			_defaultAssetManager.ClearCache();

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