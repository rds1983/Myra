using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class WidgetStyle
	{
		public int? WidthHint { get; set; }
		public int? HeightHint { get; set; }

		public TextureRegion2D Background { get; set; }
		public TextureRegion2D OverBackground { get; set; }
		public TextureRegion2D DisabledBackground { get; set; }

		public TextureRegion2D Border { get; set; }
		public TextureRegion2D OverBorder { get; set; }
		public TextureRegion2D DisabledBorder { get; set; }

		public PaddingInfo Padding;
	}
}
