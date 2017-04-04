using System.Reflection;
using Myra.Assets;
using Myra.Editor.Plugin;
using Myra.Graphics2D.UI.Styles;

namespace Myra.UIEditor.Plugin.LibGDX
{
	public class Plugin: IUIEditorPlugin
	{
		private readonly AssetManager _assetManager = new AssetManager(new ResourceAssetResolver(typeof(Plugin).GetTypeInfo().Assembly, "Myra.UIEditor.Plugin.LibGDX.Resources."));

		public void OnLoad()
		{
			// Load UI style sheet
			var stylesheet = _assetManager.Load<Stylesheet>("uiskin.json");

			DefaultAssets.UIStylesheet = stylesheet;
		}
	}
}
