using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class ScrollPaneStyle : WidgetStyle
	{
		public TextureRegion HorizontalScrollBackground { get; set; }
		public TextureRegion HorizontalScrollKnob { get; set; }
		public TextureRegion VerticalScrollBackground { get; set; }
		public TextureRegion VerticalScrollKnob { get; set; }

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