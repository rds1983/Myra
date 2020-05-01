using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Reflection;
using XNAssets;

#if !STRIDE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using SpriteFontPlus;
using Stride.Core.Mathematics;
using Stride.Graphics;
using Texture2D = Stride.Graphics.Texture;
using RasterizerState = Stride.Graphics.RasterizerStateDescription;
#endif

namespace Myra
{
	public static class DefaultAssets
	{
		private static readonly AssetManager _assetManager = new AssetManager(MyraEnvironment.GraphicsDevice, new ResourceAssetResolver(typeof(DefaultAssets).Assembly, "Resources."));
		private static SpriteFont _font;
		private static SpriteFont _fontSmall;
		private static TextureRegionAtlas _uiTextureRegionAtlas;
		private static Stylesheet _uiStylesheet;
		private static Texture2D _uiBitmap;
		private static RasterizerState _uiRasterizerState;
		private static Texture2D _white;
		private static TextureRegion _whiteRegion;

		private static Assembly Assembly
		{
			get
			{
				return typeof(DefaultAssets).Assembly;
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

		public static SpriteFont Font
		{
			get
			{
				if (_font != null)
				{
					return _font;
				}

				_font = _assetManager.Load<SpriteFont>("default_font.fnt");
				return _font;
			}
		}

		public static SpriteFont FontSmall
		{
			get
			{
				if (_fontSmall != null)
				{
					return _fontSmall;
				}

				_fontSmall = _assetManager.Load<SpriteFont>("default_font_small.fnt");
				return _fontSmall;
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

				_uiTextureRegionAtlas = _assetManager.Load<TextureRegionAtlas>("default_ui_skin_atlas.xml");
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

				_uiStylesheet = _assetManager.Load<Stylesheet>("default_ui_skin.xml");
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

				_uiBitmap = _assetManager.Load<Texture2D>("default_ui_skin_atlas.png");
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

		static DefaultAssets()
		{
#if STRIDE
			BMFontLoader.GraphicsDevice = MyraEnvironment.GraphicsDevice;
#endif
		}

		internal static void Dispose()
		{	
			_font = null;
			_fontSmall = null;
			_uiTextureRegionAtlas = null;
			_uiStylesheet = null;
			Stylesheet.Current = null;

			_assetManager.ClearCache();

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