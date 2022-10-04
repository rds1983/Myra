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
		private readonly GraphicsDevice _device;
		private readonly MGRenderer _renderer;

		public Point ViewSize
		{
			get
			{
				return new Point(_device.Viewport.Width, _device.Viewport.Height);
			}
		}

		public IMyraRenderer Renderer => _renderer;

		internal GraphicsDevice GraphicsDevice => _device;

		public MGPlatform(GraphicsDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}

			_device = device;
			_renderer = new MGRenderer(this);
		}

		public object CreateTexture(int width, int height)
		{
			var texture2d = new Texture2D(_device, width, height);

			return texture2d;
		}

		public void SetTextureData(object texture, Rectangle bounds, byte[] data)
		{
			var xnaTexture = (Texture2D)texture;
			xnaTexture.SetData(0, bounds.ToXNA(), data, 0, bounds.Width * bounds.Height * 4);
		}

		public Point GetTextureSize(object texture)
		{
			var xnaTexture = (Texture2D)texture;
			return new Point(xnaTexture.Width, xnaTexture.Height);
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
