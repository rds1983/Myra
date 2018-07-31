using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

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
			var colorBuffer = ColorBuffer.FromStream(assetResolver.Open("ui_stylesheet_image.png"));
			colorBuffer.Process(true);

			var texture = colorBuffer.CreateTexture2D();

			// Load ui text atlas
			var textureAtlas = TextureRegionAtlas.Load(assetResolver.ReadAsString("ui_stylesheet_atlas.atlas"), 
				s => texture);

			// Load ui font(s)
			var font = SpriteFontHelper.LoadFromFnt(assetResolver.ReadAsString("ui_font.fnt"),
				textureAtlas["default"]);

			// Load stylesheet
			var stylesheet = Stylesheet.CreateFromSource(assetResolver.ReadAsString("ui_stylesheet.json"),
				s => textureAtlas[s],
				s => font);

			Stylesheet.Current = stylesheet;

			// Widget.DrawFrames = true;
			_host = new Desktop();

			_allWidgets = new AllWidgets();

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