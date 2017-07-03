using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class VerticalSlider : Slider
	{
		public override Orientation Orientation
		{
			get { return Orientation.Vertical; }
		}

		public VerticalSlider(SliderStyle style)
			: base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		public VerticalSlider()
			: this(Stylesheet.Current.VerticalSliderStyle)
		{
		}
	}
}