using System;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using Myra.Platform;

namespace Myra.Samples.AllWidgets
{
	internal class MGPlatform : IMyraPlatform
	{
		private readonly MGRenderer _renderer;

		public Point ViewSize
		{
			get
			{
				return new Point(_renderer.GraphicsDevice.Viewport.Width,
					_renderer.GraphicsDevice.Viewport.Height);
			}
		}

		public IMyraRenderer Renderer => _renderer;

		public MGPlatform(GraphicsDevice device)
		{
			_renderer = new MGRenderer(device);
		}

		public MouseInfo GetMouseInfo()
		{
			var state = Mouse.GetState();

			var result = new MouseInfo
			{
				Position = new Point(state.X, state.Y),
				IsLeftButtonDown = state.LeftButton == ButtonState.Pressed,
				IsMiddleButtonDown = state.MiddleButton == ButtonState.Pressed,
				IsRightButtonDown = state.RightButton == ButtonState.Pressed,
				Wheel = state.ScrollWheelValue
			};

			return result;
		}

		public void SetKeysDown(bool[] keys)
		{
			var state = Keyboard.GetState();
			for (var i = 0; i < keys.Length; ++i)
			{
				keys[i] = state.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)i);
			}
		}

		public TouchCollection GetTouchState()
		{
			// Do not bother with accurately returning touch state for now
			return TouchCollection.Empty;
		}
	}
}
