using System.Linq;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class HorizontalSlider : Slider
	{
		public override Orientation Orientation
		{
			get { return Orientation.Horizontal; }
		}

		public HorizontalSlider(SliderStyle style) : base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		public HorizontalSlider(string style)
			: this(Stylesheet.Current.HorizontalSliderStyles[style])
		{
		}

		public HorizontalSlider() : this(Stylesheet.Current.HorizontalSliderStyle)
		{
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplySliderStyle(stylesheet.HorizontalSliderStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.HorizontalSliderStyles.Keys.ToArray();
		}
	}
}