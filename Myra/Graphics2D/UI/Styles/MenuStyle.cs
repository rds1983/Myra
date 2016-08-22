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
	}
}
