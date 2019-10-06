using System.Linq;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class VerticalSplitPane : SplitPane
	{
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Vertical;
			}
		}

		public VerticalSplitPane(SplitPaneStyle style) : base(style)
		{
		}

		public VerticalSplitPane(Stylesheet stylesheet, string style) :
			this(stylesheet.VerticalSplitPaneStyles[style])
		{
		}

		public VerticalSplitPane(Stylesheet stylesheet) : this(stylesheet.VerticalSplitPaneStyle)
		{
		}

		public VerticalSplitPane(string style) : this(Stylesheet.Current, style)
		{
		}

		public VerticalSplitPane() : this(Stylesheet.Current)
		{
		}

		public override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplySplitPaneStyle(stylesheet.VerticalSplitPaneStyles[name]);
		}
	}
}