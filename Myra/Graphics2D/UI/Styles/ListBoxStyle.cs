namespace Myra.Graphics2D.UI.Styles
{
	public class ListBoxStyle: WidgetStyle
	{
		public ListItemStyle ListItemStyle { get; set; }

		public ListBoxStyle()
		{
			ListItemStyle = new ListItemStyle();
		}
	}
}
