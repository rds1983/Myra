namespace Myra.Graphics2D.UI.Styles
{
	public class MenuStyle: ControlStyle
	{
		public MenuItemStyle MenuItemStyle { get; set; }
		public SeparatorStyle SeparatorStyle { get; set; }

		public MenuStyle()
		{
		}

		public MenuStyle(MenuStyle style) : base(style)
		{
			MenuItemStyle = style.MenuItemStyle != null ? new MenuItemStyle(style.MenuItemStyle) : null;
			SeparatorStyle = style.SeparatorStyle != null ? new SeparatorStyle(style.SeparatorStyle) : null;
		}

		public override ControlStyle Clone()
		{
			return new MenuStyle(this);
		}
	}
}
