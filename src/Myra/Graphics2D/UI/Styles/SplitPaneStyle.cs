namespace Myra.Graphics2D.UI.Styles
{
	public class SplitPanelButtonStyle: ButtonStyle
	{
		public int? HandleSize { get; set; }

		public SplitPanelButtonStyle()
		{
		}

		public SplitPanelButtonStyle(SplitPanelButtonStyle style): base(style)
		{
			HandleSize = style.HandleSize;
		}

		public override WidgetStyle Clone()
		{
			return new SplitPanelButtonStyle(this);
		}
	}

	public class SplitPaneStyle: WidgetStyle
	{
		public SplitPanelButtonStyle HandleStyle { get; set; }

		public SplitPaneStyle()
		{
		}

		public SplitPaneStyle(SplitPaneStyle style) : base(style)
		{
			HandleStyle = style.HandleStyle != null ? new SplitPanelButtonStyle(style.HandleStyle) : null;
		}

		public override WidgetStyle Clone()
		{
			return new SplitPaneStyle(this);
		}
	}
}
