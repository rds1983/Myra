using Myra.Graphics2D.UI.Styles;
using System.Collections;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A stack panel that arranges children vertically in a column.
	/// </summary>
	public class VerticalStackPanel : StackPanel
	{
		/// <summary>
		/// Gets the orientation of the stack panel, which is always vertical.
		/// </summary>
		public override Orientation Orientation => Orientation.Vertical;

		internal override bool CanStyleBeNull => true;

		/// <summary>
		/// Initializes a new instance of the <see cref="VerticalStackPanel"/> class with the specified stylesheet and style name.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply.</param>
		public VerticalStackPanel(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName)
		{
			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VerticalStackPanel"/> class with the specified style name.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public VerticalStackPanel(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.VerticalStackPanelStyles;
	}
}
