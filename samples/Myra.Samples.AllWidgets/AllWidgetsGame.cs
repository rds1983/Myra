using Myra.Graphics2D.UI;
using System.Linq;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#if ANDROID
using System;
using Microsoft.Xna.Framework.GamerServices;
#endif
#else
using System.Threading.Tasks;
using Xenko.Engine;
using Xenko.Games;
using Xenko.Graphics;
using Xenko.Core.Mathematics;
#endif

namespace Myra.Samples.AllWidgets
{
	public class AllWidgetsGame : Game
	{
#if !XENKO
		private readonly GraphicsDeviceManager _graphics;
#endif

		private Desktop _desktop;
		private AllWidgets _allWidgets;
		
		public static AllWidgetsGame Instance { get; private set; }

		public AllWidgetsGame()
		{
			Instance = this;

#if !XENKO
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800
			};
			Window.AllowUserResizing = true;
#else
#endif

			IsMouseVisible = true;
		}

#if XENKO
		protected override Task LoadContent()
		{

			MyraEnvironment.Game = this;

			_desktop = new Desktop();

			_allWidgets = new AllWidgets();

			_desktop.Widgets.Add(_allWidgets);

			return base.LoadContent();
		}
#else
		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			_desktop = new Desktop();

			_allWidgets = new AllWidgets();

			_desktop.KeyDown += (s, a) =>
			{
				if (_desktop.HasModalWindow || _allWidgets._mainMenu.IsOpen)
				{
					return;
				}

				if (_desktop.DownKeys.Contains(Keys.LeftControl) || _desktop.DownKeys.Contains(Keys.RightControl))
				{
					if (_desktop.DownKeys.Contains(Keys.O))
					{
						_allWidgets.OpenFile();
					} else if (_desktop.DownKeys.Contains(Keys.S))
					{
						_allWidgets.SaveFile();
					} else if (_desktop.DownKeys.Contains(Keys.D))
					{
						_allWidgets.ChooseFolder();
					} else if (_desktop.DownKeys.Contains(Keys.L))
					{
						_allWidgets.ChooseColor();
					}
					else if (_desktop.DownKeys.Contains(Keys.Q))
					{
						Exit();
					}
				}
			};

			_desktop.Widgets.Add(_allWidgets);

#if MONOGAME
			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			_desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};
#endif

#if ANDROID
			_desktop.WidgetGotKeyboardFocus += (s, a) =>
			{
				var asTextBox = a.Data as TextBox;
				if (asTextBox == null)
				{
					return;
				}

				Guide.BeginShowKeyboardInput(PlayerIndex.One,
					"Title",
					"Description",
					asTextBox.Text,
					new AsyncCallback(r =>
					{
						var text = Guide.EndShowKeyboardInput(r);
						asTextBox.Text = text;
					}),
					null);
			};
#endif
		}
#endif

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

#if !XENKO
			GraphicsDevice.Clear(Color.Black);

			if (_graphics.PreferredBackBufferWidth != Window.ClientBounds.Width ||
			    _graphics.PreferredBackBufferHeight != Window.ClientBounds.Height)
			{
				_graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				_graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				_graphics.ApplyChanges();
			}

#else
			// Clear screen
			GraphicsContext.CommandList.Clear(GraphicsDevice.Presenter.BackBuffer, Color.Black);
			GraphicsContext.CommandList.Clear(GraphicsDevice.Presenter.DepthStencilBuffer, DepthStencilClearOptions.DepthBuffer | DepthStencilClearOptions.Stencil);

			// Set render target
			GraphicsContext.CommandList.SetRenderTargetAndViewport(GraphicsDevice.Presenter.DepthStencilBuffer, GraphicsDevice.Presenter.BackBuffer);
#endif
			_desktop.Render();
		}
	}
}