using System;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using Myra.Assets;
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

		private static BitmapFont _font;
		private static BitmapFont _fontSmall;
		private static TextureAtlas _uiSpritesheet;
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

		public static TextureAtlas UISpritesheet
		{
			get
			{
				if (_uiSpritesheet != null) return _uiSpritesheet;

				_uiSpritesheet = _defaultAssetManager.Load<TextureAtlas>(DefaultSpritesheetName);

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

			if (_uiRasterizerState != null)
			{
				_uiRasterizerState.Dispose();
				_uiRasterizerState = null;
			}
		}
	}
}