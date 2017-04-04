namespace Myra.Graphics2D.UI.Styles
{
	public class SpinButtonStyle: WidgetStyle
	{
		public ButtonStyle UpButtonStyle { get; set; }
		public ButtonStyle DownButtonStyle { get; set; }
		public TextFieldStyle TextFieldStyle { get; set; }

		public SpinButtonStyle()
		{
			UpButtonStyle = new ButtonStyle();
			DownButtonStyle = new ButtonStyle();
			TextFieldStyle = new TextFieldStyle();
		}
	}
}
