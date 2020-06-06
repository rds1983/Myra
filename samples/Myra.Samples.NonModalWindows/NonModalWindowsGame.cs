using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Samples.NonModalWindows.UI;
using System.Runtime.InteropServices;

namespace Myra.Samples.NonModalWindows
{
	public class NonModalWindowsGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private MainPanel _mainPanel;
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

			_desktop = new Desktop
			{
				// Inform Myra that external text input is available
				// So it stops translating Keys to chars
				HasExternalTextInput = true
			};

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};

			_mainPanel = new MainPanel();

			_desktop.Root = _mainPanel;

			_mainPanel.ShowWindows();
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			_mainPanel._labelOverGui.Text = "Is mouse over GUI: " + _desktop.IsMouseOverGUI;
			_desktop.Render();
		}
	}
}