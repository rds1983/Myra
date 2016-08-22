namespace Myra.Graphics2D.UI.Styles
{
	public class ButtonStyle : ButtonBaseStyle
	{
		public TextBlockStyle LabelStyle { get; set; }
		public Drawable Image { get; set; }
		public Drawable PressedImage { get; set; }

		public ButtonStyle()
		{
			LabelStyle = new TextBlockStyle();
		}
	}
}