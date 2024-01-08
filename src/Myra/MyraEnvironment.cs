using System;
using System.Reflection;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using AssetManagementBase;
using Myra.Graphics2D.UI;
using System.Collections.Generic;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if FNA
using static SDL2.SDL;
using MouseCursor = System.Nullable<System.IntPtr>;
#endif

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
	public static class MyraEnvironment
	{
#if MONOGAME
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
#elif FNA
		private static readonly Dictionary<SDL_SystemCursor, IntPtr> _systemCursors = new Dictionary<SDL_SystemCursor, IntPtr>();

		private static IntPtr GetSystemCursor(SDL_SystemCursor type)
		{
			IntPtr result;
			if (_systemCursors.TryGetValue(type, out result))
			{
				return result;
			}

			result = SDL_CreateSystemCursor(type);
			_systemCursors[type] = result;

			return result;
		}

		private static readonly Dictionary<MouseCursorType, SDL_SystemCursor> _mouseCursors = new Dictionary<MouseCursorType, SDL_SystemCursor>
		{
			[MouseCursorType.Arrow] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_ARROW,
			[MouseCursorType.IBeam] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_IBEAM,
			[MouseCursorType.Wait] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_WAIT,
			[MouseCursorType.Crosshair] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_CROSSHAIR,
			[MouseCursorType.WaitArrow] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_WAITARROW,
			[MouseCursorType.SizeNWSE] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENWSE,
			[MouseCursorType.SizeNESW] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENESW,
			[MouseCursorType.SizeWE] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEWE,
			[MouseCursorType.SizeNS] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENS,
			[MouseCursorType.SizeAll] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEALL,
			[MouseCursorType.No] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_NO,
			[MouseCursorType.Hand] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_HAND,
		};
#endif

		private static MouseCursorType _mouseCursorType;
		private static AssetManager _defaultAssetManager;

		public static MouseCursorType MouseCursorType
		{
			get => _mouseCursorType;
			set
			{
				if (_mouseCursorType == value)
				{
					return;
				}

				_mouseCursorType = value;
#if MONOGAME
				MouseCursor mouseCursor;
				if (!_mouseCursors.TryGetValue(value, out mouseCursor))
				{
					throw new Exception($"Could not find mouse cursor {value}");
				}

				Mouse.SetCursor(mouseCursor);
#elif FNA
				SDL_SystemCursor mouseCursor;
				if (!_mouseCursors.TryGetValue(value, out mouseCursor))
				{
					throw new Exception($"Could not find mouse cursor {value}");
				}

				var mouseCursorPtr = GetSystemCursor(mouseCursor);
				SDL2.SDL.SDL_SetCursor(mouseCursorPtr);
#elif PLATFORM_AGNOSTIC
				Platform.SetMouseCursorType(value);
#endif
			}
		}

		public static MouseCursorType DefaultMouseCursorType { get; set; }

#if MONOGAME || FNA || STRIDE

		private static Game _game;

		public static Game Game
		{
			get
			{
				if (_game == null)
				{
					throw new Exception("MyraEnvironment.Game is null. Please, set it to the Game instance before using Myra.");
				}

				return _game;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (_game == value)
				{
					return;
				}

#if !STRIDE
				if (_game != null)
				{
					_game.Disposed -= GameOnDisposed;
				}
#endif

				_game = value;

#if !STRIDE
				if (_game != null)
				{
					_game.Disposed += GameOnDisposed;
				}
#endif
			}
		}

		public static GraphicsDevice GraphicsDevice
		{
			get => Game.GraphicsDevice;
		}
#else

		private static IMyraPlatform _platform;

		public static IMyraPlatform Platform
		{
			get
			{
				if (_platform == null)
				{
					throw new Exception("MyraEnvironment.Platform is null. Please, set it before using Myra.");
				}

				return _platform;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_platform = value;
			}
		}
#endif

		/// <summary>
		/// Default Assets Manager
		/// </summary>
		public static AssetManager DefaultAssetManager
		{
			get
			{
				if (_defaultAssetManager == null)
				{
					_defaultAssetManager = AssetManager.CreateFileAssetManager(PathUtils.ExecutingAssemblyDirectory);
				}

				return _defaultAssetManager;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));

				}
				_defaultAssetManager = value;
			}
		}

		public static bool DrawWidgetsFrames { get; set; }
		public static bool DrawKeyboardFocusedWidgetFrame { get; set; }
		public static bool DrawMouseHoveredWidgetFrame { get; set; }
		public static bool DrawTextGlyphsFrames { get; set; }
		public static bool DisableClipping { get; set; }

		public static Func<MouseInfo> MouseInfoGetter { get; set; } = DefaultMouseInfoGetter;
		public static Action<bool[]> DownKeysGetter { get; set; } = DefaultDownKeysGetter;

		public static int DoubleClickIntervalInMs { get; set; } = 500;
		public static int DoubleClickRadius { get; set; } = 2;
		public static int TooltipDelayInMs { get; set; } = 500;
		public static Point TooltipOffset { get; set; } = new Point(0, 20);
		public static Func<Widget, Widget> TooltipCreator { get; set; } = w =>
		{
			var tooltip = new Label(null)
			{
				Text = w.Tooltip,
				Tag = w
			};

			tooltip.ApplyLabelStyle(Stylesheet.Current.TooltipStyle);

			return tooltip;
		};

		/// <summary>
		/// Makes the text rendering more smooth(especially when scaling) for the cost of sacrificing some performance 
		/// </summary>
		public static bool SmoothText { get; set; }
		public static bool EnableModalDarkening { get; set; }

		public static Color DarkeningColor { get; set; } = new Color(0, 0, 0, 192);

		private static void GameOnDisposed(object sender, EventArgs eventArgs)
		{
			Reset();
		}

		/// <summary>
		/// 
		/// </summary>
		public static void Reset()
		{
			DefaultAssets.Dispose();
			Stylesheet.Current = null;
		}

		public static string Version
		{
			get
			{
				var assembly = typeof(MyraEnvironment).Assembly;
				var name = new AssemblyName(assembly.FullName);

				return name.Version.ToString();
			}
		}

		internal static string InternalClipboard;

		public static MouseInfo DefaultMouseInfoGetter()
		{
#if MONOGAME || FNA
			var state = Mouse.GetState();

			return new MouseInfo
			{
				Position = new Point(state.X, state.Y),
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