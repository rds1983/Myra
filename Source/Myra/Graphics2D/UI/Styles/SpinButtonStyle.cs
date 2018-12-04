namespace Myra.Graphics2D.UI.Styles
{
	public class SpinButtonStyle: WidgetStyle
	{
		public ImageButtonStyle UpButtonStyle { get; set; }
		public ImageButtonStyle DownButtonStyle { get; set; }
		public TextFieldStyle TextFieldStyle { get; set; }

		public SpinButtonStyle()
		{
			UpButtonStyle = new ImageButtonStyle();
			DownButtonStyle = new ImageButtonStyle();
			TextFieldStyle = new TextFieldStyle();
		}

		public SpinButtonStyle(SpinButtonStyle style) : base(style)
		{
			UpButtonStyle = new ImageButtonStyle(style.UpButtonStyle);
			DownButtonStyle = new ImageButtonStyle(style.DownButtonStyle);
			TextFieldStyle = new TextFieldStyle(style.TextFieldStyle);
		}

		public override WidgetStyle Clone()
		{
			return new SpinButtonStyle(this);
		}
	}
}
