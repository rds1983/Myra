namespace Myra.Graphics2D.UI.Styles
{
	public class SplitPaneStyle: WidgetStyle
	{
		public ButtonStyle HandleStyle { get; set; }

		public SplitPaneStyle()
		{
			HandleStyle = new ButtonStyle();
		}
	}
}
