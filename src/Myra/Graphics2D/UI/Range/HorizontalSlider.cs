using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class HorizontalSlider : Slider
	{
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Horizontal;
			}
		}

		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return base.HorizontalAlignment;
			}
			set
			{
				base.HorizontalAlignment = value;
			}
		}

		public HorizontalSlider(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplySliderStyle(stylesheet.HorizontalSliderStyles.SafelyGetStyle(name));
		}
	}
}