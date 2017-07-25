using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class VerticalProgressBar : ProgressBar
	{
		public override Orientation Orientation
		{
			get { return Orientation.Vertical; }
		}

		public VerticalProgressBar(ProgressBarStyle style)
			: base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;
		}

		public VerticalProgressBar(string style)
			: this(Stylesheet.Current.VerticalProgressBarVariants[style])
		{
		}

		public VerticalProgressBar()
			: this(Stylesheet.Current.VerticalProgressBarStyle)
		{
		}
	}
}