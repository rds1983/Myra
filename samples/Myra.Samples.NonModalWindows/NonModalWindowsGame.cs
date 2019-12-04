using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Samples.NonModalWindows.UI;

namespace Myra.Samples.NonModalWindows
{
	public class NonModalWindowsGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;

		private Desktop _desktop;
		public static NonModalWindowsGame Instance { get; private set; }

		public NonModalWindowsGame()
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

			_desktop = new Desktop();

			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			_desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};

			var mainPanel = new MainPanel();
			_desktop.Widgets.Add(mainPanel);

			mainPanel.ShowWindows();
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			if (_graphics.PreferredBackBufferWidth != Window.ClientBounds.Width ||
			    _graphics.PreferredBackBufferHeight != Window.ClientBounds.Height)
			{
				_graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				_graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				_graphics.ApplyChanges();
			}

			_desktop.Render();
		}
	}
}