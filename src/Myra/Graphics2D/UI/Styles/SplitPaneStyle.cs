namespace Myra.Graphics2D.UI.Styles
{
	public class SplitPaneStyle: ControlStyle
	{
		public ButtonStyle HandleStyle { get; set; }

		public SplitPaneStyle()
		{
		}

		public SplitPaneStyle(SplitPaneStyle style) : base(style)
		{
			HandleStyle = style.HandleStyle != null ? new ButtonStyle(style.HandleStyle) : null;
		}

		public override ControlStyle Clone()
		{
			return new SplitPaneStyle(this);
		}
	}
}
