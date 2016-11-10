using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;

namespace Myra
{
	public static class DefaultAssets
	{
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

				var textureRegion = UISpritesheet.Drawables["default"].TextureRegion;
				_font = BitmapFont.CreateFromFNT(new StringReader(Resources.Resources.DefaultFont), textureRegion,
							MyraEnvironment.GraphicsDevice);
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

				var textureRegion = UISpritesheet.Drawables["font-small"].TextureRegion;

				_fontSmall = BitmapFont.CreateFromFNT(new StringReader(Resources.Resources.DefaultFontSmall), textureRegion,
					MyraEnvironment.GraphicsDevice);

				return _fontSmall;
			}
		}

		public static SpriteSheet UISpritesheet
		{
			get
			{
				if (_uiSpritesheet != null) return _uiSpritesheet;

				Texture2D texture;
				using (var stream = Resources.Resources.OpenDefaultUiSkinBitmapStream())
				{
					texture = Texture2D.FromStream(MyraEnvironment.GraphicsDevice, stream);
				}

				texture.Disposing += (sender, args) =>
				{
					_uiSpritesheet = null;
				};

				if (!MyraEnvironment.IsWindowsDX)
				{
					// Manually premultiply alpha
					var data = new Color[texture.Width * texture.Height];
					texture.GetData(data);

					for (var i = 0; i < data.Length; ++i)
					{
						data[i].R = ApplyAlpha(data[i].R, data[i].A);
						data[i].G = ApplyAlpha(data[i].G, data[i].A);
						data[i].B = ApplyAlpha(data[i].B, data[i].A);
					}

					texture.SetData(data);
				}

				_uiSpritesheet = SpriteSheet.LoadLibGDX(Resources.Resources.DefaultUISkinAtlas, s => texture, new[] { "default", "font-small" });

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

				// Create default
				_uiStylesheet = Stylesheet.CreateFromSource(Resources.Resources.DefaultStyleSheet,
					s =>
					{
						switch (s)
						{
							case "default-font":
								return Font;
							case "default-font-small":
								return FontSmall;
							default:
								throw new Exception(string.Format("Could not find default font '{0}'", s));
						}
					},
					s =>
					{
						ImageDrawable result;
						if (!UISpritesheet.Drawables.TryGetValue(s, out result))
						{
							throw new Exception(string.Format("Could not find default drawable '{0}'", s));
						}

						return result;
					});

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
					_white.SetData(new[] { Color.White });
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

		private static byte ApplyAlpha(byte color, byte alpha)
		{
			var fc = color / 255.0f;
			var fa = alpha / 255.0f;

			var fr = (int)(255.0f * fc * fa);

			if (fr < 0)
			{
				fr = 0;
			}

			if (fr > 255)
			{
				fr = 255;
			}

			return (byte)fr;
		}

		internal static void Dispose()
		{
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
