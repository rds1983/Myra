namespace Myra.Graphics2D.UI.Styles
{
	public class ListBoxStyle: WidgetStyle
	{
		public ImageTextButtonStyle ListItemStyle { get; set; }
		public SeparatorStyle SeparatorStyle { get; set; }

		public ListBoxStyle()
		{
			ListItemStyle = new ImageTextButtonStyle();
			SeparatorStyle = new SeparatorStyle();
		}

		public ListBoxStyle(ListBoxStyle style) : base(style)
		{
			ListItemStyle = new ImageTextButtonStyle(style.ListItemStyle);
			SeparatorStyle = new SeparatorStyle(style.SeparatorStyle);
		}

		public override WidgetStyle Clone()
		{
			return new ListBoxStyle(this);
		}
	}
}
