using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class ScrollAreaStyle : WidgetStyle
	{
		public TextureRegion2D HorizontalScrollBackground { get; set; }
		public TextureRegion2D HorizontalScrollKnob { get; set; }
		public TextureRegion2D VerticalScrollBackground { get; set; }
		public TextureRegion2D VerticalScrollKnob { get; set; }
	}
}