namespace Myra.Graphics2D.UI.Styles
{
	public class WindowStyle: WidgetStyle
	{
		public TextBlockStyle TitleStyle { get; set; }
		public ImageButtonStyle CloseButtonStyle { get; set; }

		public WindowStyle()
		{
			TitleStyle = new TextBlockStyle();
			CloseButtonStyle = new ImageButtonStyle();
		}
	}
}
