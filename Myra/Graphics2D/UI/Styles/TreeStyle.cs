namespace Myra.Graphics2D.UI.Styles
{
	public class TreeStyle : WidgetStyle
	{
		public Drawable RowSelectionBackground { get; set; }
		public Drawable RowSelectionBackgroundWithoutFocus { get; set; }

		public ButtonStyle MarkStyle { get; set; }
		public TextBlockStyle LabelStyle { get; set; }

		public TreeStyle()
		{
			MarkStyle = new ButtonStyle();
			LabelStyle = new TextBlockStyle();
		}
	}
}