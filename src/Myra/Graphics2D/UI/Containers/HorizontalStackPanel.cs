using Myra.Graphics2D.UI.Styles;
using System.Collections;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A stack panel that arranges children horizontally in a row.
	/// </summary>
	public class HorizontalStackPanel : StackPanel
	{
		/// <summary>
		/// Gets the orientation of the stack panel, which is always horizontal.
		/// </summary>
		public override Orientation Orientation => Orientation.Horizontal;

		internal override bool CanStyleBeNull => true;

		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalStackPanel"/> class with the specified stylesheet and style name.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply.</param>
		public HorizontalStackPanel(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName)
		{
			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalStackPanel"/> class with the specified style name.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public HorizontalStackPanel(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.HorizontalStackPanelStyles;
	}
}
