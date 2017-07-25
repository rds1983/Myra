using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class HorizontalSplitPane : SplitPane
	{
		public override Orientation Orientation
		{
			get { return Orientation.Horizontal; }
		}

		public HorizontalSplitPane(SplitPaneStyle style) : base(style)
		{
		}

		public HorizontalSplitPane(string style)
			: this(Stylesheet.Current.HorizontalSplitPaneVariants[style])
		{
		}

		public HorizontalSplitPane() : this(Stylesheet.Current.HorizontalSplitPaneStyle)
		{
		}
	}
}