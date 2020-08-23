using System;
using System.Reflection;
using XNAssets;
using Myra.Assets;
using XNAssets.Utility;

#if !STRIDE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Stride.Engine;
using Stride.Graphics;
#endif

namespace Myra
{
	public static class MyraEnvironment
	{
		private static AssetManager _defaultAssetManager;

		private static bool _assetsLoadersUpdated = false;
		private static Game _game;

		public static event EventHandler GameDisposed;

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

				if (!_assetsLoadersUpdated)
				{
					// Use our own SpriteFontLoader that can use TextureRegion as backing image
					AssetManager.SetAssetLoader(new SpriteFontLoader());

					_assetsLoadersUpdated = true;
				}
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
					_defaultAssetManager = new AssetManager(GraphicsDevice, new FileAssetResolver(PathUtils.ExecutingAssemblyDirectory));
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

		/// <summary>
		/// Applies an overall scaling factor to adjust the size and spacing of widgets on devices with high definition displays
		/// </summary>
		public static float? LayoutScale { get; set; }
	}
}