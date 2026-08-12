using Myra.Graphics2D.UI.Styles;
using AssetManagementBase;
using Myra.Graphics2D.TextureAtlases;
using FontStashSharp;

namespace Myra
{
	/// <summary>
	/// Provides access to the default stylesheets and assets included with Myra.
	/// </summary>
	public static class DefaultAssets
	{
		private static AssetManager _assetManager;
		private static Stylesheet _defaultStylesheet, _defaultStylesheet2x;
		private static TextureRegion _whiteRegion;

		private static AssetManager AssetManager
		{
			get
			{
				if (_assetManager == null)
				{
					_assetManager = AssetManager.CreateResourceAssetManager(typeof(DefaultAssets).Assembly, "Resources.");
				}

				return _assetManager;
			}
		}

		/// <summary>
		/// Gets the default stylesheet for UI widgets at normal scale.
		/// </summary>
		public static Stylesheet DefaultStylesheet
		{
			get
			{
				if (_defaultStylesheet != null)
				{
					return _defaultStylesheet;
				}

				_defaultStylesheet = AssetManager.LoadStylesheet("default_ui_skin.xmms");
				return _defaultStylesheet;
			}
		}

		/// <summary>
		/// Gets the default stylesheet for UI widgets at 2x scale.
		/// </summary>
		public static Stylesheet DefaultStylesheet2X
		{
			get
			{
				if (_defaultStylesheet2x != null)
				{
					return _defaultStylesheet2x;
				}

				_defaultStylesheet2x = AssetManager.LoadStylesheet("default_ui_skin_2x.xmms");
				return _defaultStylesheet2x;
			}
		}

		/// <summary>
		/// Gets a default white texture region used for placeholder graphics and fills.
		/// </summary>
		public static TextureRegion WhiteRegion
		{
			get
			{
				if (_whiteRegion == null)
				{
#if !PLATFORM_AGNOSTIC
					_whiteRegion = new TextureRegion(SpriteFontBase.GetWhite(MyraEnvironment.GraphicsDevice));
#else
					_whiteRegion = new TextureRegion(SpriteFontBase.GetWhite(MyraEnvironment.Platform.Renderer.TextureManager));
#endif
				}

				return _whiteRegion;
			}
		}

		internal static void Dispose()
		{
			_defaultStylesheet = null;
			_defaultStylesheet2x = null;

			if (_assetManager != null)
			{
				_assetManager.Cache.Clear();
				_assetManager = null;
			}
		}
	}
}