namespace Myra.Graphics2D.UI.Styles
{
	public class ComboBoxStyle: ButtonStyle
	{
		public WidgetStyle ItemsContainerStyle { get; set; }
		public ComboBoxItemStyle ComboBoxItemStyle { get; set; }

		public ComboBoxStyle()
		{
			ItemsContainerStyle = new WidgetStyle();
			ComboBoxItemStyle = new ComboBoxItemStyle();
		}
	}
}