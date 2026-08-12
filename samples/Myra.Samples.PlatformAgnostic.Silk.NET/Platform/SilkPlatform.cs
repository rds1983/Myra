using System;
using System.Collections.Generic;
using System.Drawing;
using Myra.Graphics2D.UI;
using Myra.Platform;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace Myra.Samples.AllWidgets
{
	internal class SilkPlatform : IMyraPlatform
	{
		private static readonly Dictionary<Keys, Key> _keysMap = new Dictionary<Keys, Key>();

		private readonly IInputContext _inputContext;
		private readonly Renderer _renderer;

		public Rectangle Viewport
		{
			get => _renderer.Viewport;
			set => _renderer.Viewport = value;
		}

		public Point ViewSize => new Point(_renderer.Viewport.Width, _renderer.Viewport.Height);

		public IMyraRenderer Renderer => _renderer;

		static SilkPlatform()
		{
			// Fill _keysMap matching names
			var keysValues = Enum.GetValues(typeof(Keys));
			foreach (Keys keys in keysValues)
			{
				var name = Enum.GetName(typeof(Keys), keys);

				Key key;
				if (Enum.TryParse(name, out key))
				{
					_keysMap[keys] = key;
				}
			}

			_keysMap[Keys.Back] = Key.Backspace;
			_keysMap[Keys.LeftShift] = Key.ShiftLeft;
			_keysMap[Keys.RightShift] = Key.ShiftRight;
		}

		public SilkPlatform(IInputContext inputContext)
		{
			if (inputContext == null)
			{
				throw new ArgumentNullException(nameof(inputContext));
			}

			_inputContext = inputContext;
			_renderer = new Renderer();
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

			for (var i = 0; i < keys.Length; ++i)
			{
				var ks = (Keys)i;
				Key key;
				if (_keysMap.TryGetValue(ks, out key))
				{
					keys[i] = state.IsKeyPressed(key);
				}
				else
				{
					keys[i] = false;
				}
			}
		}

		public TouchCollection GetTouchState()
		{
			// Do not bother with accurately returning touch state for now
			return TouchCollection.Empty;
		}

		public void SetMouseCursorType(MouseCursorType mouseCursorType)
		{
		}
	}
}
