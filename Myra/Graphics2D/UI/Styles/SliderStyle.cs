namespace Myra.Graphics2D.UI.Styles
{
	public class SliderStyle: WidgetStyle
	{
		public ButtonStyle KnobStyle { get; set; }

		public SliderStyle()
		{
			KnobStyle = new ButtonStyle();
		}
	}
}
