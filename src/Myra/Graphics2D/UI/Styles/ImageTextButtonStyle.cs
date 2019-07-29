namespace Myra.Graphics2D.UI.Styles
{
	public class ImageTextButtonStyle : ButtonStyle
	{
		public TextBlockStyle LabelStyle
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
			LabelStyle = style.LabelStyle != null ? new TextBlockStyle(style.LabelStyle) : null;
			ImageStyle = style.ImageStyle != null ? new PressableImageStyle(style.ImageStyle) : null;
		}

		public ImageTextButtonStyle(ButtonStyle buttonStyle, TextBlockStyle textBlockStyle) : base(buttonStyle)
		{
			LabelStyle = textBlockStyle != null ? new TextBlockStyle(textBlockStyle) : null;
		}

		public override WidgetStyle Clone()
		{
			return new ImageTextButtonStyle(this);
		}
	}
}