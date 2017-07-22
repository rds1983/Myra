using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using Myra.Content.TextureAtlases;
using Myra.Graphics2D;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

namespace Myra
{
	public static class DefaultAssets
	{
		private const string DefaultFontName = "default_font.fnt";
		private const string DefaultSmallFontName = "default_font_small.fnt";
		private const string DefaultStylesheetName = "default_stylesheet.json";
		private const string DefaultSpritesheetName = "default_uiskin.atlas";

		private static readonly ResourceAssetResolver _assetResolver = new ResourceAssetResolver(
			typeof(DefaultAssets).GetTypeInfo().Assembly,
			"Myra.Resources.");

		private static BitmapFont _font;
		private static BitmapFont _fontSmall;
		private static TextureAtlas _uiSpritesheet;
		private static Stylesheet _uiStylesheet;
		private static Texture2D _uiBitmap;
		private static RasterizerState _uiRasterizerState;
		private static Texture2D _white;
		private static TextureRegion2D _whiteRegion;

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

		public static TextureRegion2D WhiteRegion
		{
			get
			{
				if (_whiteRegion == null)
				{
					_whiteRegion = new TextureRegion2D(White);
				}

				return _whiteRegion;
			}
		}

		public static BitmapFont Font
		{
			get
			{
				if (_font != null)
				{
					return _font;
				}

				_font = BitmapFontHelper.LoadFromFnt(DefaultFontName,
					_assetResolver.ReadAsString(DefaultFontName),
					s => UISpritesheet.GetRegion("default"));

				return _font;
			}
		}

		public static BitmapFont FontSmall
		{
			get
			{
				if (_fontSmall != null)
				{
					return _fontSmall;
				}

				_fontSmall = BitmapFontHelper.LoadFromFnt(DefaultSmallFontName,
					_assetResolver.ReadAsString(DefaultSmallFontName),
					s => UISpritesheet.GetRegion("font-small"));

				return _fontSmall;
			}
		}

		public static TextureAtlas UISpritesheet
		{
			get
			{
				if (_uiSpritesheet != null) return _uiSpritesheet;

				var content =
					TextureAtlasContentLoader.Load(_assetResolver.ReadAsString(DefaultSpritesheetName));

				_uiSpritesheet = content.Create(UiBitmap);
				
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
					s => string.IsNullOrEmpty(s) ? null : UISpritesheet.GetRegion(s),
					f => f == "default-font" ? Font : FontSmall);

				return _uiStylesheet;
			}
		}

		public static Texture2D UiBitmap
		{
			get
			{
				if (_uiBitmap != null)
				{
					return _uiBitmap;
				}

				var rawImage = RawImage.FromStream(_assetResolver.Open("default_uiskin.png"));
				rawImage.Process(true);
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