using System;
using System.Drawing;
using Myra.Graphics2D.UI;
using Myra.Platform;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GLFWKeys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using MyraKeys = Myra.Platform.Keys;

namespace Myra.Samples.AllWidgets
{
	internal class OpenTKPlatform : IMyraPlatform
	{
		private readonly GameWindow _gameWindow;
		private readonly Renderer _renderer;

		public Rectangle Viewport
		{
			get => _renderer.Viewport;
			set => _renderer.Viewport = value;
		}

		public Point ViewSize => new Point(_renderer.Viewport.Width, _renderer.Viewport.Height);

		public IMyraRenderer Renderer => _renderer;

		public OpenTKPlatform(GameWindow gameWindow)
		{
			if (gameWindow == null)
			{
				throw new ArgumentNullException(nameof(gameWindow));
			}

			_gameWindow = gameWindow;
			_renderer = new Renderer();
		}

		public MouseInfo GetMouseInfo()
		{
			var mouseState = _gameWindow.MouseState;

			var result = new MouseInfo
			{
				Position = new Point((int)mouseState.X, (int)mouseState.Y),
				IsLeftButtonDown = mouseState.IsButtonDown(MouseButton.Left),
				IsMiddleButtonDown = mouseState.IsButtonDown(MouseButton.Middle),
				IsRightButtonDown = mouseState.IsButtonDown(MouseButton.Right),
				Wheel = (int)mouseState.ScrollDelta.Y
			};

			return result;
		}

		public void SetKeysDown(bool[] keys)
		{
			var keyboardState = _gameWindow.KeyboardState;

			// Map OpenTK Keys (GLFW) to Myra Keys enum values
			keys[(int)MyraKeys.Back] = keyboardState.IsKeyDown(GLFWKeys.Backspace);
			keys[(int)MyraKeys.Tab] = keyboardState.IsKeyDown(GLFWKeys.Tab);
			keys[(int)MyraKeys.Enter] = keyboardState.IsKeyDown(GLFWKeys.Enter);
			keys[(int)MyraKeys.Escape] = keyboardState.IsKeyDown(GLFWKeys.Escape);
			keys[(int)MyraKeys.Space] = keyboardState.IsKeyDown(GLFWKeys.Space);
			keys[(int)MyraKeys.PageUp] = keyboardState.IsKeyDown(GLFWKeys.PageUp);
			keys[(int)MyraKeys.PageDown] = keyboardState.IsKeyDown(GLFWKeys.PageDown);
			keys[(int)MyraKeys.End] = keyboardState.IsKeyDown(GLFWKeys.End);
			keys[(int)MyraKeys.Home] = keyboardState.IsKeyDown(GLFWKeys.Home);
			keys[(int)MyraKeys.Left] = keyboardState.IsKeyDown(GLFWKeys.Left);
			keys[(int)MyraKeys.Up] = keyboardState.IsKeyDown(GLFWKeys.Up);
			keys[(int)MyraKeys.Right] = keyboardState.IsKeyDown(GLFWKeys.Right);
			keys[(int)MyraKeys.Down] = keyboardState.IsKeyDown(GLFWKeys.Down);
			keys[(int)MyraKeys.Insert] = keyboardState.IsKeyDown(GLFWKeys.Insert);
			keys[(int)MyraKeys.Delete] = keyboardState.IsKeyDown(GLFWKeys.Delete);

			// Number keys
			for (int i = 0; i <= 9; i++)
			{
				keys[(int)MyraKeys.D0 + i] = keyboardState.IsKeyDown((GLFWKeys)(48 + i));
			}

			// Letter keys (A-Z)
			for (int i = 0; i < 26; i++)
			{
				keys[(int)MyraKeys.A + i] = keyboardState.IsKeyDown((GLFWKeys)(65 + i));
			}

			// Function keys (F1-F12)
			for (int i = 0; i < 12; i++)
			{
				keys[(int)MyraKeys.F1 + i] = keyboardState.IsKeyDown((GLFWKeys)(290 + i));
			}

			// Modifier keys
			keys[(int)MyraKeys.LeftShift] = keyboardState.IsKeyDown(GLFWKeys.LeftShift);
			keys[(int)MyraKeys.RightShift] = keyboardState.IsKeyDown(GLFWKeys.RightShift);
			keys[(int)MyraKeys.LeftControl] = keyboardState.IsKeyDown(GLFWKeys.LeftControl);
			keys[(int)MyraKeys.RightControl] = keyboardState.IsKeyDown(GLFWKeys.RightControl);
			keys[(int)MyraKeys.LeftAlt] = keyboardState.IsKeyDown(GLFWKeys.LeftAlt);
			keys[(int)MyraKeys.RightAlt] = keyboardState.IsKeyDown(GLFWKeys.RightAlt);
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
