using System.Linq;
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
			: this(Stylesheet.Current.HorizontalSplitPaneStyles[style])
		{
		}

		public HorizontalSplitPane() : this(Stylesheet.Current.HorizontalSplitPaneStyle)
		{
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplySplitPaneStyle(stylesheet.HorizontalSplitPaneStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.HorizontalSplitPaneStyles.Keys.ToArray();
		}
	}
}