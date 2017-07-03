using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class VerticalSplitPane : SplitPane
	{
		public override Orientation Orientation
		{
			get { return Orientation.Vertical; }
		}

		public VerticalSplitPane(SplitPaneStyle style) : base(style)
		{
		}

		public VerticalSplitPane() : this(Stylesheet.Current.VerticalSplitPaneStyle)
		{
		}
	}
}