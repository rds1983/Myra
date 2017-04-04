using System;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace Myra.Samples
{
	public class ScrollPaneSample: Game
	{
		private readonly GraphicsDeviceManager graphics;

		private Desktop _host;
		private Window _window;

		public ScrollPaneSample()
		{
			graphics = new GraphicsDeviceManager(this);
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			_host = new Desktop();

			var label = new TextBlock
			{
				Text =
					"Lorem ipsum [Green]dolor sit amet, [Red]consectetur adipisicing elit, sed do eiusmod [#AAAAAAAA]tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. [white]Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum!",
				VerticalSpacing = 0,
				TextColor = Color.AntiqueWhite,
				Wrap =  true
			};

			var pane = new ScrollPane<TextBlock>
			{
				Widget = label,
				WidthHint = 200,
				HeightHint = 200
			};

			var button = new Button
			{
				Text = "Show Window"
			};

			button.Up += ButtonOnUp;

			_host.Widgets.Add(button);

			_window = new Window
			{
				Title = "Text"
			};

			_window.ContentGrid.Widgets.Add(pane);
		}

		private void ButtonOnUp(object sender, EventArgs eventArgs)
		{
			_window.ShowModal(_host);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (graphics.PreferredBackBufferWidth != Window.ClientBounds.Width ||
				graphics.PreferredBackBufferHeight != Window.ClientBounds.Height)
			{
				graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				graphics.ApplyChanges();
			}

			GraphicsDevice.Clear(Color.Black);

			_host.Bounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
			_host.Render();
		}
	}
}
