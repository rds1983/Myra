namespace Myra.Graphics2D.UI.Styles
{
	public class WindowStyle : WidgetStyle
	{
		public TextBlockStyle TitleStyle { get; set; }
		public ImageButtonStyle CloseButtonStyle { get; set; }

		public WindowStyle()
		{
		}

		public WindowStyle(WindowStyle style) : base(style)
		{
			TitleStyle = style.TitleStyle != null ? new TextBlockStyle(style.TitleStyle) : null;
			CloseButtonStyle = style.CloseButtonStyle != null ? new ImageButtonStyle(style.CloseButtonStyle) : null;
		}

		public override WidgetStyle Clone()
		{
			return new WindowStyle(this);
		}
	}
}
