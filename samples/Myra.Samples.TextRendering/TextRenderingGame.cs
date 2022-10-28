using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Samples.TextRendering.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Graphics2D.Brushes;

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

			// Top desktop occupies upper half
			_topDesktop = new Desktop
			{
				Background = new SolidBrush(Color.DarkGray),
				BoundsFetcher = () => new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2)
			};

			_labelText = new Label();
			_topDesktop.Root = _labelText;

			// Bottom desktop - bottom half
			_bottomDesktop = new Desktop
			{
				BoundsFetcher = () => new Rectangle(0, GraphicsDevice.Viewport.Height / 2, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2)
			};
#if MONOGAME
			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			_bottomDesktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_bottomDesktop.OnChar(a.Character);
			};
#endif

			_mainPanel = new MainPanel();

			_bottomDesktop.Root = _mainPanel;
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