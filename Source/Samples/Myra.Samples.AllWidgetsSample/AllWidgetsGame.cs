using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace Myra.Samples.AllWidgetsSample
{
	public class AllWidgetsGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;

		private Desktop _host;
		private Window _window;
		private AllWidgets _allWidgets;

		public AllWidgetsGame()
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

			// Widget.DrawFrames = true;
			_host = new Desktop();

			_allWidgets = new AllWidgets();

			var label = new TextBlock
			{
				Text =
					"Lorem ipsum [Green]dolor sit amet, [Red]consectetur adipisicing elit, sed do eiusmod [#AAAAAAAA]tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. [white]Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum!",
				VerticalSpacing = 0,
				TextColor = Color.AntiqueWhite,
				Wrap = true
			};

			var pane = new ScrollPane
			{
				Widget = label,
				WidthHint = 200,
				HeightHint = 200
			};

			_window = new Window
			{
				Title = "Text",
				Content = pane
			};

			_allWidgets._button.Up += (sender, args) =>
			{
				_window.ShowModal(_host);
			};

			_allWidgets._blueButton.Up += (sender, args) =>
			{
				_window.ShowModal(_host);
			};

			var tree = new Tree
			{
				HasRoot = false,
				GridPositionX = 1,
				GridPositionY = 9
			};
			var node1 = tree.AddSubNode("node1");
			var node2 = node1.AddSubNode("node2");
			var node3 = node2.AddSubNode("node3");
			node3.AddSubNode("node4");
			node3.AddSubNode("node5");
			node2.AddSubNode("node6");
			_allWidgets._gridRight.Widgets.Add(tree);

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