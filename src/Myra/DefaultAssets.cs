using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using AssetManagementBase;
using Myra.Assets;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
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
		private static object _uiBitmap;
		private static object _whiteTexture;
		private static TextureRegion _whiteRegion;

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

		public static object WhiteTexture
		{
			get
			{
				if (_whiteTexture == null)
				{
					var texture = MyraEnvironment.Platform.CreateTexture(1, 1);
					MyraEnvironment.Platform.SetTextureData(texture, new Rectangle(0, 0, 1, 1), new byte[] { 255, 255, 255, 255 });

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

				_uiTextureRegionAtlas = AssetManager.Load<TextureRegionAtlas>("default_ui_skin.atlas");
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

		public static object UIBitmap
		{
			get
			{
				if (_uiBitmap != null)
				{
					return _uiBitmap;
				}

				var wrapper = AssetManager.Load<Texture2DWrapper>("default_ui_skin_atlas.png");
				_uiBitmap = wrapper.Texture;
				return _uiBitmap;
			}
		}

		internal static void Dispose()
		{	
			_uiTextureRegionAtlas = null;
			_uiStylesheet = null;
			Stylesheet.Current = null;

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