using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Samples.NonModalWindows.UI;

namespace Myra.Samples.NonModalWindows
{
	public class NonModalWindowsGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private MainPanel _mainPanel;

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

			_mainPanel = new MainPanel();
			Desktop.Widgets.Add(_mainPanel);

            var Panel = new Panel();
            Panel.Widgets.Add(new Window() { Title = "Test" });
            Desktop.Widgets.Add(Panel);

			_mainPanel.ShowWindows();
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			_mainPanel._labelOverGui.Text = "Is mouse over GUI: " + Desktop.IsMouseOverGUI;
			Desktop.Render();
		}
	}
}