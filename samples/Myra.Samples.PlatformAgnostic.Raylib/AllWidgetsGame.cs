using Myra.Graphics2D.UI;
using Myra.Platform;
using Raylib_cs;

namespace Myra.Samples.AllWidgets
{
	internal class AllWidgetsGame
	{
		private RaylibPlatform _platform;
		private AllWidgets _allWidgets;
		private Desktop _desktop;
		private const int WindowWidth = 1200;
		private const int WindowHeight = 800;

		public bool IsRunning { get; set; } = true;

		public static AllWidgetsGame Instance { get; private set; }

		public AllWidgetsGame()
		{
			Instance = this;

			Raylib.SetConfigFlags(ConfigFlags.VSyncHint);
			Raylib.InitWindow(WindowWidth, WindowHeight, "Myra.AllWidgets.Raylib");
			Raylib.SetTargetFPS(60);
		}

		public void Run()
		{
			_platform = new RaylibPlatform();
			MyraEnvironment.Platform = _platform;
			MyraEnvironment.EnableModalDarkening = true;

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
					}
				}
			};

			_desktop.Root = _allWidgets;

			while (IsRunning && !Raylib.WindowShouldClose())
			{
				Update();
				Render();
			}

			Raylib.CloseWindow();
		}

		private void Update()
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
		}

		private void Render()
		{
			Raylib.BeginDrawing();
			Raylib.ClearBackground(new Color(0, 0, 0, 255));

			_allWidgets._labelOverGui.Text = "Is mouse over GUI: " + _desktop.IsMouseOverGUI;
			_desktop.Render();

			Raylib.EndDrawing();
		}
	}
}
