namespace Myra.Graphics2D.UI.Styles
{
	public class GridStyle : WidgetStyle
	{
		public Drawable SelectionBackground { get; set; }
		public Drawable SelectionHoverBackground { get; set; }

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