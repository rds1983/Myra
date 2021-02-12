using System;
using System.Drawing;
using Myra.Graphics2D.UI;
using Myra.Platform;
using Silk.NET.Input.Common;
using Silk.NET.OpenGL;
using TrippyGL;

namespace Myra.Samples.AllWidgets
{
	internal class TrippyPlatform : IMyraPlatform
	{
		private readonly GraphicsDevice _device;
		private readonly IInputContext _inputContext;

		public Point ViewSize
		{
			get
			{
				return new Point((int)_device.Viewport.Width, (int)_device.Viewport.Height);
			}
		}

		public TrippyPlatform(GraphicsDevice device, IInputContext inputContext)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}

			if (inputContext == null)
			{
				throw new ArgumentNullException(nameof(inputContext));
			}

			_device = device;
			_inputContext = inputContext;
		}

		public object CreateTexture(int width, int height)
		{
			var texture2d = new Texture2D(_device, (uint)width, (uint)height);

			return texture2d;
		}

		public void SetTextureData(object texture, Rectangle bounds, byte[] data)
		{
			var xnaTexture = (Texture2D)texture;

			xnaTexture.SetData<byte>(data, bounds.X, bounds.Y, (uint)bounds.Width, (uint)bounds.Height, PixelFormat.Rgba);
		}

		public IMyraRenderer CreateRenderer()
		{
			return new TrippyRenderer(_device);
		}

		public MouseInfo GetMouseInfo()
		{
			var state = _inputContext.Mice[0];

			var result = new MouseInfo
			{
				Position = new Point((int)state.Position.X, (int)state.Position.Y),
				IsLeftButtonDown = state.IsButtonPressed(MouseButton.Left),
				IsMiddleButtonDown = state.IsButtonPressed(MouseButton.Middle),
				IsRightButtonDown = state.IsButtonPressed(MouseButton.Right),
				Wheel = state.ScrollWheels[0].X
			};

			return result;
		}

		public void SetKeysDown(bool[] keys)
		{
			var state = _inputContext.Keyboards[0];

			return;

			for (var i = 0; i < keys.Length; ++i)
			{
				keys[i] = state.IsKeyPressed((Key)i);
			}
		}

		public TouchCollection GetTouchState()
		{
			// Do not bother with accurately returning touch state for now
			return TouchCollection.Empty;
		}
	}
}
