namespace Myra.Graphics2D.UI.Styles
{
	public class SpinButtonStyle : WidgetStyle
	{
		public ImageButtonStyle UpButtonStyle
		{
			get; set;
		}
		public ImageButtonStyle DownButtonStyle
		{
			get; set;
		}
		public TextFieldStyle TextFieldStyle
		{
			get; set;
		}

		public SpinButtonStyle()
		{
		}

		public SpinButtonStyle(SpinButtonStyle style) : base(style)
		{
			UpButtonStyle = style.UpButtonStyle != null ? new ImageButtonStyle(style.UpButtonStyle) : null;
			DownButtonStyle = style.DownButtonStyle != null ? new ImageButtonStyle(style.DownButtonStyle) : null;
			TextFieldStyle = style.TextFieldStyle != null ? new TextFieldStyle(style.TextFieldStyle) : null;
		}

		public override WidgetStyle Clone()
		{
			return new SpinButtonStyle(this);
		}
	}
}