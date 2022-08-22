using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.IO;
using Myra.Assets;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
#endif

namespace Myra
{
	public static class DefaultAssets
	{
		private static AssetManager _assetManager;
		private static TextureRegionAtlas _uiTextureRegionAtlas;
		private static Stylesheet _uiStylesheet;
		private static TextureRegion _whiteRegion;

#if MONOGAME || FNA || STRIDE
		private static Texture2D _whiteTexture;
#else
		private static object _whiteTexture;
#endif

		private static AssetManager AssetManager
		{
			get
			{
				if (_assetManager == null)
				{
					_assetManager = new AssetManager(new ResourceAssetResolver(typeof(DefaultAssets).Assembly, "Resources."));
				}

				return _assetManager;
			}
		}

#if MONOGAME || FNA || STRIDE
		public static Texture2D WhiteTexture
#else
		public static object WhiteTexture
#endif
		{
			get
			{
				if (_whiteTexture == null)
				{
#if MONOGAME || FNA || STRIDE
					var texture = CrossEngineStuff.CreateTexture(MyraEnvironment.GraphicsDevice, 1, 1);
					CrossEngineStuff.SetTextureData(texture, new Rectangle(0, 0, 1, 1), new byte[] { 255, 255, 255, 255 });
#else
					var texture = MyraEnvironment.Platform.CreateTexture(1, 1);
					MyraEnvironment.Platform.SetTextureData(texture, new Rectangle(0, 0, 1, 1), new byte[] { 255, 255, 255, 255 });
#endif

					_whiteTexture = texture;
				}

				return _whiteTexture;
			}
		}

		public static TextureRegion WhiteRegion
		{
			get
			{
				if (_whiteRegion == null)
				{
					_whiteRegion = UITextureRegionAtlas["white"];
				}

				return _whiteRegion;
			}
		}

		public static TextureRegionAtlas UITextureRegionAtlas
		{
			get
			{
				if (_uiTextureRegionAtlas != null)
				{
					return _uiTextureRegionAtlas;
				}

				_uiTextureRegionAtlas = AssetManager.Load<TextureRegionAtlas>("default_ui_skin.xmat");
				return _uiTextureRegionAtlas;
			}
		}

		public static Stylesheet UIStylesheet
		{
			get
			{
				if (_uiStylesheet != null)
				{
					return _uiStylesheet;
				}

				_uiStylesheet = AssetManager.Load<Stylesheet>("default_ui_skin.xmms");
				return _uiStylesheet;
			}
		}

		public static Stream OpenDefaultFontDataStream()
		{
			var assembly = typeof(DefaultAssets).Assembly;
			return assembly.OpenResourceStream("Myra.Resources.Inter-Regular.ttf");
		}

		internal static void Dispose()
		{	
			_uiTextureRegionAtlas = null;
			_uiStylesheet = null;

			if (_assetManager != null)
			{
				_assetManager.ClearCache();
				_assetManager = null;
			}

			_whiteRegion = null;
			_whiteTexture = null;
		}
	}
}