using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Collections.Generic;
using System.IO;

namespace Myra.Samples.CustomUIStylesheet
{
	public class CustomUIStylesheetGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;

		private Desktop _host;
		private AllWidgets _allWidgets;

		public CustomUIStylesheetGame()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800
			};

			Content.RootDirectory = "Content";

			IsMouseVisible = true;
			Window.AllowUserResizing = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			// Create resource asset resolver
			var assetResolver = new ResourceAssetResolver(GetType().Assembly, "Myra.Samples.CustomUIStylesheet.Resources.");

			// Load image containing font & ui spritesheet
			var colorBuffer = ColorBuffer.FromStream(assetResolver.Open("ui_stylesheet_atlas.png"));
			colorBuffer.PremultiplyAlpha();

			var texture = colorBuffer.CreateTexture2D();

			// Load ui text atlas
			var textureAtlas = TextureRegionAtlas.FromJson(assetResolver.ReadAsString("ui_stylesheet_atlas.json"), texture);

			// Load ui font(s)
			var fonts = new Dictionary<string, SpriteFont>
			{
				["commodore-64"] = SpriteFontHelper.LoadFromFnt(assetResolver.ReadAsString("commodore-64.fnt"), textureAtlas["commodore-64"]),
			};

			// Load stylesheet
			var stylesheet = Stylesheet.CreateFromSource(assetResolver.ReadAsString("ui_stylesheet.json"),
				s => textureAtlas[s],
				s => fonts[s]);

			Stylesheet.Current = stylesheet;

			// Widget.DrawFrames = true;
			_host = new Desktop();

			_allWidgets = new AllWidgets
			{
				Background = new Drawable(textureAtlas["blue"])
			};

			_allWidgets._button.Image = new Drawable(textureAtlas["music-off"]);
			_allWidgets._imageButton.Image = new Drawable(textureAtlas["sound-off"]);

			_host.Widgets.Add(_allWidgets);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_allWidgets._horizontalProgressBar.Value += 0.5f;
			if (_allWidgets._horizontalProgressBar.Value > _allWidgets._horizontalProgressBar.Maximum)
			{
				_allWidgets._horizontalProgressBar.Value = _allWidgets._horizontalProgressBar.Minimum;
			}

			_allWidgets._verticalProgressBar.Value += 0.5f;
			if (_allWidgets._verticalProgressBar.Value > _allWidgets._verticalProgressBar.Maximum)
			{
				_allWidgets._verticalProgressBar.Value = _allWidgets._verticalProgressBar.Minimum;
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (_graphics.PreferredBackBufferWidth != Window.ClientBounds.Width ||
			    _graphics.PreferredBackBufferHeight != Window.ClientBounds.Height)
			{
				_graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				_graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				_graphics.ApplyChanges();
			}

			GraphicsDevice.Clear(Color.Black);

			_host.Bounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_host.Render();
		}
	}
}