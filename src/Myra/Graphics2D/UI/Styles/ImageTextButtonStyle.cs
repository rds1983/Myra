namespace Myra.Graphics2D.UI.Styles
{
	public class ImageTextButtonStyle : ButtonStyle
	{
		public LabelStyle LabelStyle
		{
			get; set;
		}
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
			LabelStyle = style.LabelStyle != null ? new LabelStyle(style.LabelStyle) : null;
			ImageStyle = style.ImageStyle != null ? new PressableImageStyle(style.ImageStyle) : null;
		}

		public ImageTextButtonStyle(ButtonStyle buttonStyle, LabelStyle textBlockStyle) : base(buttonStyle)
		{
			LabelStyle = textBlockStyle != null ? new LabelStyle(textBlockStyle) : null;
		}

		public override ControlStyle Clone()
		{
			return new ImageTextButtonStyle(this);
		}
	}
}