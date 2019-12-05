using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Samples.NonModalWindows.UI;

namespace Myra.Samples.NonModalWindows
{
	public class NonModalWindowsGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;

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

			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			Desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				Desktop.OnChar(a.Character);
			};

			var mainPanel = new MainPanel();
			Desktop.Widgets.Add(mainPanel);

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

			Desktop.Render();
		}
	}
}