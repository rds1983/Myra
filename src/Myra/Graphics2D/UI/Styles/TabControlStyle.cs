namespace Myra.Graphics2D.UI.Styles
{
	public class TabControlStyle : WidgetStyle
	{
		public ButtonStyle TabItemStyle
		{
			get; set;
		}
		public TabControlStyle()
		{
			TabItemStyle = new ButtonStyle();
		}

		public TabControlStyle(TabControlStyle style) : base(style)
		{
			TabItemStyle = new ButtonStyle(style.TabItemStyle);
		}

		public override WidgetStyle Clone()
		{
			return new TabControlStyle(this);
		}
	}
}
