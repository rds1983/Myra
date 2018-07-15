namespace Myra.Graphics2D.UI.Styles
{
	public class TreeStyle : GridStyle
	{
		public ImageButtonStyle MarkStyle { get; set; }
		public TextBlockStyle LabelStyle { get; set; }

		public TreeStyle()
		{
			MarkStyle = new ImageButtonStyle();
			LabelStyle = new TextBlockStyle();
		}

		public TreeStyle(TreeStyle style): base(style)
		{
			MarkStyle = new ImageButtonStyle(style.MarkStyle);
			LabelStyle = new TextBlockStyle(style.LabelStyle);
		}

		public override WidgetStyle Clone()
		{
			return new TreeStyle(this);
		}
	}
}