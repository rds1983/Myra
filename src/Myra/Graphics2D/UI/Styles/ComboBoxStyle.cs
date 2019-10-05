namespace Myra.Graphics2D.UI.Styles
{
	public class ComboBoxStyle : ImageTextButtonStyle
	{
		public ControlStyle ItemsContainerStyle { get; set; }
		public ImageTextButtonStyle ListItemStyle { get; set; }

		public ComboBoxStyle()
		{
		}

		public ComboBoxStyle(ComboBoxStyle style) : base(style)
		{
			ItemsContainerStyle = style.ItemsContainerStyle != null ? new ControlStyle(style.ItemsContainerStyle) : null;
			ListItemStyle = style.ListItemStyle != null ? new ImageTextButtonStyle(style.ListItemStyle) : null;
		}

		public override ControlStyle Clone()
		{
			return new ComboBoxStyle(this);
		}
	}
}