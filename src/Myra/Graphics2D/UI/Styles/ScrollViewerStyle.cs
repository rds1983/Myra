namespace Myra.Graphics2D.UI.Styles
{
	public class ScrollViewerStyle : WidgetStyle
	{
		public IBrush HorizontalScrollBackground { get; set; }
		public IBrush HorizontalScrollKnob { get; set; }
		public IBrush VerticalScrollBackground { get; set; }
		public IBrush VerticalScrollKnob { get; set; }

		public ScrollViewerStyle()
		{
		}

		public ScrollViewerStyle(ScrollViewerStyle style) : base(style)
		{
			HorizontalScrollBackground = style.HorizontalScrollBackground;
			HorizontalScrollKnob = style.HorizontalScrollKnob;
			VerticalScrollBackground = style.VerticalScrollBackground;
			VerticalScrollKnob = style.VerticalScrollKnob;
		}

		public override WidgetStyle Clone()
		{
			return new ScrollViewerStyle(this);
		}
	}
}