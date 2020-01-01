namespace Myra.Graphics2D.UI.Styles
{
	public class WidgetStyle: IItemWithId
	{
		public string Id { get; set; }

		public int? Width { get; set; }
		public int? Height { get; set; }

		public int? MinWidth { get; set; }
		public int? MinHeight { get; set; }

		public int? MaxWidth { get; set; }
		public int? MaxHeight { get; set; }

		public IBrush Background { get; set; }
		public IBrush OverBackground { get; set; }
		public IBrush DisabledBackground { get; set; }
		public IBrush FocusedBackground { get; set; }

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
			MinWidth = style.MinWidth;
			MinHeight = style.MinHeight;
			MaxWidth = style.MaxWidth;
			MaxHeight = style.MaxHeight;
			Background = style.Background;
			OverBackground = style.OverBackground;
			DisabledBackground = style.DisabledBackground;
			FocusedBackground = style.FocusedBackground;
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
