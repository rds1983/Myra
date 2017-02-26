using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using Myra.Graphics3D;
using Myra.Graphics3D.Terrain;
using Myra.Graphics3D.Utils;
using Myra.Samples.Utility;
using Myra.Utility;
using DirectionalLight = Myra.Graphics3D.Environment.DirectionalLight;

namespace Myra.Samples
{
	public class TerrainSample: Game
	{
		private readonly GraphicsDeviceManager graphics;
		private Scene _scene;
		private TerrainObject _terrain;
		private readonly Camera _camera;
		private readonly CameraInputController _cameraController;
		private readonly DirectionalLight[] _lights;
		private Desktop _desktop;
		private Grid _statisticsGrid;
		private CheckBox _lightsOn;
		private CheckBox _drawNormals;
		private TextBlock _gcMemoryLabel;
		private TextBlock _processMemoryLabel;
		private TextBlock _fpsLabel;
		private TextBlock _drawCallsLabel;
		private TextBlock _modelsCountLabel;
		private TextBlock _primitiveCountLabel;
		private TextBlock _textureCountLabel;
		private TextBlock _vertexShaderCountLabel;
		private TextBlock _pixelShaderCountLabel;
		private readonly FPSCounter _fpsCounter = new FPSCounter();
		float _sunAngle = (float)Math.PI * 3 / 2;

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

			_statisticsGrid = new Grid();

			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());

			_lightsOn = new CheckBox
			{
				IsPressed = true,
				Text = "Lights On"
			};
			_statisticsGrid.Widgets.Add(_lightsOn);


			_drawNormals = new CheckBox
			{
				IsPressed = false,
				Text = "Draw Normals",
				GridPositionY = 1
			};
			_statisticsGrid.Widgets.Add(_drawNormals);

			_gcMemoryLabel = new TextBlock
			{
				Text = "GC Memory: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 2
			};
			_statisticsGrid.Widgets.Add(_gcMemoryLabel);

			_processMemoryLabel = new TextBlock
			{
				Text = "Process Memory: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 3
			};
			_statisticsGrid.Widgets.Add(_processMemoryLabel);

			_fpsLabel = new TextBlock
			{
				Text = "FPS: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 4
			};

			_statisticsGrid.Widgets.Add(_fpsLabel);

			_drawCallsLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 5
			};

			_statisticsGrid.Widgets.Add(_drawCallsLabel);

			_modelsCountLabel = new TextBlock
			{
				Text = "Models Count: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 6
			};

			_statisticsGrid.Widgets.Add(_modelsCountLabel);

			_primitiveCountLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 7
			};

			_statisticsGrid.Widgets.Add(_primitiveCountLabel);

			_textureCountLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 8
			};

			_statisticsGrid.Widgets.Add(_textureCountLabel);

			_vertexShaderCountLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 9
			};

			_statisticsGrid.Widgets.Add(_vertexShaderCountLabel);

			_pixelShaderCountLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 10
			};

			_statisticsGrid.Widgets.Add(_pixelShaderCountLabel);

			_statisticsGrid.HorizontalAlignment = HorizontalAlignment.Left;
			_statisticsGrid.VerticalAlignment = VerticalAlignment.Bottom;
			_statisticsGrid.XHint = 10;
			_statisticsGrid.YHint = -10;
			_desktop.Widgets.Add(_statisticsGrid);

			// Init 3d stuff
			var generator = new HeightMapGenerator(2048, 2048, 0, 300);

			var heightMap = generator.Generate();

			_terrain = new TerrainObject
			{
				HeightMap = heightMap,
				BlockWidth = 4,
				BlockHeight = 4
			};

			_terrain.Init();

			_terrain.AddTextureLayer(SampleAssets.SampleTexture2);

			_scene = new Scene();
			_scene.Items.Add(_terrain);

			GC.Collect();
		}

		protected override void Update(GameTime gameTime)
		{
			var keyboardState = Keyboard.GetState();
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
				Exit();

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

			var s = (float)Math.Sin(_sunAngle);
			var c = (float)Math.Cos(_sunAngle);

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

			var device = GraphicsDevice;

			device.Clear(Color.Black);

			if (graphics.PreferredBackBufferWidth != Window.ClientBounds.Width ||
			    graphics.PreferredBackBufferHeight != Window.ClientBounds.Height)
			{
				graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				graphics.ApplyChanges();
			}

			_terrain.DrawNormals = _drawNormals.IsPressed;
			_scene.Render(_camera, _lightsOn.IsPressed ? _lights : null);

			_desktop.Bounds = new Rectangle(0, 0,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_desktop.Render();

			_fpsCounter.Update();
			_gcMemoryLabel.Text = string.Format("GC Memory: {0} kb", GC.GetTotalMemory(false)/1024);
			_processMemoryLabel.Text = string.Format("Process Memory: {0} kb",
				Process.GetCurrentProcess().PrivateMemorySize64/1024);
			_fpsLabel.Text = string.Format("FPS: {0:0.##}", _fpsCounter.FPS);
			_drawCallsLabel.Text = string.Format("Draw Calls: {0}", GraphicsDevice.Metrics.DrawCount);
			_modelsCountLabel.Text = string.Format("Models Count: {0}", _scene.ModelsRendered);
			_primitiveCountLabel.Text = string.Format("Primitive Count: {0}", GraphicsDevice.Metrics.PrimitiveCount);
			_textureCountLabel.Text = string.Format("Texture Count: {0}", GraphicsDevice.Metrics.TextureCount);
			_vertexShaderCountLabel.Text = string.Format("Vertex Shader Count: {0}", GraphicsDevice.Metrics.VertexShaderCount);
			_pixelShaderCountLabel.Text = string.Format("Pixel Shader Count: {0}", GraphicsDevice.Metrics.PixelShaderCount);
		}

	}
}
