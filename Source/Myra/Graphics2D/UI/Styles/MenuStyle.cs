namespace Myra.Graphics2D.UI.Styles
{
	public class MenuStyle: WidgetStyle
	{
		public MenuItemStyle MenuItemStyle { get; set; }
		public MenuSeparatorStyle SeparatorStyle { get; set; }

		public MenuStyle()
		{
			MenuItemStyle = new MenuItemStyle();
			SeparatorStyle = new MenuSeparatorStyle();
		}

		public MenuStyle(MenuStyle style) : base(style)
		{
			MenuItemStyle = new MenuItemStyle(style.MenuItemStyle);
			SeparatorStyle = new MenuSeparatorStyle(style.SeparatorStyle);
		}

		public override WidgetStyle Clone()
		{
			return new MenuStyle(this);
		}
	}
}
