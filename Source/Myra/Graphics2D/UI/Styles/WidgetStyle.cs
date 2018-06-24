using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class WidgetStyle
	{
		public int? WidthHint { get; set; }
		public int? HeightHint { get; set; }

		public TextureRegion Background { get; set; }
		public TextureRegion OverBackground { get; set; }
		public TextureRegion DisabledBackground { get; set; }
		public TextureRegion FocusedBackground { get; set; }

		public TextureRegion Border { get; set; }
		public TextureRegion OverBorder { get; set; }
		public TextureRegion DisabledBorder { get; set; }
		public TextureRegion FocusedBorder { get; set; }

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
			FocusedBackground = style.FocusedBackground;
			Border = style.Border;
			OverBorder = style.OverBorder;
			DisabledBorder = style.DisabledBorder;
			FocusedBorder = style.FocusedBorder;
			Padding = style.Padding;
		}

		public virtual WidgetStyle Clone()
		{
			return new WidgetStyle(this);
		}
	}
}
