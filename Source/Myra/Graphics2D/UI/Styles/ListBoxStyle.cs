namespace Myra.Graphics2D.UI.Styles
{
	public class ListBoxStyle: WidgetStyle
	{
		public ListItemStyle ListItemStyle { get; set; }

		public ListBoxStyle()
		{
			ListItemStyle = new ListItemStyle();
		}

		public ListBoxStyle(ListBoxStyle style) : base(style)
		{
			ListItemStyle = new ListItemStyle(style.ListItemStyle);
		}

		public override WidgetStyle Clone()
		{
			return new ListBoxStyle(this);
		}
	}
}
