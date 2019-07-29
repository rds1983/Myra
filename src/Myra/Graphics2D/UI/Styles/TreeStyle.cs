namespace Myra.Graphics2D.UI.Styles
{
	public class TreeStyle : WidgetStyle
	{
		public ImageButtonStyle MarkStyle { get; set; }
		public TextBlockStyle LabelStyle { get; set; }
		public IRenderable SelectionBackground
		{
			get; set;
		}
		public IRenderable SelectionHoverBackground
		{
			get; set;
		}

		public TreeStyle()
		{
		}

		public TreeStyle(TreeStyle style): base(style)
		{
			MarkStyle = style.MarkStyle != null ? new ImageButtonStyle(style.MarkStyle) : null;
			LabelStyle = style.LabelStyle != null ? new TextBlockStyle(style.LabelStyle) : null;
			SelectionBackground = style.SelectionBackground;
			SelectionHoverBackground = style.SelectionHoverBackground;
		}

		public override WidgetStyle Clone()
		{
			return new TreeStyle(this);
		}
	}
}