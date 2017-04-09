using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra
{

	public static class MyraEnvironment
	{
		public delegate void LogHandler(string message);

		public static LogHandler DebugLogHandler { get; set; }
		public static LogHandler InfoLogHandler { get; set; }
		public static LogHandler WarningLogHandler { get; set; }
		public static LogHandler ErrorLogHandler { get; set; }
		
		private static Game _game;
		private static bool? _isOpenGL;

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

				if (_game != null)
				{
					_game.Disposed -= GameOnDisposed;
				}

				_game = value;

				if (_game != null)
				{
					_game.Disposed += GameOnDisposed;
				}
			}
		}

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
				var assembly = typeof (MyraEnvironment).GetTypeInfo().Assembly;
				var name = new AssemblyName(assembly.FullName);

				return name.Version.ToString();
			}
		}

		public static bool IsOpenGL
		{
			get
			{
				if (_isOpenGL == null)
				{
					_isOpenGL = (from f in typeof (GraphicsDevice).GetRuntimeFields() where f.Name == "glFramebuffer" select f).FirstOrDefault() != null;
				}

				return _isOpenGL.Value;
			}
		}

		private static string FormatMessage(string message, params object[] args)
		{
			string str;
			try
			{
				if (args != null && args.Length > 0)
				{
					str = string.Format(message, args);
				}
				else
				{
					str = message;
				}
			}
			catch (FormatException)
			{
				str = message;
			}

			return str;
		}

		public static void LogDebug(string message, params object[] args)
		{
			if (DebugLogHandler == null)
			{
				return;
			}

			DebugLogHandler(FormatMessage(message, args));
		}

		public static void LogInfo(string message, params object[] args)
		{
			if (InfoLogHandler == null)
			{
				return;
			}

			InfoLogHandler(FormatMessage(message, args));
		}

		public static void LogWarning(string message, params object[] args)
		{
			if (WarningLogHandler == null)
			{
				return;
			}

			WarningLogHandler(FormatMessage(message, args));
		}

		public static void LogError(string message, params object[] args)
		{
			if (ErrorLogHandler == null)
			{
				return;
			}

			ErrorLogHandler(FormatMessage(message, args));
		}
	}
}