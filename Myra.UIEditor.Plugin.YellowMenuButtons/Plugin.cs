using Microsoft.Xna.Framework;
using Myra.Editor.Plugin;

namespace Myra.UIEditor.Plugin.YellowMenuButtons
{
	public class Plugin: IUIEditorPlugin
	{
		public void OnLoad()
		{
			var result = DefaultAssets.UIStylesheet;

			result.HorizontalMenuStyle.MenuItemStyle.LabelStyle.TextColor = Color.Yellow;
			result.VerticalMenuStyle.MenuItemStyle.LabelStyle.TextColor = Color.Yellow;
		}
	}
}
