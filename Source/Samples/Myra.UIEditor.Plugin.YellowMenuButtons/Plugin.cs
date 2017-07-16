using Microsoft.Xna.Framework;
using Myra.Editor.Plugin;
using Myra.Graphics2D.UI.Styles;

namespace Myra.UIEditor.Plugin.YellowMenuButtons
{
	public class Plugin: UIEditorPlugin
	{
		public override void OnLoad()
		{
			var result = Stylesheet.Current;

			result.HorizontalMenuStyle.MenuItemStyle.LabelStyle.TextColor = Color.Yellow;
			result.VerticalMenuStyle.MenuItemStyle.LabelStyle.TextColor = Color.Yellow;
		}
	}
}
