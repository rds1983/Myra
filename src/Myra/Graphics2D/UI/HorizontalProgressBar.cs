using System.ComponentModel;
using System.Linq;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class HorizontalProgressBar : ProgressBar
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

		[DefaultValue(VerticalAlignment.Top)]
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

		public HorizontalProgressBar(ProgressBarStyle style) : base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		public HorizontalProgressBar(Stylesheet stylesheet, string style) : this(stylesheet.HorizontalProgressBarStyles[style])
		{
		}

		public HorizontalProgressBar(Stylesheet stylesheet) : this(stylesheet.HorizontalProgressBarStyle)
		{
		}

		public HorizontalProgressBar(string style) : this(Stylesheet.Current, style)
		{
		}

		public HorizontalProgressBar() : this(Stylesheet.Current)
		{
		}

		public override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyProgressBarStyle(stylesheet.HorizontalProgressBarStyles[name]);
		}
	}
}