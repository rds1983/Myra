using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using Myra.Graphics3D;
using Myra.Graphics3D.Materials;
using Myra.Graphics3D.Utils;
using Myra.Samples.Utility;
using DirectionalLight = Myra.Graphics3D.Environment.DirectionalLight;

namespace Myra.Samples
{
	public class Primitives3DSample : Game
	{
		private class ModelInfo
		{
			public ModelObject ModelObject;
			public Vector3 Rotate;
		}

		private readonly GraphicsDeviceManager graphics;
		private Scene _scene;
		private readonly Camera _camera;
		private readonly CameraInputController _cameraController;
		private readonly List<ModelInfo> _modelInstances = new List<ModelInfo>();
		private readonly DirectionalLight[] _lights;
		private Desktop _desktop;
		private StatisticsGrid3d _statisticsGrid;
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

			_statisticsGrid = new StatisticsGrid3d();
			_desktop.Widgets.Add(_statisticsGrid);

			// Init 3d stuff
			var grid = PrimitivesFactory.CreateXZGrid(new Vector2(100, 100));
			grid.Material = new BaseMaterial
			{
				DiffuseColor = Color.Gray
			};

			_modelInstances.Add(new ModelInfo
			{
				ModelObject = grid,
				Rotate = new Vector3(0, 0, 0)
			});

			for (var i = 0; i < 500; ++i)
			{
				var sr = _random.Next(1, 7);
				var trx = _random.Next(-50, 50);
				var trs = _random.Next(-50, 50);
				var trz = _random.Next(-50, 50);

				ModelObject instance;
				switch (_random.Next(0, 4))
				{
					case 0:
						instance = PrimitivesFactory.CreateCube();
						break;
					case 1:
						instance = PrimitivesFactory.CreateCylinder();
						break;
					case 2:
						instance = PrimitivesFactory.CreateSphere();
						break;
					default:
						instance = PrimitivesFactory.CreateTorus();
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

				instance.Scale = new Vector3(sr, sr, sr);
				instance.Translate = new Vector3(trx, trs, trz);
				instance.Material = new BaseMaterial
				{
					DiffuseColor = Color.FromNonPremultiplied(_random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255), 255),
					Texture = texture,
					HasLight = true
				};

				var info = new ModelInfo
				{
					ModelObject = instance,
					Rotate = new Vector3(GenRotation(), GenRotation(), GenRotation())
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
				model.Rotate += info.Rotate;
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

			_scene.Render(_camera, _statisticsGrid.IsLightningOn ? _lights : null, _statisticsGrid.RenderFlags);

			_desktop.Bounds = new Rectangle(0, 0,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_desktop.Render();

			_statisticsGrid.Update(GraphicsDevice, _scene);
		}
	}
}