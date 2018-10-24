namespace Myra.Graphics2D.UI.Styles
{
	public class MenuStyle: GridStyle
	{
		public MenuItemStyle MenuItemStyle { get; set; }
		public SeparatorStyle SeparatorStyle { get; set; }

		public MenuStyle()
		{
			MenuItemStyle = new MenuItemStyle();
			SeparatorStyle = new SeparatorStyle();
		}

		public MenuStyle(MenuStyle style) : base(style)
		{
			MenuItemStyle = new MenuItemStyle(style.MenuItemStyle);
			SeparatorStyle = new SeparatorStyle(style.SeparatorStyle);
		}

		public override WidgetStyle Clone()
		{
			return new MenuStyle(this);
		}
	}
}
