using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class HorizontalSplitPane : SplitPane
	{
		public override Orientation Orientation => Orientation.Horizontal;

		public HorizontalSplitPane(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplySplitPaneStyle(stylesheet.HorizontalSplitPaneStyles.SafelyGetStyle(name));
		}
	}
}