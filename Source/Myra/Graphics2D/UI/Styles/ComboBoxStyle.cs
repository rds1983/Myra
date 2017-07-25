namespace Myra.Graphics2D.UI.Styles
{
	public class ComboBoxStyle: ButtonStyle
	{
		public WidgetStyle ItemsContainerStyle { get; set; }
		public ListItemStyle ListItemStyle { get; set; }

		public ComboBoxStyle()
		{
			ItemsContainerStyle = new WidgetStyle();
			ListItemStyle = new ListItemStyle();
		}

		public ComboBoxStyle(ComboBoxStyle style): base(style)
		{
			ItemsContainerStyle = new WidgetStyle(style.ItemsContainerStyle);
			ListItemStyle = new ListItemStyle(style.ListItemStyle);
		}

		public override WidgetStyle Clone()
		{
			return new ComboBoxStyle(this);
		}
	}
}