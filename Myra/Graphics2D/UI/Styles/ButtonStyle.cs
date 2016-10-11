namespace Myra.Graphics2D.UI.Styles
{
	public class ButtonStyle : ButtonBaseStyle
	{
		public TextBlockStyle LabelStyle { get; set; }
		public PressableImageStyle ImageStyle { get; set; }

		public ButtonStyle()
		{
			LabelStyle = new TextBlockStyle();
			ImageStyle = new PressableImageStyle();
		}
	}
}