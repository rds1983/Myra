using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class MenuItemStyle: ButtonStyle
	{
		public int? IconWidth { get; set; }
		public int? ShortcutWidth { get; set; }

		public MenuItemStyle()
		{
		}

		public MenuItemStyle(MenuItemStyle style) : base(style)
		{
			IconWidth = style.IconWidth;
			ShortcutWidth = style.ShortcutWidth;
		}

		public override WidgetStyle Clone()
		{
			return new MenuItemStyle(this);
		}
	}
}
