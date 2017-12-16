using System.Linq;
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

		public VerticalSplitPane(string style)
			: this(Stylesheet.Current.VerticalSplitPaneStyles[style])
		{
		}

		public VerticalSplitPane() : this(Stylesheet.Current.VerticalSplitPaneStyle)
		{
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplySplitPaneStyle(stylesheet.VerticalSplitPaneStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.VerticalSplitPaneStyles.Keys.ToArray();
		}
	}
}