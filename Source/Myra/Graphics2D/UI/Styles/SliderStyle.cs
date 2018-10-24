namespace Myra.Graphics2D.UI.Styles
{
	public class SliderStyle: WidgetStyle
	{
		public ImageButtonStyle KnobStyle { get; set; }

		public SliderStyle()
		{
			KnobStyle = new ImageButtonStyle();
		}

		public SliderStyle(SliderStyle style) : base(style)
		{
			KnobStyle = new ImageButtonStyle(style.KnobStyle);
		}

		public override WidgetStyle Clone()
		{
			return new SliderStyle(this);
		}
	}
}
