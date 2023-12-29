using Myra.Graphics2D.UI.Styles;
using AssetManagementBase;
using System;

namespace Myra
{
	public static class DefaultAssets
	{
		private static AssetManager _assetManager;
		private static Stylesheet _defaultStylesheet, _defaultStylesheet2x;

		private static AssetManager AssetManager
		{
			get
			{
				if (_assetManager == null)
				{
					_assetManager = AssetManager.CreateResourceAssetManager(typeof(DefaultAssets).Assembly, "Resources.");
				}

				return _assetManager;
			}
		}

		[Obsolete("Use DefaultStylesheet")]
		public static Stylesheet UIStylesheet => DefaultStylesheet;

		public static Stylesheet DefaultStylesheet
		{
			get
			{
				if (_defaultStylesheet != null)
				{
					return _defaultStylesheet;
				}

				_defaultStylesheet = AssetManager.LoadStylesheet("default_ui_skin.xmms");
				return _defaultStylesheet;
			}
		}

		public static Stylesheet DefaultStylesheet2X
		{
			get
			{
				if (_defaultStylesheet2x != null)
				{
					return _defaultStylesheet2x;
				}

				_defaultStylesheet2x = AssetManager.LoadStylesheet("default_ui_skin_2x.xmms");
				return _defaultStylesheet2x;
			}
		}

		internal static void Dispose()
		{
			_defaultStylesheet = null;
			_defaultStylesheet2x = null;

			if (_assetManager != null)
			{
				_assetManager.Cache.Clear();
				_assetManager = null;
			}
		}
	}
}