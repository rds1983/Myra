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

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.HorizontalStackPanelStyles;

		internal override bool CanStyleBeNull => true;
	}
}
