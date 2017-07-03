using Myra.Editor.Plugin;
using Myra.Graphics2D.UI.Styles;

namespace Myra.UIEditor.Plugin.LibGDX
{
	public class Plugin: IUIEditorPlugin
	{
		public void OnLoad()
		{
			// Load UI style sheet
			MyraEnvironment.Game.Content.RootDirectory = "Content";
			var stylesheet = MyraEnvironment.Game.Content.Load<Stylesheet>("ui_stylesheet");
			Stylesheet.Current = stylesheet;
		}
	}
}
