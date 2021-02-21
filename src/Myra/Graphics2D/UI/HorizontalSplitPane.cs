using System.Linq;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class HorizontalSplitPane : SplitPane
	{
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Horizontal;
			}
		}

		public HorizontalSplitPane(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			if (stylesheet.HorizontalSplitPaneStyles.ContainsKey(name))
			{
				ApplySplitPaneStyle(stylesheet.HorizontalSplitPaneStyles[name]);
			}
		}
	}
}