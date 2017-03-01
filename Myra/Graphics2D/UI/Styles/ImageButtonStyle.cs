namespace Myra.Graphics2D.UI.Styles
{
	public class ImageButtonStyle: ButtonBaseStyle
	{
		public PressableImageStyle ImageStyle { get; set; }

		public ImageButtonStyle()
		{
			ImageStyle = new PressableImageStyle();
		}
	}
}
