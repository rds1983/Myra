using Myra.Graphics2D.UI.Styles;
using System.Collections;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A split pane container that divides space horizontally between two child widgets with a resizable separator.
	/// </summary>
	public class HorizontalSplitPane : SplitPane
	{
		/// <summary>
		/// Gets the orientation of the split pane, which is always horizontal.
		/// </summary>
		public override Orientation Orientation => Orientation.Horizontal;

		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalSplitPane"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public HorizontalSplitPane(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName) : base(stylesheet, styleName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalSplitPane"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public HorizontalSplitPane(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.HorizontalSplitPaneStyles;
	}
}