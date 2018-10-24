using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

namespace Myra
{
	public static class DefaultAssets
	{
		private const string DefaultFontName = "default_font.fnt";
		private const string DefaultSmallFontName = "default_font_small.fnt";
		private const string DefaultStylesheetName = "default_ui_skin.json";
		private const string DefaultAtlasName = "default_ui_skin_atlas.json";
		private const string DefaultAtlasImageName = "default_ui_skin_atlas.png";

		private static readonly ResourceAssetResolver _assetResolver = new ResourceAssetResolver(
			typeof(DefaultAssets).GetTypeInfo().Assembly,
			"Myra.Resources.");

		private static SpriteFont _font;
		private static SpriteFont _fontSmall;
		private static TextureRegionAtlas _uiSpritesheet;
		private static Stylesheet _uiStylesheet;
		private static Texture2D _uiBitmap;
		private static RasterizerState _uiRasterizerState;
		private static Texture2D _white;
		private static TextureRegion _whiteRegion;

		public static Texture2D White
		{
			get
			{
				if (_white == null)
				{
					_white = new Texture2D(MyraEnvironment.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
					_white.SetData(new[] {Color.White});
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
					_whiteRegion = new TextureRegion(White);
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

				_font = SpriteFontHelper.LoadFromFnt(_assetResolver.ReadAsString(DefaultFontName),
					UISpritesheet.Regions["default"]);

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

				_fontSmall = SpriteFontHelper.LoadFromFnt(
					_assetResolver.ReadAsString(DefaultSmallFontName),
					UISpritesheet.Regions["font-small"]);

				return _fontSmall;
			}
		}

		public static TextureRegionAtlas UISpritesheet
		{
			get
			{
				if (_uiSpritesheet != null) return _uiSpritesheet;

				_uiSpritesheet = TextureRegionAtlas.FromJson(_assetResolver.ReadAsString(DefaultAtlasName), UIBitmap);

				return _uiSpritesheet;
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

				_uiStylesheet = Stylesheet.CreateFromSource(_assetResolver.ReadAsString(DefaultStylesheetName),
					s => string.IsNullOrEmpty(s) ? null : UISpritesheet.Regions[s],
					f => f == "default-font" ? Font : FontSmall);

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

				var rawImage = ColorBuffer.FromStream(_assetResolver.Open(DefaultAtlasImageName));
				rawImage.PremultiplyAlpha();
				_uiBitmap = rawImage.CreateTexture2D();

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
			_font = null;
			_fontSmall = null;
			_uiSpritesheet = null;
			_uiStylesheet = null;
			Stylesheet.Current = null;

			_whiteRegion = null;
			if (_white != null)
			{
				_white.Dispose();
				_white = null;
			}
		
			if (_uiBitmap != null)
			{
				_uiBitmap.Dispose();
				_uiBitmap = null;
			}		

			if (_uiRasterizerState != null)
			{
				_uiRasterizerState.Dispose();
				_uiRasterizerState = null;
			}
		}
	}
}