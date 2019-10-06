using System.ComponentModel;
using System.Linq;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class VerticalSlider : Slider
	{
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Vertical;
			}
		}

		[DefaultValue(HorizontalAlignment.Left)]
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

		[DefaultValue(VerticalAlignment.Stretch)]
		public override VerticalAlignment VerticalAlignment
		{
			get
			{
				return base.VerticalAlignment;
			}
			set
			{
				base.VerticalAlignment = value;
			}
		}

		public VerticalSlider(SliderStyle style)
			: base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		public VerticalSlider(Stylesheet stylesheet, string style) :
			this(stylesheet.VerticalSliderStyles[style])
		{
		}

		public VerticalSlider(Stylesheet stylesheet) : this(stylesheet.VerticalSliderStyle)
		{
		}

		public VerticalSlider(string style) : this(Stylesheet.Current, style)
		{
		}

		public VerticalSlider() : this(Stylesheet.Current)
		{
		}

		public override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplySliderStyle(stylesheet.VerticalSliderStyles[name]);
		}
	}
}