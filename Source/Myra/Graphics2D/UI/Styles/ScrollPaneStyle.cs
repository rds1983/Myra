using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class ScrollPaneStyle : WidgetStyle
	{
		public TextureRegion2D HorizontalScrollBackground { get; set; }
		public TextureRegion2D HorizontalScrollKnob { get; set; }
		public TextureRegion2D VerticalScrollBackground { get; set; }
		public TextureRegion2D VerticalScrollKnob { get; set; }

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