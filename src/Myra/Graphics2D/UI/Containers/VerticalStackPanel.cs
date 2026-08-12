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

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.VerticalStackPanelStyles;

		internal override bool CanStyleBeNull => true;
	}
}
