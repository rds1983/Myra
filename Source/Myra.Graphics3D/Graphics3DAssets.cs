using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.MultiCompileEffects;

namespace Myra.Graphics3D
{
	public static class Graphics3DAssets
	{
		private static AssetsContentManager _assetsContentManager;
		private static Texture2D _white;
		private static Texture2D _transparent;

		public static Texture2D White
		{
			get
			{
				if (_white == null)
				{
					_white = new Texture2D(MyraEnvironment.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
					_white.SetData(new[] { Color.White });
					return _white;
				}

				return _white;
			}
		}

		public static Texture2D Transparent
		{
			get
			{
				if (_transparent == null)
				{
					_transparent = new Texture2D(MyraEnvironment.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
					_transparent.SetData(new[] { Color.Transparent });
					return _transparent;
				}

				return _transparent;
			}
		}

		private static AssetsContentManager AssetsContentManager
		{
			get {
				return _assetsContentManager ?? (_assetsContentManager = new AssetsContentManager(MyraEnvironment.Game.Services));
			}
		}

		public static MultiCompileEffect DefaultEffect
		{
			get
			{
				return AssetsContentManager.Load<MultiCompileEffect>("DefaultEffect." +
																	  (MyraEnvironment.IsOpenGL ? "OpenGL" : "DirectX")
																	  + ".xnb");
			}
		}

		public static MultiCompileEffect TerrainEffect
		{
			get
			{
				return AssetsContentManager.Load<MultiCompileEffect>("TerrainEffect." +
																	  (MyraEnvironment.IsOpenGL ? "OpenGL" : "DirectX")
																	  + ".xnb");
			}
		}

		static Graphics3DAssets()
		{
			MyraEnvironment.GameDisposed += MyraEnvironmentOnGameDisposed;
		}

		private static void MyraEnvironmentOnGameDisposed(object sender, EventArgs eventArgs)
		{
			if (_white != null)
			{
				_white.Dispose();
				_white = null;
			}

			if (_transparent != null)
			{
				_transparent.Dispose();
				_transparent = null;
			}

			if (_assetsContentManager != null)
			{
				_assetsContentManager.Dispose();
				_assetsContentManager = null;
			}
		}
	}
}
