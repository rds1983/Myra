namespace Myra.Graphics2D.UI.Styles
{
	public class SplitPaneStyle: WidgetStyle
	{
		public ImageButtonStyle HandleStyle { get; set; }

		public SplitPaneStyle()
		{
			HandleStyle = new ImageButtonStyle();
		}

		public SplitPaneStyle(SplitPaneStyle style) : base(style)
		{
			HandleStyle = new ImageButtonStyle(style.HandleStyle);
		}

		public override WidgetStyle Clone()
		{
			return new SplitPaneStyle(this);
		}
	}
}
