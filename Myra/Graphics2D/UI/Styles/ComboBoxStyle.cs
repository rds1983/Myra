namespace Myra.Graphics2D.UI.Styles
{
	public class ComboBoxStyle: ButtonStyle
	{
		public WidgetStyle ItemsContainerStyle { get; set; }
		public ListItemStyle ListItemStyle { get; set; }

		public ComboBoxStyle()
		{
			ItemsContainerStyle = new WidgetStyle();
			ListItemStyle = new ListItemStyle();
		}
	}
}