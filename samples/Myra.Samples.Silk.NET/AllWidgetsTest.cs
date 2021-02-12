using Myra.Graphics2D.UI;
using Myra.Platform;
using Silk.NET.OpenGL;
using System;
using System.Drawing;
using System.Numerics;

namespace Myra.Samples.AllWidgets
{
	internal class AllWidgetsTest: TestBase
	{
		private AllWidgets _allWidgets;
		private Desktop _desktop;

		public static AllWidgetsTest Instance { get; private set; }

		public event EventHandler SizeChanged;

		public AllWidgetsTest()
		{
			Instance = this;
		}

		protected override void OnLoad()
		{
			graphicsDevice.ClearColor = new Vector4(0, 0, 0, 1);

			MyraEnvironment.Platform = new TrippyPlatform(graphicsDevice, InputContext);

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


		protected override void OnUpdate(double dt)
		{
			base.OnUpdate(dt);

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

		protected override void OnRender(double dt)
		{
			graphicsDevice.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.DepthBufferBit);
			_desktop.Render();

			Window.SwapBuffers();
		}

		protected override void OnResized(Size size)
		{
			graphicsDevice.SetViewport(0, 0, (uint)size.Width, (uint)size.Height);

			SizeChanged?.Invoke(this, EventArgs.Empty);
		}

		protected override void OnUnload()
		{
		}
	}
}
