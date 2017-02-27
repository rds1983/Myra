using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using Myra.Graphics3D;
using Myra.Graphics3D.Materials;
using Myra.Graphics3D.Utils;
using Myra.Utility;
using DirectionalLight = Myra.Graphics3D.Environment.DirectionalLight;

namespace Myra.Samples
{
	public class Primitives3DSample : Game
	{
		private class ModelInfo
		{
			public ModelObject ModelObject;
			public Vector3 Rotation;
		}

		private readonly GraphicsDeviceManager graphics;
		private Scene _scene;
		private readonly Camera _camera;
		private readonly CameraInputController _cameraController;
		private readonly List<ModelInfo> _modelInstances = new List<ModelInfo>();
		private readonly DirectionalLight[] _lights;
		private Desktop _desktop;
		private Grid _statisticsGrid;
		private CheckBox _lightsOn;
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
		private readonly Random _random = new Random();

		public Primitives3DSample()
		{
			graphics = new GraphicsDeviceManager(this);

			IsMouseVisible = true;

			_camera = new PerspectiveCamera
			{
				Position = new Vector3(0, 80, 80)
			};

			_cameraController = new CameraInputController(_camera);

			_lights = new[]
			{
				new DirectionalLight
				{
					Color = Color.White,
					Direction = new Vector3(0, -1, 0)
				},
				new DirectionalLight
				{
					Color = Color.White,
					Direction = new Vector3(1, 1, -1)
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

			_lightsOn = new CheckBox
			{
				IsPressed = true,
				Text = "Lights On"
			};
			_statisticsGrid.Widgets.Add(_lightsOn);

			_gcMemoryLabel = new TextBlock
			{
				Text = "GC Memory: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 1
			};
			_statisticsGrid.Widgets.Add(_gcMemoryLabel);

			_processMemoryLabel = new TextBlock
			{
				Text = "Process Memory: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 2
			};
			_statisticsGrid.Widgets.Add(_processMemoryLabel);

			_fpsLabel = new TextBlock
			{
				Text = "FPS: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 3
			};

			_statisticsGrid.Widgets.Add(_fpsLabel);

			_drawCallsLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 4
			};

			_statisticsGrid.Widgets.Add(_drawCallsLabel);

			_modelsCountLabel = new TextBlock
			{
				Text = "Models Count: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 5
			};

			_statisticsGrid.Widgets.Add(_modelsCountLabel);

			_primitiveCountLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 6
			};

			_statisticsGrid.Widgets.Add(_primitiveCountLabel);

			_textureCountLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 7
			};

			_statisticsGrid.Widgets.Add(_textureCountLabel);

			_vertexShaderCountLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 8
			};

			_statisticsGrid.Widgets.Add(_vertexShaderCountLabel);

			_pixelShaderCountLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 9
			};

			_statisticsGrid.Widgets.Add(_pixelShaderCountLabel);

			_statisticsGrid.HorizontalAlignment = HorizontalAlignment.Left;
			_statisticsGrid.VerticalAlignment = VerticalAlignment.Bottom;
			_statisticsGrid.XHint = 10;
			_statisticsGrid.YHint = -10;
			_desktop.Widgets.Add(_statisticsGrid);

			// Init 3d stuff
			var gridModel = PrimitivesFactory.CreateXZGrid(new Vector2(100, 100));

			var grid = new ModelObject(gridModel)
			{
				Material = new BaseMaterial
				{
					DiffuseColor = Color.Gray
				}
			};

			_modelInstances.Add(new ModelInfo
			{
				ModelObject = grid,
				Rotation = new Vector3(0, 0, 0)
			});

			for (var i = 0; i < 500; ++i)
			{
				var sr = _random.Next(1, 7);
				var trx = _random.Next(-50, 50);
				var trs = _random.Next(-50, 50);
				var trz = _random.Next(-50, 50);

				Mesh mesh;
				switch (_random.Next(0, 4))
				{
					case 0:
						mesh = PrimitivesFactory.CreateCube();
						break;
					case 1:
						mesh = PrimitivesFactory.CreateCylinder();
						break;
					case 2:
						mesh = PrimitivesFactory.CreateSphere();
						break;
					default:
						mesh = PrimitivesFactory.CreateTorus();
						break;
				}

				Texture2D texture = null;
				var r = _random.Next(0, 6);
				if (r >= 1 && r <= 3)
				{
					texture = SampleAssets.SampleTexture1;
				}
				else if (r > 3)
				{
					texture = SampleAssets.SampleTexture2;
				}

				var instance = new ModelObject(mesh)
				{
					Scale = new Vector3(sr, sr, sr),
					Translate = new Vector3(trx, trs, trz),
					Material = new BaseMaterial
					{
						DiffuseColor = Color.FromNonPremultiplied(_random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255), 255),
						Texture = texture,
						HasLight = true
					}
				};

				var info = new ModelInfo
				{
					ModelObject = instance,
					Rotation = new Vector3(GenRotation(), GenRotation(), GenRotation())
				};

				_modelInstances.Add(info);
			}

			// Add everything to scene
			_scene = new Scene();
			foreach (var info in _modelInstances)
			{
				_scene.Items.Add(info.ModelObject);
			}

			GC.Collect();
		}

		private float GenRotation()
		{
			return _random.Next(-1, 2);
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

			// Rotate models
			foreach (var info in _modelInstances)
			{
				var model = info.ModelObject;
				model.RotationX += info.Rotation.X;
				model.RotationY += info.Rotation.Y;
				model.RotationZ += info.Rotation.Z;
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