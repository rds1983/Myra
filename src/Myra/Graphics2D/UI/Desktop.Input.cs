using Myra.Utility;
using System;
using Myra.Events;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
#if MONOGAME
using MonoGame.Framework.Utilities;
#endif
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Drawing;
using Myra.Platform;
using System.Numerics;
using Matrix = System.Numerics.Matrix3x2;
#endif

namespace Myra.Graphics2D.UI
{
	public struct MouseInfo
	{
		public Point Position;
		public bool IsLeftButtonDown;
		public bool IsMiddleButtonDown;
		public bool IsRightButtonDown;
		public float Wheel;
	}

	partial class Desktop: IInputEventsProcessor
	{
		private MouseInfo _lastMouseInfo;
		private DateTime? _lastKeyDown;
		private int _keyDownCount = 0;
		private readonly bool[] _downKeys = new bool[0xff], _lastDownKeys = new bool[0xff];
		private Point _mousePosition;
		private Point? _touchPosition;
		private float _mouseWheelDelta;

		public Point PreviousMousePosition { get; private set; }
		public Point? PreviousTouchPosition { get; private set; }

		public Point MousePosition
		{
			get => _mousePosition;
			private set
			{
				if (value == _mousePosition)
				{
					return;
				}

				_mousePosition = value;
				InputEventsManager.Queue(this, InputEventType.MouseMoved);
			}
		}

		public Point? TouchPosition
		{
			get => _touchPosition;

			private set
			{
				if (value == _touchPosition)
				{
					return;
				}

				var oldValue = _touchPosition;
				_touchPosition = value;

				if (value != null && oldValue == null)
				{
					InputEventsManager.Queue(this, InputEventType.TouchDown);
				}
				else if (value == null && oldValue != null)
				{
					InputEventsManager.Queue(this, InputEventType.TouchUp);
				}
				else if (value != null && oldValue != null &&
					value.Value != oldValue.Value)
				{
					InputEventsManager.Queue(this, InputEventType.TouchMoved);
				}
			}
		}

		public bool IsTouchDown => TouchPosition != null;

		public float MouseWheelDelta
		{
			get => _mouseWheelDelta;

			set
			{
				_mouseWheelDelta = value;

				if (!value.IsZero())
				{
					InputEventsManager.Queue(this, InputEventType.MouseWheel);
				}
			}
		}

		public bool[] DownKeys => _downKeys;
		public int RepeatKeyDownStartInMs { get; set; } = 500;

		public int RepeatKeyDownInternalInMs { get; set; } = 50;

		public static bool IsMobile
		{
			get
			{
#if MONOGAME
				return PlatformInfo.MonoGamePlatform == MonoGamePlatform.Android ||
					PlatformInfo.MonoGamePlatform == MonoGamePlatform.iOS;
#else
				return false;
#endif
			}
		}

		public event EventHandler MouseMoved;

		public event EventHandler TouchMoved;
		public event EventHandler TouchDown;
		public event EventHandler TouchUp;
		public event EventHandler TouchDoubleClick;

		public event EventHandler<GenericEventArgs<float>> MouseWheelChanged;

		public event EventHandler<GenericEventArgs<Keys>> KeyUp;
		public event EventHandler<GenericEventArgs<Keys>> KeyDown;
		public event EventHandler<GenericEventArgs<char>> Char;

		public void UpdateMouseInput()
		{
			if (MyraEnvironment.MouseInfoGetter == null)
			{
				return;
			}

			var mouseInfo = MyraEnvironment.MouseInfoGetter();

			// Mouse Position
			MousePosition = mouseInfo.Position;

			// Touch Position
			Point? touchPosition = null;
			if (mouseInfo.IsLeftButtonDown || mouseInfo.IsRightButtonDown || mouseInfo.IsMiddleButtonDown)
			{
				// Touch by mouse
				touchPosition = MousePosition;
			}

			TouchPosition = touchPosition;

#if STRIDE
			var handleWheel = mouseInfo.Wheel != 0;
#else
			var handleWheel = mouseInfo.Wheel != _lastMouseInfo.Wheel;
#endif

			if (handleWheel)
			{
				var delta = mouseInfo.Wheel;
#if !STRIDE
				delta -= _lastMouseInfo.Wheel;
#endif
				MouseWheelDelta = delta;
			}
			else
			{
				MouseWheelDelta = 0;
			}

			_lastMouseInfo = mouseInfo;
		}

#if MONOGAME || FNA || PLATFORM_AGNOSTIC
		public void UpdateTouchInput()
		{
#if MONOGAME || FNA
			var touchState = TouchPanel.GetState();
#else
			var touchState = MyraEnvironment.Platform.GetTouchState();
#endif

			if (touchState.IsConnected && touchState.Count > 0)
			{
				var pos = touchState[0].Position;
				TouchPosition = new Point((int)pos.X, (int)pos.Y);
			}
			else
			{
				TouchPosition = null;
			}
		}
#endif

		public void UpdateKeyboardInput()
		{
			if (MyraEnvironment.DownKeysGetter == null)
			{
				return;
			}

			MyraEnvironment.DownKeysGetter(_downKeys);

			var now = DateTime.Now;
			for (var i = 0; i < _downKeys.Length; ++i)
			{
				var key = (Keys)i;
				if (_downKeys[i] && !_lastDownKeys[i])
				{
					if (key == Keys.Tab)
					{
						FocusNextWidget();
					}

					KeyDownHandler?.Invoke(key);

					_lastKeyDown = now;
					_keyDownCount = 0;
				}
				else if (!_downKeys[i] && _lastDownKeys[i])
				{
					// Key had been released
					KeyUp.Invoke(key);
					if (_focusedKeyboardWidget != null)
					{
						_focusedKeyboardWidget.OnKeyUp(key);
					}

					_lastKeyDown = null;
					_keyDownCount = 0;
				}
				else if (_downKeys[i] && _lastDownKeys[i])
				{
					if (_lastKeyDown != null &&
									  ((_keyDownCount == 0 && (now - _lastKeyDown.Value).TotalMilliseconds > RepeatKeyDownStartInMs) ||
									  (_keyDownCount > 0 && (now - _lastKeyDown.Value).TotalMilliseconds > RepeatKeyDownInternalInMs)))
					{
						KeyDownHandler?.Invoke(key);

						_lastKeyDown = now;
						++_keyDownCount;
					}
				}
			}

			Array.Copy(_downKeys, _lastDownKeys, _downKeys.Length);
		}

		public void UpdateInput()
		{
			UpdateKeyboardInput();

			PreviousMousePosition = MousePosition;
			PreviousTouchPosition = TouchPosition;

			if (!IsMobile)
			{
				UpdateMouseInput();
			}
			else
			{
#if MONOGAME || FNA || PLATFORM_AGNOSTIC
				try
				{
					UpdateTouchInput();
				}
				catch (Exception)
				{
				}
#endif
			}
		}

		void IInputEventsProcessor.ProcessEvent(InputEventType eventType)
		{
			switch (eventType)
			{
				case InputEventType.MouseLeft:
					break;
				case InputEventType.MouseEntered:
					break;
				case InputEventType.MouseMoved:
					MouseMoved.Invoke(this);
					break;
				case InputEventType.MouseWheel:
					MouseWheelChanged.Invoke(this, MouseWheelDelta);
					break;
				case InputEventType.TouchLeft:
					break;
				case InputEventType.TouchEntered:
					break;
				case InputEventType.TouchMoved:
					TouchMoved.Invoke(this);
					break;
				case InputEventType.TouchDown:
					InputOnTouchDown();
					TouchDown.Invoke(this);
					break;
				case InputEventType.TouchUp:
					TouchUp.Invoke(this);
					break;
				case InputEventType.TouchDoubleClick:
					TouchDoubleClick.Invoke(this);
					break;
			}
		}
	}
}