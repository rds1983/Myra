using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Platform;
using System;
using System.Numerics;

namespace Myra.Samples.AllWidgets
{
	public class AllWidgetsGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;

		private AllWidgets _allWidgets;
		private Desktop _desktop;
		
		public static AllWidgetsGame Instance { get; private set; }

		public AllWidgetsGame()
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

			MyraEnvironment.Platform = new MGPlatform(GraphicsDevice);

			_allWidgets = new AllWidgets();

			_desktop = new Desktop();
			_desktop.KeyDown += (s, a) =>
			{
				if (_desktop.HasModalWidget || _allWidgets._mainMenu.IsOpen)
				{
					return;
				}

				if (_desktop.IsKeyDown(Keys.LeftControl) || _desktop.IsKeyDown(Keys.RightControl))
				{
					if (_desktop.IsKeyDown(Keys.O))
					{
						_allWidgets.OpenFile();
					} else if (_desktop.IsKeyDown(Keys.S))
					{
						_allWidgets.SaveFile();
					} else if (_desktop.IsKeyDown(Keys.D))
					{
						_allWidgets.ChooseFolder();
					} else if (_desktop.IsKeyDown(Keys.L))
					{
						_allWidgets.ChooseColor();
					}
					else if (_desktop.IsKeyDown(Keys.Q))
					{
						Exit();
					}
				}
			};

			_desktop.Root = _allWidgets;

			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			// _desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};
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

			GraphicsDevice.Clear(Color.Black);
			_desktop.Render();
		}
	}
}