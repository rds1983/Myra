using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class GridStyle : WidgetStyle
	{
		public TextureRegion SelectionBackground { get; set; }
		public TextureRegion SelectionHoverBackground { get; set; }

		public GridStyle()
		{
		}

		public GridStyle(GridStyle style): base(style)
		{
			SelectionBackground = style.SelectionBackground;
			SelectionHoverBackground = style.SelectionHoverBackground;
		}

		public override WidgetStyle Clone()
		{
			return new GridStyle(this);
		}
	}
}