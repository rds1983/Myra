using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Samples.TextRendering.UI;
using FontStashSharp;
using System.IO;

namespace Myra.Samples.TextRendering
{
	public class TextRenderingGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private MainPanel _mainPanel;
		private Desktop _topDesktop;
		private Desktop _bottomDesktop;
		private Label _labelText;

		public static TextRenderingGame Instance { get; private set; }

		public Desktop TopDesktop => _topDesktop;
		public Label LabelText => _labelText;

		public TextRenderingGame()
		{
			Instance = this;

			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800
			};
			Window.AllowUserResizing = true;
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			_topDesktop = new Desktop();

			var fontSystem = new FontSystem();
			fontSystem.AddFont(File.ReadAllBytes("Fonts\\DroidSans.ttf"));

			_labelText = new Label();
			_labelText.Font = fontSystem.GetFont(32);

			_topDesktop.Root = _labelText;

			_bottomDesktop = new Desktop
			{
				// Inform Myra that external text input is available
				// So it stops translating Keys to chars
				HasExternalTextInput = true
			};

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_bottomDesktop.OnChar(a.Character);
			};

			_mainPanel = new MainPanel();

			_bottomDesktop.Root = _mainPanel;

			// Top desktop occupies upper half
			_topDesktop.BoundsFetcher = () => new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2);

			// Bottom desktop - bottom half
			_bottomDesktop.BoundsFetcher = () => new Rectangle(0, GraphicsDevice.Viewport.Height / 2, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			_bottomDesktop.Render();
			_topDesktop.Render();
		}
	}
}