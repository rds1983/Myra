using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class HorizontalProgressBar : ProgressBar
	{
		public override Orientation Orientation
		{
			get { return Orientation.Horizontal; }
		}

		public HorizontalProgressBar(ProgressBarStyle style) : base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		public HorizontalProgressBar(string style)
			: this(Stylesheet.Current.HorizontalProgressBarVariants[style])
		{
		}

		public HorizontalProgressBar()
			: this(Stylesheet.Current.HorizontalProgressBarStyle)
		{
		}
	}
}