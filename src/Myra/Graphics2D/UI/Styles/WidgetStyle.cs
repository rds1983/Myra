namespace Myra.Graphics2D.UI.Styles
{
	public class WidgetStyle
	{
		public int? Width { get; set; }
		public int? Height { get; set; }

		public IRenderable Background { get; set; }
		public IRenderable OverBackground { get; set; }
		public IRenderable DisabledBackground { get; set; }
		public IRenderable FocusedBackground { get; set; }

		public IRenderable Border { get; set; }
		public IRenderable OverBorder { get; set; }
		public IRenderable DisabledBorder { get; set; }
		public IRenderable FocusedBorder { get; set; }

		public int? PaddingLeft { get; set; }
		public int? PaddingRight { get; set; }
		public int? PaddingTop { get; set; }
		public int? PaddingBottom { get; set; }

		public WidgetStyle()
		{
		}

		public WidgetStyle(WidgetStyle style)
		{
			Width = style.Width;
			Height = style.Height;
			Background = style.Background;
			OverBackground = style.OverBackground;
			DisabledBackground = style.DisabledBackground;
			FocusedBackground = style.FocusedBackground;
			Border = style.Border;
			OverBorder = style.OverBorder;
			DisabledBorder = style.DisabledBorder;
			FocusedBorder = style.FocusedBorder;
			PaddingLeft = style.PaddingLeft;
			PaddingRight = style.PaddingRight;
			PaddingTop = style.PaddingTop;
			PaddingBottom = style.PaddingBottom;
		}

		public virtual WidgetStyle Clone()
		{
			return new WidgetStyle(this);
		}
	}
}
