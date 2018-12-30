namespace Myra.Graphics2D.UI.Styles
{
	public class ComboBoxStyle : ButtonStyle
	{
		public WidgetStyle ItemsContainerStyle { get; set; }
		public ButtonStyle ListItemStyle { get; set; }

		public ComboBoxStyle()
		{
			ItemsContainerStyle = new WidgetStyle();
			ListItemStyle = new ButtonStyle();
		}

		public ComboBoxStyle(ComboBoxStyle style) : base(style)
		{
			ItemsContainerStyle = new WidgetStyle(style.ItemsContainerStyle);
			ListItemStyle = new ButtonStyle(style.ListItemStyle);
		}

		public override WidgetStyle Clone()
		{
			return new ComboBoxStyle(this);
		}
	}
}