using Myra.Graphics2D.UI;
using Myra.Platform;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Drawing;

namespace Myra.Samples.AllWidgets
{
	internal class AllWidgetsGame : GameWindow
	{
		private OpenTKPlatform _platform;
		private AllWidgets _allWidgets;
		private Desktop _desktop;

		public AllWidgetsGame() : base(new GameWindowSettings { UpdateFrequency = 60.0 }, new NativeWindowSettings { ClientSize = new OpenTK.Mathematics.Vector2i(1200, 800), Title = "Myra.PlatformAgnostic.OpenTK" })
		{
		}

		protected override void OnLoad()
		{
			base.OnLoad();

			_platform = new OpenTKPlatform(this);
			MyraEnvironment.Platform = _platform;
			MyraEnvironment.EnableModalDarkening = true;
			_platform.Viewport = new Rectangle(0, 0, ClientSize.X, ClientSize.Y);

			GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

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
						Close();
					}
				}
			};

			_desktop.Root = _allWidgets;
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			base.OnUpdateFrame(args);

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

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);

			GL.Clear(ClearBufferMask.ColorBufferBit);

			_allWidgets._labelOverGui.Text = "Is mouse over GUI: " + _desktop.IsMouseOverGUI;
			_desktop.Render();

			SwapBuffers();
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);

			_platform.Viewport = new Rectangle(0, 0, ClientSize.X, ClientSize.Y);
		}

		protected override void OnUnload()
		{
			base.OnUnload();
		}
	}
}
