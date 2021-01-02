using System;
using System.Reflection;
using AssetManagementBase;
using AssetManagementBase.Utility;
using Myra.Platform;
using Myra.Assets;
using Myra.Platform.XNA;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Engine;
using Stride.Graphics;
#endif

namespace Myra
{
	public static class MyraEnvironment
	{
		private static AssetManager _defaultAssetManager;
		private static IMyraPlatform _platform;
		private static bool _assetsLoadersUpdated = false;

		public static event EventHandler GameDisposed;

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
				_platform = new XNAPlatform(_game.GraphicsDevice);

#if !STRIDE
				if (_game != null)
				{
					_game.Disposed += GameOnDisposed;
				}
#endif
				if (!_assetsLoadersUpdated)
				{
					AssetManager.SetAssetLoader(new StaticSpriteFontLoader());
					AssetManager.SetAssetLoader(new FontSystemLoader());
					AssetManager.SetAssetLoader(new DynamicSpriteFontLoader());
					AssetManager.SetAssetLoader(new SpriteFontBaseLoader());

					_assetsLoadersUpdated = true;
				}
			}
		}
#endif

		public static IMyraPlatform Platform
		{
			get
			{
				if (_platform == null)
				{
					throw new Exception("MyraEnvironment.Game is null. Please, set it to the Game instance before using Myra.");
				}

				return _platform;
			}

			private set
			{
				_platform = value;
			}
		}

		/// <summary>
		/// Default Assets Manager
		/// </summary>
		public static AssetManager DefaultAssetManager
		{
			get
			{
				if (_defaultAssetManager == null)
				{
					_defaultAssetManager = new AssetManager(new FileAssetResolver(PathUtils.ExecutingAssemblyDirectory));
				}

				return _defaultAssetManager;
			}
		}

		public static bool DrawWidgetsFrames { get; set; }
		public static bool DrawKeyboardFocusedWidgetFrame { get; set; }
		public static bool DrawMouseWheelFocusedWidgetFrame { get; set; }
		public static bool DrawTextGlyphsFrames { get; set; }
		public static bool DisableClipping { get; set; }

		private static void GameOnDisposed(object sender, EventArgs eventArgs)
		{
			DefaultAssets.Dispose();

			var ev = GameDisposed;
			if (ev != null)
			{
				ev(null, EventArgs.Empty);
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

		/// <summary>
		/// Applies an overall scaling factor to adjust the size and spacing of widgets on devices with high definition displays
		/// </summary>
		public static float? LayoutScale { get; set; }
	}
}