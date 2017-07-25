namespace Myra.Graphics2D.UI.Styles
{
	public class SliderStyle: WidgetStyle
	{
		public ButtonStyle KnobStyle { get; set; }

		public SliderStyle()
		{
			KnobStyle = new ButtonStyle();
		}

		public SliderStyle(SliderStyle style) : base(style)
		{
			KnobStyle = new ButtonStyle(style.KnobStyle);
		}

		public override WidgetStyle Clone()
		{
			return new SliderStyle(this);
		}
	}
}
