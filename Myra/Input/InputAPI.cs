using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using Myra.Utility;

namespace Myra.Input
{
	public static class InputAPI
	{
		private static DateTime _lastUpdateStamp = DateTime.MinValue;

		public static Point MousePosition { get; private set; }
		public static MouseState MouseState { get; private set; }

		public static event EventHandler<GenericEventArgs<Point>> MouseMoved;
		public static event EventHandler<GenericEventArgs<MouseButtons>> MouseDown;
		public static event EventHandler<GenericEventArgs<MouseButtons>> MouseUp;

		public static event EventHandler<GenericEventArgs<char>> KeyPressed;
		public static event EventHandler<GenericEventArgs<Keys>> KeyUp;
		public static event EventHandler<GenericEventArgs<Keys>> KeyDown;

		private static void HandleButton(ButtonState buttonState, ButtonState lastState, MouseButtons buttons)
		{
			if (buttonState == ButtonState.Pressed && lastState == ButtonState.Released)
			{
				var ev = MouseDown;
				if (ev != null)
				{
					ev(null, new GenericEventArgs<MouseButtons>(buttons));
				}
			}
			else if(buttonState == ButtonState.Released && lastState == ButtonState.Pressed)
			{
				var ev = MouseUp;
				if (ev != null)
				{
					ev(null, new GenericEventArgs<MouseButtons>(buttons));
				}
			}
		}

		public static void Update()
		{
			var now = DateTime.Now;
			if (_lastUpdateStamp == now)
			{
				return;
			}

			_lastUpdateStamp = now;

			var lastState = MouseState;

			MouseState = Mouse.GetState();
			MousePosition = MouseState.Position;

			if (MouseState.X != lastState.X || MouseState.Y != lastState.Y)
			{
				// Mouse position changed
				var ev = MouseMoved;
				if (ev != null)
				{
					ev(null, new GenericEventArgs<Point>(new Point(MouseState.X, MouseState.Y)));
				}
			}

			HandleButton(MouseState.LeftButton, lastState.LeftButton, MouseButtons.Left);
			HandleButton(MouseState.MiddleButton, lastState.MiddleButton, MouseButtons.Middle);
			HandleButton(MouseState.RightButton, lastState.RightButton, MouseButtons.Right);
		}

/*		protected void FireKeyUp(Keys key)
		{
			var ev = KeyUp;

			if (ev != null)
			{
				ev(this, new GenericEventArgs<Keys>(key));
			}
		}

		protected void FireKeyDown(Keys key)
		{
			var ev = KeyDown;

			if (ev != null)
			{
				ev(this, new GenericEventArgs<Keys>(key));
			}
		}

		protected void FireKeyPressed(char ch)
		{
			var ev = KeyPressed;

			if (ev != null)
			{
				ev(this, new GenericEventArgs<char>(ch));
			}
		}*/
	}
}