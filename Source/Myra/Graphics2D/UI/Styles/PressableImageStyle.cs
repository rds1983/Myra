namespace Myra.Graphics2D.UI.Styles
{
	public class PressableImageStyle: ImageStyle
	{
		public Drawable PressedImage { get; set; }

		public PressableImageStyle()
		{
		}

		public PressableImageStyle(PressableImageStyle style) : base(style)
		{
			PressedImage = style.PressedImage;
		}

		public override WidgetStyle Clone()
		{
			return new PressableImageStyle(this);
		}
	}
}
