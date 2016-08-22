namespace Myra.Graphics2D.UI.Styles
{
	public class TreeStyle : WidgetStyle
	{
		public Drawable RowOverBackground { get; set; }
		public Drawable RowSelectionBackground { get; set; }

		public ButtonStyle MarkStyle { get; set; }
		public TextBlockStyle LabelStyle { get; set; }

		public TreeStyle()
		{
			MarkStyle = new ButtonStyle();
			LabelStyle = new TextBlockStyle();
		}
	}
}