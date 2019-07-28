namespace Myra.Graphics2D.UI.Styles
{
	public class TabControlStyle : WidgetStyle
	{
		public ImageTextButtonStyle TabItemStyle
		{
			get; set;
		}

		public WidgetStyle ContentStyle
		{
			get; set;
		}

		public int ButtonSpacing
		{
			get; set;
		}

		public int HeaderSpacing
		{
			get; set;
		}

		public TabControlStyle()
		{
			TabItemStyle = new ImageTextButtonStyle();
			ContentStyle = new WidgetStyle();
		}

		public TabControlStyle(TabControlStyle style) : base(style)
		{
			TabItemStyle = new ImageTextButtonStyle(style.TabItemStyle);
			ContentStyle = new WidgetStyle(style.ContentStyle);

			ButtonSpacing = style.ButtonSpacing;
			HeaderSpacing = style.HeaderSpacing;
		}

		public override WidgetStyle Clone()
		{
			return new TabControlStyle(this);
		}
	}
}
