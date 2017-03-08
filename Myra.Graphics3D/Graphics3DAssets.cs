using System;
using Myra.Graphics;

namespace Myra.Graphics3D
{
	public static class Graphics3DAssets
	{
		private static AssetsContentManager _assetsContentManager;

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
			if (_assetsContentManager != null)
			{
				_assetsContentManager.Dispose();
				_assetsContentManager = null;
			}
		}
	}
}
