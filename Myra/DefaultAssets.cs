using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Assets;
using Myra.Graphics2D;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;

namespace Myra
{
	public static class DefaultAssets
	{
		private const string DefaultFontName = "default_font.fnt";
		private const string DefaultSmallFontName = "default_font_small.fnt";
		private const string DefaultStylesheetName = "default_stylesheet.json";
		private const string DefaultSpritesheetName = "default_uiskin.atlas";

		private static readonly AssetManager _defaultAssetManager =
			new AssetManager(new ResourceAssetResolver(typeof (DefaultAssets).GetTypeInfo().Assembly, "Myra.Resources."));

		private static Texture2D _white;
		private static TextureRegion _whiteRegion;

		private static BitmapFont _font;
		private static BitmapFont _fontSmall;
		private static SpriteSheet _uiSpritesheet;
		private static Stylesheet _uiStylesheet;
		private static RasterizerState _uiRasterizerState;

		public static BitmapFont Font
		{
			get
			{
				if (_font != null)
				{
					return _font;
				}

				_font = _defaultAssetManager.Load<BitmapFont>(DefaultFontName);

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

				_fontSmall = _defaultAssetManager.Load<BitmapFont>(DefaultSmallFontName);

				return _fontSmall;
			}
		}

		public static SpriteSheet UISpritesheet
		{
			get
			{
				if (_uiSpritesheet != null) return _uiSpritesheet;

				_uiSpritesheet = _defaultAssetManager.Load<SpriteSheet>(DefaultSpritesheetName);

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

				_uiStylesheet = _defaultAssetManager.Load<Stylesheet>(DefaultStylesheetName);

				return _uiStylesheet;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_uiStylesheet = value;
			}
		}

		public static Texture2D White
		{
			get
			{
				if (_white == null)
				{
					_white = new Texture2D(MyraEnvironment.Game.GraphicsDevice, 1, 1);
					_white.SetData(new[] {Color.White});
				}

				return _white;
			}
		}

		public static Texture2D Transparent
		{
			get
			{
				if (_white == null)
				{
					_white = new Texture2D(MyraEnvironment.Game.GraphicsDevice, 1, 1);
					_white.SetData(new[] { Color.Transparent });
				}

				return _white;
			}
		}


		public static TextureRegion WhiteRegion
		{
			get { return _whiteRegion ?? (_whiteRegion = new TextureRegion(White, new Rectangle(0, 0, 1, 1))); }
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
			_defaultAssetManager.ClearCache();
			_font = null;
			_fontSmall = null;
			_uiSpritesheet = null;
			_uiStylesheet = null;
			_whiteRegion = null;

			if (_white != null)
			{
				_white.Dispose();
				_white = null;
			}

			if (_uiRasterizerState != null)
			{
				_uiRasterizerState.Dispose();
				_uiRasterizerState = null;
			}
		}
	}
}