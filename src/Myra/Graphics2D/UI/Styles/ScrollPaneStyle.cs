namespace Myra.Graphics2D.UI.Styles
{
	public class ScrollPaneStyle : WidgetStyle
	{
		public IRenderable HorizontalScrollBackground { get; set; }
		public IRenderable HorizontalScrollKnob { get; set; }
		public IRenderable VerticalScrollBackground { get; set; }
		public IRenderable VerticalScrollKnob { get; set; }

		public ScrollPaneStyle()
		{
		}

		public ScrollPaneStyle(ScrollPaneStyle style) : base(style)
		{
			HorizontalScrollBackground = style.HorizontalScrollBackground;
			HorizontalScrollKnob = style.HorizontalScrollKnob;
			VerticalScrollBackground = style.VerticalScrollBackground;
			VerticalScrollKnob = style.VerticalScrollKnob;
		}

		public override WidgetStyle Clone()
		{
			return new ScrollPaneStyle(this);
		}
	}
}