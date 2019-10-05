namespace Myra.Graphics2D.UI.Styles
{
	public class TabControlStyle : ControlStyle
	{
		public ImageTextButtonStyle TabItemStyle
		{
			get; set;
		}

		public ControlStyle ContentStyle
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
		}

		public TabControlStyle(TabControlStyle style) : base(style)
		{
			TabItemStyle = style.TabItemStyle != null ? new ImageTextButtonStyle(style.TabItemStyle) : null;
			ContentStyle = style.ContentStyle != null ? new ControlStyle(style.ContentStyle) : null;

			ButtonSpacing = style.ButtonSpacing;
			HeaderSpacing = style.HeaderSpacing;
		}

		public override ControlStyle Clone()
		{
			return new TabControlStyle(this);
		}
	}
}
