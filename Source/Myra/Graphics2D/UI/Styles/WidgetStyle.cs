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

		public WidgetStyle()
		{
		}

		public WidgetStyle(WidgetStyle style)
		{
			WidthHint = style.WidthHint;
			HeightHint = style.HeightHint;
			Background = style.Background;
			OverBackground = style.OverBackground;
			DisabledBackground = style.DisabledBackground;
			Border = style.Border;
			OverBorder = style.OverBorder;
			DisabledBorder = style.DisabledBorder;
			Padding = style.Padding;
		}

		public virtual WidgetStyle Clone()
		{
			return new WidgetStyle(this);
		}
	}
}
