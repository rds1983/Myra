using System.Linq;
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

		public VerticalSlider(string style)
			: this(Stylesheet.Current.VerticalSliderStyles[style])
		{
		}

		public VerticalSlider()
			: this(Stylesheet.Current.VerticalSliderStyle)
		{
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplySliderStyle(stylesheet.VerticalSliderStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.VerticalSliderStyles.Keys.ToArray();
		}
	}
}