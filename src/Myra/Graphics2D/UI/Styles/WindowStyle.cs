namespace Myra.Graphics2D.UI.Styles
{
	public class WindowStyle : ControlStyle
	{
		public LabelStyle TitleStyle { get; set; }
		public ImageButtonStyle CloseButtonStyle { get; set; }

		public WindowStyle()
		{
		}

		public WindowStyle(WindowStyle style) : base(style)
		{
			TitleStyle = style.TitleStyle != null ? new LabelStyle(style.TitleStyle) : null;
			CloseButtonStyle = style.CloseButtonStyle != null ? new ImageButtonStyle(style.CloseButtonStyle) : null;
		}

		public override ControlStyle Clone()
		{
			return new WindowStyle(this);
		}
	}
}
