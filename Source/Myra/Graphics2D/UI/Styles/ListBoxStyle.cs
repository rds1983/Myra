namespace Myra.Graphics2D.UI.Styles
{
	public class ListBoxStyle: WidgetStyle
	{
		public ButtonStyle ListItemStyle { get; set; }

		public ListBoxStyle()
		{
			ListItemStyle = new ButtonStyle();
		}

		public ListBoxStyle(ListBoxStyle style) : base(style)
		{
			ListItemStyle = new ButtonStyle(style.ListItemStyle);
		}

		public override WidgetStyle Clone()
		{
			return new ListBoxStyle(this);
		}
	}
}
