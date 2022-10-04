using Myra.Graphics2D.UI;
using Myra.Platform;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;
using TrippyGL;

namespace Myra.Samples.AllWidgets
{
	internal class AllWidgetsGame
	{
		private IWindow _window;
		private GraphicsDevice _graphicsDevice;
		private TrippyPlatform _platform;
		private AllWidgets _allWidgets;
		private Desktop _desktop;

		public AllWidgetsGame()
		{
			var options = WindowOptions.Default;
			options.Size = new Vector2D<int>(1200, 800);
			options.Title = "FontStashSharp.Silk.NET.TrippyGL";
			_window = Silk.NET.Windowing.Window.Create(options);
			_window.Load += OnLoad;
			_window.Render += OnRender;
			_window.Closing += OnClose;
			_window.Resize += OnResize;
		}

		public void Run() => _window.Run();

		private void OnResize(Vector2D<int> size)
		{
			_graphicsDevice.SetViewport(0, 0, (uint)size.X, (uint)size.Y);
			_platform.OnViewportChanged();
		}

		private void OnLoad()
		{
			_graphicsDevice = new GraphicsDevice(GL.GetApi(_window))
			{
				ClearColor = new Vector4(0, 0, 0, 1)
			};

			_platform = new TrippyPlatform(_graphicsDevice, _window.CreateInput());
			MyraEnvironment.Platform = _platform;
			OnResize(_window.Size);

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
					}
					else if (_desktop.IsKeyDown(Keys.S))
					{
						_allWidgets.SaveFile();
					}
					else if (_desktop.IsKeyDown(Keys.D))
					{
						_allWidgets.ChooseFolder();
					}
					else if (_desktop.IsKeyDown(Keys.L))
					{
						_allWidgets.ChooseColor();
					}
					else if (_desktop.IsKeyDown(Keys.Q))
					{
//						Exit();
					}
				}
			};

			_desktop.Root = _allWidgets;

/*			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			_desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};*/
		}


		private void OnRender(double obj)
		{
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

			_graphicsDevice.Clear(ClearBuffers.Color | ClearBuffers.Depth | ClearBuffers.Stencil);
			_desktop.Render();
		}

		private void OnClose()
		{
			_graphicsDevice.Dispose();
		}
	}
}
