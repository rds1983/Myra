using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using SpriteFontPlus;
using System.Collections.Generic;

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
			Texture2D texture;
			using (var stream = assetResolver.Open("ui_stylesheet_atlas.png"))
			{
				texture = Texture2D.FromStream(GraphicsDevice, stream);
			}

			// Load ui text atlas
			var textureAtlas = TextureRegionAtlas.FromJson(assetResolver.ReadAsString("ui_stylesheet_atlas.json"), texture);

			// Load ui font(s)
			var region = textureAtlas["commodore-64"];
			var fonts = new Dictionary<string, SpriteFont>
			{
				["commodore-64"] = 
					BMFontLoader.LoadText(
						assetResolver.ReadAsString("commodore-64.fnt"),
						s => new TextureWithOffset(region.Texture, region.Bounds.Location)
					)
			};

			// Load stylesheet
			var stylesheet = Stylesheet.CreateFromSource(assetResolver.ReadAsString("ui_stylesheet.json"),
				s => textureAtlas[s],
				s => fonts[s]);

			Stylesheet.Current = stylesheet;

			_host = new Desktop();

			_allWidgets = new AllWidgets();
			_allWidgets._button.Image = textureAtlas["music-off"];
			_allWidgets._imageButton.Image = textureAtlas["sound-off"];

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