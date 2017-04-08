using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using Myra.Graphics3D;
using Myra.Graphics3D.Terrain;
using Myra.Graphics3D.Utils;
using Myra.Samples.Graphics3D.Utility;
using Myra.Utility;
using DirectionalLight = Myra.Graphics3D.Environment.DirectionalLight;

namespace Myra.Samples.Graphics3D
{
	public class TerrainSample : Game
	{
		private readonly GraphicsDeviceManager graphics;
		private Scene _scene;
		private TerrainObject _terrain;
		private readonly Camera _camera;
		private readonly CameraInputController _cameraController;
		private readonly DirectionalLight[] _lights;
		private Desktop _desktop;
		private Grid _progressGrid;
		private HorizontalProgressBar _progressBar;
		private TextBlock _progressText;
		private StatisticsGrid3d _statisticsGrid;
		private float _sunAngle = (float) Math.PI*3/2;

		public TerrainSample()
		{
			graphics = new GraphicsDeviceManager(this);

			IsMouseVisible = true;

			_camera = new PerspectiveCamera
			{
				Position = new Vector3(0, 300, 80)
			};

			_cameraController = new CameraInputController(_camera);

			_lights = new[]
			{
				new DirectionalLight
				{
					Color = Color.White,
					Direction = new Vector3(0, -1, 0)
				}
			};
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			// Init 2d stuff
			_desktop = new Desktop();

			_statisticsGrid = new StatisticsGrid3d();

			_progressGrid = new Grid
			{
				ColumnSpacing = 8,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			_progressGrid.ColumnsProportions.Add(new Grid.Proportion());
			_progressGrid.ColumnsProportions.Add(new Grid.Proportion());

			_progressText = new TextBlock
			{
				Text = "Generating Height Map:"
			};
			_progressGrid.Widgets.Add(_progressText);

			_progressBar = new HorizontalProgressBar
			{
				WidthHint = 200,
				GridPositionX = 1,
				VerticalAlignment = VerticalAlignment.Center
			};
			_progressGrid.Widgets.Add(_progressBar);

			_desktop.Widgets.Add(_progressGrid);

			_terrain = new TerrainObject();

			_scene = new Scene();
			_scene.Items.Add(_terrain);

			Task.Run(() => TerrainBuilder());
		}

		private void TerrainBuilder()
		{
			// Init 3d stuff
			var generator = new HeightMapGenerator(2048, 2048, 0, 300)
			{
				ProgressReporter = HeightMapProgressReporter
			};

			var heightMap = generator.Generate();

			_terrain.ProgressReporter = TerrainProgressReporter;
			_terrain.HeightMap = heightMap;
			_terrain.AddTextureLayer(Samples3DAssets.SampleTexture2);

			_terrain.Update();

			GC.Collect();
		}

		private void HeightMapProgressReporter(ProgressInfo f)
		{
			if (!f.Finished)
			{
				_progressBar.Value = _progressBar.Minimum + (_progressBar.Maximum - _progressBar.Minimum)*f.Progress;
			}
			else
			{
				_progressText.Text = "Building Terrain:";
			}
		}

		private void TerrainProgressReporter(ProgressInfo f)
		{
			if (!f.Finished)
			{
				_progressBar.Value = _progressBar.Minimum + (_progressBar.Maximum - _progressBar.Minimum)*f.Progress;
			}
			else
			{
				_desktop.Widgets.Remove(_progressGrid);
				_desktop.Widgets.Add(_statisticsGrid);
			}
		}

		protected override void Update(GameTime gameTime)
		{
			_statisticsGrid.Update(gameTime);

			var keyboardState = Keyboard.GetState();
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
				Exit();

			if (!_terrain.Ready)
			{
				return;
			}

			// Manage camera input controller
			_cameraController.SetControlKeyState(CameraInputController.ControlKeys.Left, keyboardState.IsKeyDown(Keys.A));
			_cameraController.SetControlKeyState(CameraInputController.ControlKeys.Right, keyboardState.IsKeyDown(Keys.D));
			_cameraController.SetControlKeyState(CameraInputController.ControlKeys.Forward, keyboardState.IsKeyDown(Keys.W));
			_cameraController.SetControlKeyState(CameraInputController.ControlKeys.Backward, keyboardState.IsKeyDown(Keys.S));
			_cameraController.SetControlKeyState(CameraInputController.ControlKeys.Up, keyboardState.IsKeyDown(Keys.Up));
			_cameraController.SetControlKeyState(CameraInputController.ControlKeys.Down, keyboardState.IsKeyDown(Keys.Down));

			var mouseState = Mouse.GetState();
			_cameraController.SetTouchState(CameraInputController.TouchType.Move, mouseState.LeftButton == ButtonState.Pressed);
			_cameraController.SetTouchState(CameraInputController.TouchType.Rotate, mouseState.RightButton == ButtonState.Pressed);
			_cameraController.SetMousePosition(mouseState.Position);

			_cameraController.Update();

			var s = (float) Math.Sin(_sunAngle);
			var c = (float) Math.Cos(_sunAngle);

			_lights[0].Direction = new Vector3(c, s, 0);

			_sunAngle += 0.0025f;

			if (_sunAngle > (float) Math.PI*2)
			{
				_sunAngle -= (float) Math.PI*2;
			}

			base.Update(gameTime);
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

			var device = GraphicsDevice;

			device.Clear(Color.Black);

			if (_terrain.Ready)
			{
				_scene.Render(_camera, _statisticsGrid.IsLightningOn ? _lights : null, _statisticsGrid.RenderFlags);
			}

			_desktop.Bounds = new Rectangle(0, 0,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_desktop.Render();

			_statisticsGrid.Draw(gameTime, GraphicsDevice, _scene);
		}
	}
}