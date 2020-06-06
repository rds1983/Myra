namespace Myra.Graphics2D.UI.Styles
{
	public class ImageTextButtonStyle : ButtonStyle
	{
		public PressableImageStyle ImageStyle
		{
			get; set;
		}
		public int ImageTextSpacing
		{
			get; set;
		}

		public ImageTextButtonStyle()
		{
		}

		public ImageTextButtonStyle(ImageTextButtonStyle style) : base(style)
		{
			ImageStyle = style.ImageStyle != null ? new PressableImageStyle(style.ImageStyle) : null;
		}

		public ImageTextButtonStyle(ButtonStyle buttonStyle) : base(buttonStyle)
		{
		}

		public override WidgetStyle Clone()
		{
			return new ImageTextButtonStyle(this);
		}
	}
}