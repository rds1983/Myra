namespace Myra.Graphics2D.UI.Styles
{
	public class ScrollViewerStyle : ControlStyle
	{
		public IRenderable HorizontalScrollBackground { get; set; }
		public IRenderable HorizontalScrollKnob { get; set; }
		public IRenderable VerticalScrollBackground { get; set; }
		public IRenderable VerticalScrollKnob { get; set; }

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

		public override ControlStyle Clone()
		{
			return new ScrollViewerStyle(this);
		}
	}
}