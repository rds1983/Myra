namespace Myra.Graphics2D.UI.Styles
{
	public class ComboBoxStyle : ImageTextButtonStyle
	{
		public ListBoxStyle ListBoxStyle { get; set; }

		public ComboBoxStyle()
		{
		}

		public ComboBoxStyle(ComboBoxStyle style) : base(style)
		{
			ListBoxStyle = style.ListBoxStyle != null ? new ListBoxStyle(style.ListBoxStyle) : null;
		}

		public override WidgetStyle Clone()
		{
			return new ComboBoxStyle(this);
		}
	}
}