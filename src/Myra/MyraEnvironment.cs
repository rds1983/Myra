using System;
using System.Reflection;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Engine;
using Xenko.Graphics;
#endif

namespace Myra
{

	public static class MyraEnvironment
	{
		private static Game _game;

		public static event EventHandler GameDisposed;

		public static Game Game
		{
			get
			{
				return _game;
			}

			set
			{
				if (_game == value)
				{
					return;
				}

#if !XENKO
				if (_game != null)
				{
					_game.Disposed -= GameOnDisposed;
				}
#endif

				_game = value;

#if !XENKO
				if (_game != null)
				{
					_game.Disposed += GameOnDisposed;
				}
#endif
			}
		}

		public static bool DrawWidgetsFrames { get; set; }
		public static bool DrawKeyboardFocusedWidgetFrame { get; set; }
		public static bool DrawMouseWheelFocusedWidgetFrame { get; set; }
		public static bool DrawTextGlyphsFrames { get; set; }
		public static bool DisableClipping { get; set; }
		public static bool DrawPartialLastSymbol { get; set; }

		private static void GameOnDisposed(object sender, EventArgs eventArgs)
		{
			DefaultAssets.Dispose();

			var ev = GameDisposed;
			if (ev != null)
			{
				ev(null, EventArgs.Empty);
			}
		}

		public static GraphicsDevice GraphicsDevice
		{
			get
			{
				return Game.GraphicsDevice;
			}
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
	}
}