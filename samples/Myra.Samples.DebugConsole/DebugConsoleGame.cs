using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;

namespace Myra.Samples.DebugConsole
{
	public class DebugConsoleGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private GamePanel _gamePanel;
		
		public static DebugConsoleGame Instance { get; private set; }

		public DebugConsoleGame()
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

			_gamePanel = new GamePanel();

			Desktop.Widgets.Add(_gamePanel);

#if MONOGAME
			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			Desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				Desktop.OnChar(a.Character);
			};
#endif
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);
			Desktop.Render();
		}
	}
}