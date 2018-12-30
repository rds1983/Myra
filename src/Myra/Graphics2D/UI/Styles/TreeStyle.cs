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
			MarkStyle = new ImageButtonStyle();
			LabelStyle = new TextBlockStyle();
		}

		public TreeStyle(TreeStyle style): base(style)
		{
			MarkStyle = new ImageButtonStyle(style.MarkStyle);
			LabelStyle = new TextBlockStyle(style.LabelStyle);
			SelectionBackground = style.SelectionBackground;
			SelectionHoverBackground = style.SelectionHoverBackground;
		}

		public override WidgetStyle Clone()
		{
			return new TreeStyle(this);
		}
	}
}