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
			LabelStyle = new TextBlockStyle();
			ImageStyle = new PressableImageStyle();
		}

		public ImageTextButtonStyle(ImageTextButtonStyle style) : base(style)
		{
			LabelStyle = new TextBlockStyle(style.LabelStyle);
			ImageStyle = new PressableImageStyle(style.ImageStyle);
		}

		public override WidgetStyle Clone()
		{
			return new ImageTextButtonStyle(this);
		}
	}
}