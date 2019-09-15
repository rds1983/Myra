using Myra.Graphics2D.UI;

#if !XENKO
using Microsoft.Xna.Framework;
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

		public AllWidgetsGame()
		{
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
				var asTextField = a.Data as TextField;
				if (asTextField == null)
				{
					return;
				}

				Guide.BeginShowKeyboardInput(PlayerIndex.One,
					"Title",
					"Description",
					asTextField.Text,
					new AsyncCallback(r =>
					{
						var text = Guide.EndShowKeyboardInput(r);
						asTextField.Text = text;
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