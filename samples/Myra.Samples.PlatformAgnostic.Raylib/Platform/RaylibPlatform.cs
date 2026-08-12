using System.Drawing;
using Myra.Graphics2D.UI;
using Myra.Platform;
using Raylib_cs;

namespace Myra.Samples.AllWidgets
{
	internal class RaylibPlatform : IMyraPlatform
	{
		private readonly Renderer _renderer = new Renderer();
		private float _scrollWheelValue;

		public Point ViewSize => new Point(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

		public IMyraRenderer Renderer => _renderer;

		public MouseInfo GetMouseInfo()
		{
			var mousePos = Raylib.GetMousePosition();
			var wheelMove = Raylib.GetMouseWheelMove();

			// Accumulate mouse wheel value
			_scrollWheelValue += wheelMove;

			var result = new MouseInfo
			{
				Position = new Point((int)mousePos.X, (int)mousePos.Y),
				IsLeftButtonDown = Raylib.IsMouseButtonDown(MouseButton.Left),
				IsMiddleButtonDown = Raylib.IsMouseButtonDown(MouseButton.Middle),
				IsRightButtonDown = Raylib.IsMouseButtonDown(MouseButton.Right),
				Wheel = _scrollWheelValue
			};

			return result;
		}

		public void SetKeysDown(bool[] keys)
		{
			for (var i = 0; i < keys.Length; ++i)
			{
				var ks = (Keys)i;
				keys[i] = IsKeyDown(ks);
			}
		}

		public TouchCollection GetTouchState()
		{
			return TouchCollection.Empty;
		}

		public void SetMouseCursorType(MouseCursorType mouseCursorType)
		{
		}

		private bool IsKeyDown(Keys key)
		{
			return key switch
			{
				Keys.Back => Raylib.IsKeyDown(KeyboardKey.Backspace),
				Keys.Tab => Raylib.IsKeyDown(KeyboardKey.Tab),
				(Keys)13 => Raylib.IsKeyDown(KeyboardKey.Enter), // Return key
				Keys.Escape => Raylib.IsKeyDown(KeyboardKey.Escape),
				Keys.Space => Raylib.IsKeyDown(KeyboardKey.Space),
				Keys.PageUp => Raylib.IsKeyDown(KeyboardKey.PageUp),
				Keys.PageDown => Raylib.IsKeyDown(KeyboardKey.PageDown),
				Keys.End => Raylib.IsKeyDown(KeyboardKey.End),
				Keys.Home => Raylib.IsKeyDown(KeyboardKey.Home),
				Keys.Left => Raylib.IsKeyDown(KeyboardKey.Left),
				Keys.Up => Raylib.IsKeyDown(KeyboardKey.Up),
				Keys.Right => Raylib.IsKeyDown(KeyboardKey.Right),
				Keys.Down => Raylib.IsKeyDown(KeyboardKey.Down),
				Keys.Delete => Raylib.IsKeyDown(KeyboardKey.Delete),
				Keys.LeftShift => Raylib.IsKeyDown(KeyboardKey.LeftShift),
				Keys.RightShift => Raylib.IsKeyDown(KeyboardKey.RightShift),
				Keys.LeftControl => Raylib.IsKeyDown(KeyboardKey.LeftControl),
				Keys.RightControl => Raylib.IsKeyDown(KeyboardKey.RightControl),
				Keys.A => Raylib.IsKeyDown(KeyboardKey.A),
				Keys.B => Raylib.IsKeyDown(KeyboardKey.B),
				Keys.C => Raylib.IsKeyDown(KeyboardKey.C),
				Keys.D => Raylib.IsKeyDown(KeyboardKey.D),
				Keys.E => Raylib.IsKeyDown(KeyboardKey.E),
				Keys.F => Raylib.IsKeyDown(KeyboardKey.F),
				Keys.G => Raylib.IsKeyDown(KeyboardKey.G),
				Keys.H => Raylib.IsKeyDown(KeyboardKey.H),
				Keys.I => Raylib.IsKeyDown(KeyboardKey.I),
				Keys.J => Raylib.IsKeyDown(KeyboardKey.J),
				Keys.K => Raylib.IsKeyDown(KeyboardKey.K),
				Keys.L => Raylib.IsKeyDown(KeyboardKey.L),
				Keys.M => Raylib.IsKeyDown(KeyboardKey.M),
				Keys.N => Raylib.IsKeyDown(KeyboardKey.N),
				Keys.O => Raylib.IsKeyDown(KeyboardKey.O),
				Keys.P => Raylib.IsKeyDown(KeyboardKey.P),
				Keys.Q => Raylib.IsKeyDown(KeyboardKey.Q),
				Keys.R => Raylib.IsKeyDown(KeyboardKey.R),
				Keys.S => Raylib.IsKeyDown(KeyboardKey.S),
				Keys.T => Raylib.IsKeyDown(KeyboardKey.T),
				Keys.U => Raylib.IsKeyDown(KeyboardKey.U),
				Keys.V => Raylib.IsKeyDown(KeyboardKey.V),
				Keys.W => Raylib.IsKeyDown(KeyboardKey.W),
				Keys.X => Raylib.IsKeyDown(KeyboardKey.X),
				Keys.Y => Raylib.IsKeyDown(KeyboardKey.Y),
				Keys.Z => Raylib.IsKeyDown(KeyboardKey.Z),
				Keys.D0 => Raylib.IsKeyDown(KeyboardKey.Zero),
				Keys.D1 => Raylib.IsKeyDown(KeyboardKey.One),
				Keys.D2 => Raylib.IsKeyDown(KeyboardKey.Two),
				Keys.D3 => Raylib.IsKeyDown(KeyboardKey.Three),
				Keys.D4 => Raylib.IsKeyDown(KeyboardKey.Four),
				Keys.D5 => Raylib.IsKeyDown(KeyboardKey.Five),
				Keys.D6 => Raylib.IsKeyDown(KeyboardKey.Six),
				Keys.D7 => Raylib.IsKeyDown(KeyboardKey.Seven),
				Keys.D8 => Raylib.IsKeyDown(KeyboardKey.Eight),
				Keys.D9 => Raylib.IsKeyDown(KeyboardKey.Nine),
				_ => false
			};
		}
	}
}
