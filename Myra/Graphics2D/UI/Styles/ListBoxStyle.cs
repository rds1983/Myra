namespace Myra.Graphics2D.UI.Styles
{
	public class ListBoxStyle: WidgetStyle
	{
		public ComboBoxItemStyle ListBoxItemStyle { get; set; }

		public ListBoxStyle()
		{
			ListBoxItemStyle = new ComboBoxItemStyle();
		}
	}
}
