namespace Myra.Graphics2D.UI.Styles
{
	public class SplitPaneStyle: WidgetStyle
	{
		public ImageButtonStyle HandleStyle { get; set; }

		public SplitPaneStyle()
		{
			HandleStyle = new ImageButtonStyle();
		}
	}
}
