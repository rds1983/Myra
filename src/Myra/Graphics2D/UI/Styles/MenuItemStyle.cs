namespace Myra.Graphics2D.UI.Styles
{
	public class MenuItemStyle: ImageTextButtonStyle
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

		public override ControlStyle Clone()
		{
			return new MenuItemStyle(this);
		}
	}
}
