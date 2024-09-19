namespace Myra.Graphics2D.UI.Styles
{
	public class TabControlStyle : WidgetStyle
	{
		public ImageTextButtonStyle TabItemStyle { get; set; }

		public WidgetStyle ContentStyle { get; set; }

		public int ButtonSpacing { get; set; }

		public int HeaderSpacing { get; set; }

		public TabSelectorPosition TabSelectorPosition { get; set; }
		public ImageButtonStyle CloseButtonStyle { get; set; }

		public TabControlStyle()
		{
		}

		public TabControlStyle(TabControlStyle style) : base(style)
		{
			TabItemStyle = style.TabItemStyle != null ? new ImageTextButtonStyle(style.TabItemStyle) : null;
			ContentStyle = style.ContentStyle != null ? new WidgetStyle(style.ContentStyle) : null;

			ButtonSpacing = style.ButtonSpacing;
			HeaderSpacing = style.HeaderSpacing;

			TabSelectorPosition = style.TabSelectorPosition;
		}

		public override WidgetStyle Clone()
		{
			return new TabControlStyle(this);
		}
	}
}
