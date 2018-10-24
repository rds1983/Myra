namespace Myra.Graphics2D.UI.Styles
{
	public class WindowStyle : GridStyle
	{
		public TextBlockStyle TitleStyle { get; set; }
		public ImageButtonStyle CloseButtonStyle { get; set; }

		public WindowStyle()
		{
			TitleStyle = new TextBlockStyle();
			CloseButtonStyle = new ImageButtonStyle();
		}

		public WindowStyle(WindowStyle style) : base(style)
		{
			TitleStyle = new TextBlockStyle(style.TitleStyle);
			CloseButtonStyle = new ImageButtonStyle(style.CloseButtonStyle);
		}

		public override WidgetStyle Clone()
		{
			return new WindowStyle(this);
		}
	}
}
