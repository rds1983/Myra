namespace Myra.Graphics2D.UI.Styles
{
	public class ListBoxStyle: GridStyle
	{
		public ButtonStyle ListItemStyle { get; set; }
		public SeparatorStyle SeparatorStyle { get; set; }

		public ListBoxStyle()
		{
			ListItemStyle = new ButtonStyle();
			SeparatorStyle = new SeparatorStyle();
		}

		public ListBoxStyle(ListBoxStyle style) : base(style)
		{
			ListItemStyle = new ButtonStyle(style.ListItemStyle);
			SeparatorStyle = new SeparatorStyle(style.SeparatorStyle);
		}

		public override WidgetStyle Clone()
		{
			return new ListBoxStyle(this);
		}
	}
}
