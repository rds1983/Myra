namespace Myra.Graphics2D.UI.Styles
{
	public class ScrollViewerStyle : WidgetStyle
	{
		public IImage HorizontalScrollBackground { get; set; }
		public IImage HorizontalScrollKnob { get; set; }
		public IImage VerticalScrollBackground { get; set; }
		public IImage VerticalScrollKnob { get; set; }

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