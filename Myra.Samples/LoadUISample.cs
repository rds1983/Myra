using System;
using System.IO;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Utility;

namespace Myra.Samples
{
	public class LoadUISample : Game
	{
		private readonly GraphicsDeviceManager graphics;

		private Desktop _host;
		private ListBox _mainMenuList;

		public LoadUISample()
		{
			graphics = new GraphicsDeviceManager(this);
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			// Widget.DrawFrames = true;
			_host = new Desktop();

			// Load UI
			var data = File.ReadAllText("Assets/awesomeGame.ui");
			var ui = Serialization.LoadFromData(data);

			// Obtain reference to the "#listBoxMenu"
			_mainMenuList = (ListBox) ui.Root.FindWidgetById("mainMenu");

			_mainMenuList.SelectedIndexChanged += MainMenuListOnSelectedIndexChanged;

			_host.Widgets.Add(ui.Root);
		}

		private void MainMenuListOnSelectedIndexChanged(object sender, EventArgs eventArgs)
		{
			var messageBox = Dialog.CreateMessageBox("Selected", _mainMenuList.SelectedItem.Text);
			messageBox.ShowModal(_host);
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
