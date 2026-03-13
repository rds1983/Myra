using Myra.Graphics2D.UI;

#if !STRIDE
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#if ANDROID
#endif
#else
using System.Threading.Tasks;
using Stride.Engine;
using Stride.Games;
using Stride.Graphics;
using Stride.Core.Mathematics;
#endif

namespace Myra.Samples.Inspector
{
	public class InspectGame : Game
	{
#if !STRIDE
		private readonly GraphicsDeviceManager _graphics;
#endif

		private Input _input;
		private RootWidgets _widgets;
		private Desktop _desktop;
		
		public static InspectGame Instance { get; private set; }

		public InspectGame()
		{
			Instance = this;
#if !STRIDE
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800
			};
			Window.AllowUserResizing = false;
#else
#endif

			IsMouseVisible = true;
		}

#if STRIDE
		protected override Task LoadContent()
		{
			MyraEnvironment.Game = this;

			_widgets = new RootWidgets();

			_desktop = new Desktop();
			_desktop.Widgets.Add(_allWidgets);

			return base.LoadContent();
		}
#else
		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;
			MyraEnvironment.EnableModalDarkening = true;

//			Stylesheet.Current = DefaultAssets.DefaultStylesheet2X;

			_widgets = new RootWidgets();

			_desktop = new Desktop();
			_desktop.Root = _widgets;
			
			_input = new Input(() => _widgets.inspectables.Count, () => _widgets.infos.Count)
			{
				OnSelectionChanged = (int i) => { _widgets.SetSelectedIndex(i); },
				OnTextInfoChanged = (int i) => { _widgets.SetInfoIndex(i); },
			};
			
#if MONOGAME && !ANDROID
			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			_desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};
#endif
		}
#endif
		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			_input.Update(Keyboard.GetState());
			_widgets.Update(gameTime);
		}
		
		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

#if !STRIDE
			GraphicsDevice.Clear(Color.Black);
#else
			// Clear screen
			GraphicsContext.CommandList.Clear(GraphicsDevice.Presenter.BackBuffer, Color.Black);
			GraphicsContext.CommandList.Clear(GraphicsDevice.Presenter.DepthStencilBuffer, DepthStencilClearOptions.DepthBuffer | DepthStencilClearOptions.Stencil);

			// Set render target
			GraphicsContext.CommandList.SetRenderTargetAndViewport(GraphicsDevice.Presenter.DepthStencilBuffer, GraphicsDevice.Presenter.BackBuffer);
#endif
			_widgets.OnPreRender();
			_desktop.Render();
		}
	}
}