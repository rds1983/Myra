namespace Myra.Graphics2D.UI.Styles
{
	public class ImageButtonStyle: ButtonStyle
	{
		public PressableImageStyle ImageStyle { get; set; }

		public ImageButtonStyle()
		{
			ImageStyle = new PressableImageStyle();
		}

		public ImageButtonStyle(ImageButtonStyle style): base(style)
		{
			ImageStyle = new PressableImageStyle(style.ImageStyle);
		}

		public override WidgetStyle Clone()
		{
			return new ImageButtonStyle(this);
		}
	}
}
