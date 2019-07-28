namespace Myra.Graphics2D.UI.Styles
{
	public class ComboBoxStyle : ImageTextButtonStyle
	{
		public WidgetStyle ItemsContainerStyle { get; set; }
		public ImageTextButtonStyle ListItemStyle { get; set; }

		public ComboBoxStyle()
		{
			ItemsContainerStyle = new WidgetStyle();
			ListItemStyle = new ImageTextButtonStyle();
		}

		public ComboBoxStyle(ComboBoxStyle style) : base(style)
		{
			ItemsContainerStyle = new WidgetStyle(style.ItemsContainerStyle);
			ListItemStyle = new ImageTextButtonStyle(style.ListItemStyle);
		}

		public override WidgetStyle Clone()
		{
			return new ComboBoxStyle(this);
		}
	}
}