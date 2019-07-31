using System.ComponentModel;
using System.Linq;
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

		public HorizontalSlider(SliderStyle style) : base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		public HorizontalSlider(Stylesheet stylesheet, string style) : this(stylesheet.HorizontalSliderStyles[style])
		{
		}

		public HorizontalSlider(Stylesheet stylesheet) : this(stylesheet.HorizontalSliderStyle)
		{
		}

		public HorizontalSlider(string style) : this(Stylesheet.Current, style)
		{
		}

		public HorizontalSlider() : this(Stylesheet.Current)
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