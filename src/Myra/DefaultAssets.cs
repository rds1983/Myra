using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using XNAssets;

#if !STRIDE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Stride.Core.Mathematics;
using Texture2D = Stride.Graphics.Texture;
using RasterizerState = Stride.Graphics.RasterizerStateDescription;
#endif

namespace Myra
{
	public static class DefaultAssets
	{
		private static AssetManager _assetManager;
		private static TextureRegionAtlas _uiTextureRegionAtlas;
		private static Stylesheet _uiStylesheet;
		private static Texture2D _uiBitmap;
		private static RasterizerState _uiRasterizerState;
		private static Texture2D _white;
		private static TextureRegion _whiteRegion;

		private static AssetManager AssetManager
		{
			get
			{
				if (_assetManager == null)
				{
					_assetManager = new AssetManager(MyraEnvironment.GraphicsDevice, new ResourceAssetResolver(typeof(DefaultAssets).Assembly, "Resources."));
				}

				return _assetManager;
			}
		}

		public static Texture2D White
		{
			get
			{
				if (_white == null)
				{
					_white = CrossEngineStuff.CreateTexture2D(1, 1);
					CrossEngineStuff.SetData(_white, new[] {Color.White});
				}

				return _white;
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

		public static Texture2D UIBitmap
		{
			get
			{
				if (_uiBitmap != null)
				{
					return _uiBitmap;
				}

				_uiBitmap = AssetManager.Load<Texture2D>("default_ui_skin_atlas.png");
				return _uiBitmap;
			}
		}

		public static RasterizerState UIRasterizerState
		{
			get
			{
				if (_uiRasterizerState != null)
				{
					return _uiRasterizerState;
				}

				_uiRasterizerState = new RasterizerState
				{
					ScissorTestEnable = true
				};
				return _uiRasterizerState;
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
			if (_white != null)
			{
				_white.Dispose();
				_white = null;
			}
		
#if !STRIDE
			if (_uiRasterizerState != null)
			{
				_uiRasterizerState.Dispose();
				_uiRasterizerState = null;
			}
#endif
		}
	}
}