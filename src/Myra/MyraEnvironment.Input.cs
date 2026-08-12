using Myra.Graphics2D.UI;
using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Engine;
using Stride.Graphics;
using Stride.Core.Mathematics;
using Stride.Input;
#else
using Myra.Platform;
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra
{
	partial class MyraEnvironment
	{
		/// <summary>
		/// Gets or sets the function used to retrieve the current mouse information.
		/// </summary>
		public static Func<MouseInfo> MouseInfoGetter { get; set; } = DefaultMouseInfoGetter;

		/// <summary>
		/// Gets or sets the function used to retrieve which keys are currently pressed.
		/// </summary>
		public static Action<bool[]> DownKeysGetter { get; set; } = DefaultDownKeysGetter;

		/// <summary>
		/// Gets or sets the time interval in milliseconds for double-click detection.
		/// </summary>
		public static int DoubleClickIntervalInMs { get; set; } = 500;

		/// <summary>
		/// Gets or sets the pixel radius within which a second click is considered a double-click.
		/// </summary>
		public static int DoubleClickRadius { get; set; } = 2;

		/// <summary>
		/// Gets or sets the delay in milliseconds before a pressed key starts repeating. Defaults to <c>500</c>.
		/// </summary>
		public static int RepeatKeyDownStartInMs { get; set; } = 500;

		/// <summary>
		/// Gets or sets the interval in milliseconds between key repeat events. Defaults to <c>50</c>.
		/// </summary>
		public static int RepeatKeyDownInternalInMs { get; set; } = 50;

		/// <summary>
		/// Gets the current mouse information from the game instance.
		/// </summary>
		/// <returns>A MouseInfo struct containing the current mouse position and button states.</returns>
		public static MouseInfo DefaultMouseInfoGetter()
		{
#if MONOGAME || FNA
			var state = Mouse.GetState();

			var pos = new Point(state.X - GraphicsDevice.Viewport.X, state.Y - GraphicsDevice.Viewport.Y);

			return new MouseInfo
			{
				Position = pos,
				IsLeftButtonDown = Game.IsActive && state.LeftButton == ButtonState.Pressed,
				IsMiddleButtonDown = Game.IsActive && state.MiddleButton == ButtonState.Pressed,
				IsRightButtonDown = Game.IsActive && state.RightButton == ButtonState.Pressed,
				Wheel = state.ScrollWheelValue
			};
#elif STRIDE
			var input = Game.Input;

			var v = input.AbsoluteMousePosition;

			return new MouseInfo
			{
				Position = new Point((int)v.X, (int)v.Y),
				IsLeftButtonDown = input.IsMouseButtonDown(MouseButton.Left),
				IsMiddleButtonDown = input.IsMouseButtonDown(MouseButton.Middle),
				IsRightButtonDown = input.IsMouseButtonDown(MouseButton.Right),
				Wheel = input.MouseWheelDelta
			};
#else
			return Platform.GetMouseInfo();
#endif
		}

		/// <summary>
		/// Gets which keyboard keys are currently pressed from the game instance.
		/// </summary>
		/// <param name="keys">An array to be filled with boolean values indicating which keys are pressed.</param>
		public static void DefaultDownKeysGetter(bool[] keys)
		{
#if MONOGAME || FNA
			var state = Keyboard.GetState();
			for (var i = 0; i < keys.Length; ++i)
			{
				keys[i] = state.IsKeyDown((Keys)i);
			}
#elif STRIDE
			var input = Game.Input;
			for (var i = 0; i < keys.Length; ++i)
			{
				keys[i] = input.IsKeyDown((Keys)i);
			}
#else
			Platform.SetKeysDown(keys);
#endif
		}
	}
}
