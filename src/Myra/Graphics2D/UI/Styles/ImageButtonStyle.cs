namespace Myra.Graphics2D.UI.Styles
{
	public class ImageButtonStyle: ButtonStyle
	{
		public PressableImageStyle ImageStyle { get; set; }

		public ImageButtonStyle()
		{
		}

		public ImageButtonStyle(ImageButtonStyle style): base(style)
		{
			ImageStyle = style.ImageStyle != null ? new PressableImageStyle(style.ImageStyle) : null;
		}

		public ImageButtonStyle(ButtonStyle buttonStyle) : base(buttonStyle)
		{
		}

		public override ControlStyle Clone()
		{
			return new ImageButtonStyle(this);
		}
	}
}
