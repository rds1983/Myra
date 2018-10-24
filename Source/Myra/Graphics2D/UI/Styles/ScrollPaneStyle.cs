namespace Myra.Graphics2D.UI.Styles
{
	public class ScrollPaneStyle : WidgetStyle
	{
		public Drawable HorizontalScrollBackground { get; set; }
		public Drawable HorizontalScrollKnob { get; set; }
		public Drawable VerticalScrollBackground { get; set; }
		public Drawable VerticalScrollKnob { get; set; }

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