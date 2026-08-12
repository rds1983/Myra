using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Myra.Samples.AllWidgets
{
	public enum ViewportMode
	{
		Default,
		FixedCentered,
		Cinema,
		FixedScaled
	}

	public class AllWidgetsGame : Game
	{
		private const int FixedWidth = 800;
		private const int FixedHeight = 600;
		private const int CinemaBorder = 100;

		private readonly GraphicsDeviceManager _graphics;

		private AllWidgets _allWidgets;
		private Desktop _desktop1, _desktop2;
		private ViewportMode _mode;

		public static AllWidgetsGame Instance { get; private set; }

		public ViewportMode Mode
		{
			get => _mode;

			set
			{
				if (value == _mode)
				{
					return;
				}

				_mode = value;

				switch (_mode)
				{
					case ViewportMode.Default:
						_desktop1.BoundsFetcher = Desktop.DefaultBoundsFetcher;
						break;

					case ViewportMode.FixedCentered:
						_desktop1.BoundsFetcher = () =>
						{
							var viewport = GraphicsDevice.Viewport;
							var x = (viewport.Width - FixedWidth) / 2;
							var y = (viewport.Height - FixedHeight) / 2;
							return new Rectangle(x, y, FixedWidth, FixedHeight);
						};
						break;

					case ViewportMode.Cinema:
						_desktop1.BoundsFetcher = () =>
						{
							var viewport = GraphicsDevice.Viewport;
							return new Rectangle(0, CinemaBorder, viewport.Width, viewport.Height - CinemaBorder * 2);
						};
						break;

					case ViewportMode.FixedScaled:
						_desktop1.BoundsFetcher = () => new Rectangle(0, 0, FixedWidth, FixedHeight);
						break;

				}
			}
		}

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

			MyraEnvironment.Game = this;
			MyraEnvironment.EnableModalDarkening = true;

			_allWidgets = new AllWidgets();

			_desktop1 = new Desktop();
			_desktop1.KeyDown += (s, a) =>
			{
				if (_desktop1.HasModalWidget || _allWidgets._mainMenu.IsOpen)
				{
					return;
				}

				if (_desktop1.IsKeyDown(Keys.LeftControl) || _desktop1.IsKeyDown(Keys.RightControl))
				{
					if (_desktop1.IsKeyDown(Keys.O))
					{
						_allWidgets.OpenFile();
					}
					else if (_desktop1.IsKeyDown(Keys.S))
					{
						_allWidgets.SaveFile();
					}
					else if (_desktop1.IsKeyDown(Keys.D))
					{
						_allWidgets.ChooseFolder();
					}
					else if (_desktop1.IsKeyDown(Keys.L))
					{
						_allWidgets.ChooseColor();
					}
					else if (_desktop1.IsKeyDown(Keys.Q))
					{
						Exit();
					}
				}
			};

			_desktop1.Root = _allWidgets;

			// Zero Desktop.TransformOrigin(by default it is set to 0.5, 0.5)
			_desktop1.TransformOrigin = Vector2.Zero;

			_desktop2 = new Desktop();

			var combo = new ComboView
			{
				VerticalAlignment = VerticalAlignment.Bottom
			};

			combo.Top = -100;

			foreach (var value in Enum.GetValues(typeof(ViewportMode)))
			{
				combo.Widgets.Add(new Label { Text = value.ToString() });
			}

			combo.SelectedIndexChanged += (s, a) =>
			{
				Mode = (ViewportMode)combo.SelectedIndex;
			};

			_desktop2.Root = combo;
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

			// Update Desktop.Scale
			switch (_mode)
			{
				case ViewportMode.FixedScaled:
					{
						var viewport = GraphicsDevice.Viewport;
						_desktop1.Scale = new Vector2((float)viewport.Width / FixedWidth, (float)viewport.Height / FixedHeight);
					}
					break;
				default:
					_desktop1.Scale = Vector2.One;
					break;
			}

			_allWidgets._labelOverGui.Text = "Is mouse over GUI: " + _desktop1.IsMouseOverGUI;
			_desktop1.Render();
			_desktop2.Render();
		}
	}
}