namespace Myra.Graphics2D.UI.Styles
{
	public class ButtonStyle : ButtonBaseStyle
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

		public ButtonStyle()
		{
			LabelStyle = new TextBlockStyle();
			ImageStyle = new PressableImageStyle();
		}

		public ButtonStyle(ButtonStyle style) : base(style)
		{
			LabelStyle = new TextBlockStyle(style.LabelStyle);
			ImageStyle = new PressableImageStyle(style.ImageStyle);
		}

		public override WidgetStyle Clone()
		{
			return new ButtonStyle(this);
		}
	}
}