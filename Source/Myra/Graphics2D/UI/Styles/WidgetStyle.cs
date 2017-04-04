namespace Myra.Graphics2D.UI.Styles
{
	public class WidgetStyle
	{
		public int? WidthHint { get; set; }
		public int? HeightHint { get; set; }

		public Drawable Background { get; set; }
		public Drawable OverBackground { get; set; }
		public Drawable DisabledBackground { get; set; }

		public Drawable Border { get; set; }
		public Drawable OverBorder { get; set; }
		public Drawable DisabledBorder { get; set; }

		public PaddingInfo Padding;
	}
}
