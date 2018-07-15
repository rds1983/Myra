using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class GridStyle : WidgetStyle
	{
		public TextureRegion RowSelectionBackground { get; set; }
		public TextureRegion RowSelectionBackgroundWithoutFocus { get; set; }
		public TextureRegion RowHoverBackground { get; set; }

		public GridStyle()
		{
		}

		public GridStyle(GridStyle style): base(style)
		{
			RowSelectionBackground = style.RowSelectionBackground;
			RowSelectionBackgroundWithoutFocus = style.RowSelectionBackgroundWithoutFocus;
			RowHoverBackground = style.RowHoverBackground;
		}

		public override WidgetStyle Clone()
		{
			return new GridStyle(this);
		}
	}
}