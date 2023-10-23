using System;
using System.Collections.Generic;
using System.Drawing;
using info.lundin.math;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using Myra.Platform;

namespace Myra.Samples.AllWidgets
{
	internal class MGPlatform : IMyraPlatform
	{
		private static readonly Dictionary<MouseCursorType, MouseCursor> _mouseCursors = new Dictionary<MouseCursorType, MouseCursor>
		{
			[MouseCursorType.Arrow] = MouseCursor.Arrow,
			[MouseCursorType.IBeam] = MouseCursor.IBeam,
			[MouseCursorType.Wait] = MouseCursor.Wait,
			[MouseCursorType.Crosshair] = MouseCursor.Crosshair,
			[MouseCursorType.WaitArrow] = MouseCursor.WaitArrow,
			[MouseCursorType.SizeNWSE] = MouseCursor.SizeNWSE,
			[MouseCursorType.SizeNESW] = MouseCursor.SizeNESW,
			[MouseCursorType.SizeWE] = MouseCursor.SizeWE,
			[MouseCursorType.SizeNS] = MouseCursor.SizeNS,
			[MouseCursorType.SizeAll] = MouseCursor.SizeAll,
			[MouseCursorType.No] = MouseCursor.No,
			[MouseCursorType.Hand] = MouseCursor.Hand,
		};

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

		public void SetMouseCursorType(MouseCursorType mouseCursorType)
		{
			MouseCursor mouseCursor;
			if (!_mouseCursors.TryGetValue(mouseCursorType, out mouseCursor))
			{
				throw new Exception($"Could not find mouse cursor {mouseCursorType}");
			}

			Mouse.SetCursor(mouseCursor);
		}
	}
}
