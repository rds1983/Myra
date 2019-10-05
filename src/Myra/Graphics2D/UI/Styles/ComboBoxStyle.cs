namespace Myra.Graphics2D.UI.Styles
{
	public class ComboBoxStyle : ImageTextButtonStyle
	{
		public WidgetStyle ItemsContainerStyle { get; set; }
		public ImageTextButtonStyle ListItemStyle { get; set; }

		public ComboBoxStyle()
		{
		}

		public ComboBoxStyle(ComboBoxStyle style) : base(style)
		{
			ItemsContainerStyle = style.ItemsContainerStyle != null ? new WidgetStyle(style.ItemsContainerStyle) : null;
			ListItemStyle = style.ListItemStyle != null ? new ImageTextButtonStyle(style.ListItemStyle) : null;
		}

		public override WidgetStyle Clone()
		{
			return new ComboBoxStyle(this);
		}
	}
}